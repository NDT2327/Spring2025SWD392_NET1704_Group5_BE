using AutoMapper;
using Azure.Core;
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
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var bDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailById(request.DetailId);
                if (bDetail == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingDetailId);
                }

                // Nếu đã có housekeeper assign trước đó, không cho phép assign lại
                var existingAssignment = await _unitOfWork.ScheduleAssignRepository.ExistsAsync(sa =>
                    sa.DetailId == request.DetailId &&
                    (sa.Status == AssignEnums.Status.ASSIGNED.ToString() ||
                     sa.Status == AssignEnums.Status.INPROGRESS.ToString()));

                if (existingAssignment)
                {
                    throw new ConflictException(MessageConstant.ScheduleAssign.BookingAlreadyAssigned);
                }

                if (bDetail.IsAssign == true 
                    || bDetail.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.ASSIGNED.ToString()
                    || bDetail.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.COMPLETED.ToString()
                    || bDetail.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.CANCELLED.ToString())
                {
                    throw new ConflictException(MessageConstant.ScheduleAssign.CannotAssignWork);
                }    
                var housekeeper = await _unitOfWork.AccountRepository.GetAccountAsync(request.HousekeeperId);
                if (housekeeper == null || housekeeper.Status != AccountEnums.Status.ACTIVE.ToString())
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                if(housekeeper.Role != AccountEnums.Role.HOUSEKEEPER.ToString())  
                {
                    throw new NotFoundException(MessageConstant.ScheduleAssign.InvalidRole);
                }
                // Lấy serviceDetail từ bDetail
                var serviceDetail = bDetail.ServiceDetail;
                if (serviceDetail == null || serviceDetail.Duration == null)
                {
                    throw new NotFoundException(MessageConstant.ScheduleAssign.MissingServiceDetail);
                }

                var startTime = bDetail.ScheduleTime;
                var endTime = startTime.AddMinutes(serviceDetail.Duration.Value); // Cộng thêm số phút
                var date = bDetail.ScheduleDate;

                // Kiểm tra Housekeeper đã có công việc trong khoảng thời gian này chưa
                if (await IsOverlappingSchedule(request.HousekeeperId, startTime, endTime, date))
                {
                    throw new ConflictException(MessageConstant.ScheduleAssign.AlreadyAssign);
                }

                var scheduleAssign = new ScheduleAssignment
                {
                    DetailId = request.DetailId,
                    HousekeeperId = request.HousekeeperId,
                    AssignDate = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = AssignEnums.Status.ASSIGNED.ToString(),
                    Notes = request.Notes,
                };

                bDetail.IsAssign = true;
                bDetail.BookdetailStatus = BookingDetailEnums.BookingDetailStatus.ASSIGNED.ToString();

                await _unitOfWork.ScheduleAssignRepository.AddAsync(scheduleAssign);
                await _unitOfWork.BookingDetailRepository.UpdateBookingDetail(bDetail);

                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        private async Task<bool> IsOverlappingSchedule(int housekeeperId, TimeOnly startTime, TimeOnly endTime, DateOnly date)
        {
            return await _unitOfWork.ScheduleAssignRepository.ExistsAsync(sa =>
                sa.HousekeeperId == housekeeperId && sa.AssignDate == date &&
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

        public async Task<List<ScheduleAssignmentResponse>> GetAssignsByHousekeeper(int housekeeperId)
        {
            try
            {
                var housekeeper = await _unitOfWork.AccountRepository.GetByIdAsync(housekeeperId);
                if (housekeeper == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                var assigns = await _unitOfWork.ScheduleAssignRepository.GetAssignsByHousekeeper(housekeeper.AccountId);
                var responses = _mapper.Map<List<ScheduleAssignmentResponse>>(assigns);
                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ChangeAssignStatus(PatchAssignStatusRequest request)
        {
            try
            {
                var assign = await _unitOfWork.ScheduleAssignRepository.GetByIdAsync(request.AssignmentId);
                if (assign == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAssignId);
                }
                var validStatuses = new List<string>
                {
                    AssignEnums.Status.ASSIGNED.ToString(),
                    AssignEnums.Status.CANCELLED.ToString(),
                    AssignEnums.Status.COMPLETED.ToString(),
                    AssignEnums.Status.INPROGRESS.ToString(),
                    AssignEnums.Status.WAITINGCONFIRM.ToString()
                };

                if (!validStatuses.Contains(request.Status))
                {
                    throw new BadRequestException(MessageConstant.ScheduleAssign.StatusValidate);
                }

                assign.Status = request.Status;


                await _unitOfWork.ScheduleAssignRepository.UpdateAsync(assign);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CompleteAssignmentAndNotifyCustomer(CompleteAssignmentRequest request)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var assign = await _unitOfWork.ScheduleAssignRepository.GetByIdAsync(request.AssignmentId);
                if (assign == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAssignId);
                }

                if (assign.HousekeeperId != request.HousekeeperId)
                {
                    throw new BadRequestException("This assignment does not belong to the specified housekeeper.");
                }

                var changeStatusRequest = new PatchAssignStatusRequest
                {
                    AssignmentId = assign.AssignmentId,
                    Status = AssignEnums.Status.WAITINGCONFIRM.ToString(),
                };

                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailById(assign.DetailId);
                if (bookingDetail == null)
                {
                    throw new NotFoundException("Booking detail not found.");
                }

                bookingDetail.BookdetailStatus = BookingDetailEnums.BookingDetailStatus.WAITINGCONFIRM.ToString();
                await _unitOfWork.BookingDetailRepository.UpdateBookingDetail(bookingDetail);

                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingDetail.BookingId);
                var customer = await _unitOfWork.AccountRepository.GetByIdAsync(booking.CustomerId);
                await _unitOfWork.EmailRepository.SendEmailToCustomerConfirm(customer.Email);
                await ChangeAssignStatus(changeStatusRequest);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
            
        }

        public async Task ConfirmAssignment(ConfirmAssignmentRequest request)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Lấy thông tin BookingDetail theo BookingDetailId được truyền vào
                var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailById(request.BookingDetailId);
                if (bookingDetail == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingDetailId);
                }
                if (bookingDetail.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.COMPLETED.ToString())
                {
                    throw new BadRequestException(MessageConstant.BookingDetailMessage.CompletedBookingDetail);
                }

                // Lấy thông tin Booking để kiểm tra xem khách hàng có khớp không
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingDetail.BookingId);
                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }

                if (booking.CustomerId != request.CustomerId)
                {
                    throw new BadRequestException("This booking detail does not belong to the specified customer.");
                }

                // Lấy danh sách các ScheduleAssignment dựa trên DetailId của BookingDetail
                var assignments = await _unitOfWork.ScheduleAssignRepository.GetAssignmentByDetailId(bookingDetail.DetailId);
                if (assignments == null || !assignments.Any())
                {
                    throw new NotFoundException("Assignment not found.");
                }

                // Cập nhật trạng thái của các assignment có trạng thái WAITINGCONFIRM thành COMPLETED
                foreach (var assignment in assignments)
                {
                    if (assignment.Status == AssignEnums.Status.WAITINGCONFIRM.ToString())
                    {
                        assignment.Status = AssignEnums.Status.COMPLETED.ToString();
                        await _unitOfWork.ScheduleAssignRepository.UpdateAsync(assignment);
                    }
                }

                // Cập nhật trạng thái của bookingDetail thành COMPLETED
                bookingDetail.BookdetailStatus = BookingDetailEnums.BookingDetailStatus.COMPLETED.ToString();
                await _unitOfWork.BookingDetailRepository.UpdateBookingDetail(bookingDetail);

                // Commit các thay đổi
                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error during assignment confirmation: " + ex.Message);
            }
        }

    }
}
