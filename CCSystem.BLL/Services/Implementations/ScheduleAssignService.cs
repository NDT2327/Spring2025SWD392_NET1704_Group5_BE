using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.ScheduleAssign;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Enums;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class ScheduleAssignService : IScheduleAssignService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ScheduleAssignService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task AddAsync(PostScheduleAssignRequest request)
        {
            try
            {
                var bDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailById(request.DetailId);
                if (bDetail == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingDetailId);
                }
                if (bDetail.IsAssign == true 
                    || bDetail.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.ASSIGN.ToString()
                    || bDetail.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.COMPLETED.ToString()
                    || bDetail.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.CANCELLED.ToString())
                {
                    throw new ConflictException("Cannot assign this work!");
                }    
                var housekeeper = await _unitOfWork.AccountRepository.GetAccountAsync(request.HousekeeperId);
                if (housekeeper == null || housekeeper.Status != AccountEnums.Status.ACTIVE.ToString())
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                if(housekeeper.Role != AccountEnums.Role.HOUSEKEEPER.ToString())  
                {
                    throw new NotFoundException("Role is not allow to assign work");
                }
                // Lấy serviceDetail từ bDetail
                var serviceDetail = bDetail.ServiceDetail;
                if (serviceDetail == null || serviceDetail.Duration == null)
                {
                    throw new NotFoundException("Service detail or duration is missing.");
                }

                var startTime = bDetail.ScheduleTime;
                var endTime = startTime.AddMinutes(serviceDetail.Duration.Value); // Cộng thêm số phút

                // Kiểm tra Housekeeper đã có công việc trong khoảng thời gian này chưa
                if (await IsOverlappingSchedule(request.HousekeeperId, startTime, endTime))
                {
                    throw new ConflictException(MessageConstant.ScheduleAssign.AlreadyAssign);
                }

                var scheduleAssign = new ScheduleAssignment
                {
                    DetailId = request.DetailId,
                    HousekeeperId = request.HousekeeperId,
                    AssignDate = DateOnly.FromDateTime(DateTime.Now),
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = AssignEnums.Status.ASSIGNED.ToString(),
                    Notes = request.Notes,
                };

                bDetail.IsAssign = true;
                bDetail.BookdetailStatus = BookingDetailEnums.BookingDetailStatus.ASSIGN.ToString();

                await _unitOfWork.ScheduleAssignRepository.AddAsync(scheduleAssign);
                await _unitOfWork.BookingDetailRepository.UpdateBookingDetail(bDetail);

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<bool> IsOverlappingSchedule(int housekeeperId, TimeOnly startTime, TimeOnly endTime)
        {
            return await _unitOfWork.ScheduleAssignRepository.ExistsAsync(sa =>
                sa.HousekeeperId == housekeeperId && 
                ((sa.Status == AssignEnums.Status.ASSIGNED.ToString()) || 
                 (sa.Status == AssignEnums.Status.INPROGRESS.ToString())) &&
                ((startTime >= sa.StartTime && startTime < sa.EndTime) ||
                 (endTime > sa.StartTime && endTime <= sa.EndTime) ||
                 (startTime <= sa.StartTime && endTime >= sa.EndTime))
            );
        }


        public async Task<List<ScheduleAssignmentResponse>> GetAllAsync()
        {
            try
            {
                var assigns = await _unitOfWork.ScheduleAssignRepository.GetAllAsync();
                var responses = _mapper.Map<List<ScheduleAssignmentResponse>>(assigns);
                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ScheduleAssignmentResponse> GetByIdAsync(int id)
        {
            try
            {
                var assign = await _unitOfWork.ScheduleAssignRepository.GetByIdAsync(id);
                if(assign == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAssignId);
                }
                var reponse = _mapper.Map<ScheduleAssignmentResponse>(assign);
                return reponse;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task UpdateAsync(PostScheduleAssignRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
