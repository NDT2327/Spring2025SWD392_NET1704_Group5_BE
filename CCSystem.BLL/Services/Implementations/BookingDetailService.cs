using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Enums;
using CCSystem.Infrastructure.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class BookingDetailService : IBookingDetailService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BookingDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork) unitOfWork;
            this._mapper = mapper;
        }

        public async Task CreateBookingDetailAsync(PostBookingDetailRequest postBookingDetailRequest)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(postBookingDetailRequest.BookingId);
                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }
                var service = await _unitOfWork.ServiceRepository.GetServiceAsync(postBookingDetailRequest.ServiceId);
                if (service == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistServiceId);
                }
                var serviceDetail = await _unitOfWork.ServiceDetailRepository.GetByIdAsync(postBookingDetailRequest.ServiceDetailId);
                if (serviceDetail == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistServiceDetailId);
                }

                var bookingDetail = new BookingDetail()
                {
                    BookingId = postBookingDetailRequest.BookingId,
                    Quantity = postBookingDetailRequest.Quantity,
                    ScheduleDate = postBookingDetailRequest.ScheduleDate,
                    ScheduleTime = postBookingDetailRequest.ScheduleTime,
                    UnitPrice = postBookingDetailRequest.Quantity * serviceDetail.BasePrice.Value,
                    ServiceId = postBookingDetailRequest.ServiceId,
                    ServiceDetailId = postBookingDetailRequest.ServiceDetailId,
                    IsAssign = false,
                    BookdetailStatus = BookingDetailEnums.BookingDetailStatus.PENDING.ToString(),
                };
                await _unitOfWork.BookingDetailRepository.CreateBookingDetailAsync(bookingDetail);
                await _unitOfWork.CommitAsync();

            }
            catch(BadRequestException ex)
            {
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                throw new BadRequestException(message);
            }
            catch (NotFoundException ex)
            {
                string message = ErrorUtil.GetErrorString("Failed to create booking detail", ex.Message);
                throw new NotFoundException(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<BookingDetailResponse>> GetActiveBookingDetail()
        {
            try
            {
                var bdetails = await _unitOfWork.BookingDetailRepository.GetActiveBookingDetailAsync();
                var responses = _mapper.Map<List<BookingDetailResponse>>(bdetails);
                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task<List<BookingDetailResponse>> GetBookDetailByBooking(int bookingId)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }
                var detailLists = await _unitOfWork.BookingDetailRepository
                    .GetBookingDetailsByBooking(booking.BookingId);
                var reponses = _mapper.Map<List<BookingDetailResponse>>(detailLists);
                return reponses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<BookingDetailResponse>> GetBookingDetailsByServiceIdAsync(int serviceId)
        {
            var bookingDetails = await _unitOfWork.BookingDetailRepository.GetBookingDetailsByServiceId(serviceId);
            return _mapper.Map<List<BookingDetailResponse>>(bookingDetails);
        }

        public async Task<List<BookingDetailResponse>> GetBookingDetailsByServiceDetailIdAsync(int serviceDetailId)
        {
            var bookingDetails = await _unitOfWork.BookingDetailRepository.GetBookingDetailsByServiceDetailId(serviceDetailId);
            return _mapper.Map<List<BookingDetailResponse>>(bookingDetails);
        }

        public async Task<BookingDetailResponse> GetBookingDetailById(int id)
        {
            try
            {
                var bDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailById(id);
                if (bDetail == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingDetailId);
                }
                var response = _mapper.Map<BookingDetailResponse>(bDetail);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<RescheduleResponse> RescheduleBookingDetail(int detailId, RescheduleRequest request)
        {
            
            var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailById(detailId);
            if (bookingDetail == null)
            {
                throw new Exception(MessageConstant.BookingDetailMessage.BookingDetailNotFound);
            }
            var oldDateTime = bookingDetail.ScheduleDate.ToDateTime(bookingDetail.ScheduleTime);
            var newDateTime = request.NewDate.ToDateTime(request.NewTime);
            if (newDateTime < oldDateTime)
            {
                throw new Exception(MessageConstant.BookingDetailMessage.NewDateEarlierThanCurrent);
            }
            // Kiểm tra nếu thời gian mới trừ thời gian cũ nhỏ hơn 24 giờ
            if ((newDateTime - oldDateTime).TotalHours > 24)
            {
                throw new Exception(MessageConstant.BookingDetailMessage.RescheduleMustBe24HoursApart);
            }

            // Cập nhật lịch
            bookingDetail.ScheduleDate = request.NewDate;
            bookingDetail.ScheduleTime = request.NewTime;
            bookingDetail.BookdetailStatus = "WAITINGCONFIRM";

            // Hủy lịch cũ
            var assignments = await _unitOfWork.ScheduleAssignRepository.GetAssignmentByDetailId(detailId);
            if (assignments != null && assignments.Any())
            {
                foreach (var assignment in assignments)
                {
                    assignment.Status = "CANCELLED";
                    await _unitOfWork.ScheduleAssignRepository.UpdateAsync(assignment);
                }
            }

            await _unitOfWork.BookingDetailRepository.UpdateBookingDetail(bookingDetail);
            await _unitOfWork.CommitAsync();

            return new RescheduleResponse
            {
                DetailId = detailId,
                ScheduleDate = request.NewDate,
                ScheduleTime = request.NewTime,
                Status = "WAITINGCONFIRM"
            };
        }

        public async Task<ConfirmRescheduleResponse> ConfirmReschedule(int detailId, ConfirmRescheduleRequest request)
        {
            var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailById(detailId);
            if (bookingDetail == null)
            {
                throw new Exception("Booking detail not found");
            }
            if (request.IsAccepted)
            {
                var assignments = await _unitOfWork.ScheduleAssignRepository.GetAssignmentByDetailId(detailId);
                if (assignments != null && assignments.Any())
                {
                    foreach (var assignment in assignments)
                    {
                        assignment.AssignDate = bookingDetail.ScheduleDate; 
                        assignment.StartTime = bookingDetail.ScheduleTime;
                        assignment.EndTime = bookingDetail.ScheduleTime.AddHours(1);
                        assignment.Status = "WAITINGCONFIRM";
                        await _unitOfWork.ScheduleAssignRepository.UpdateAsync(assignment);
                    }
                }
                bookingDetail.BookdetailStatus = "PENDING";
            }
            else
            {

                bookingDetail.BookdetailStatus = "CANCELLED";
                bookingDetail.IsAssign = false;
                var assignments = await _unitOfWork.ScheduleAssignRepository.GetAssignmentByDetailId(detailId);
                if (assignments != null && assignments.Any())
                {
                    foreach (var assignment in assignments)
                    {
                        assignment.Status = "CANCELLED"; // Hủy lịch
                        await _unitOfWork.ScheduleAssignRepository.UpdateAsync(assignment);
                    }
                }
            
        }

            await _unitOfWork.BookingDetailRepository.UpdateBookingDetail(bookingDetail);
            await _unitOfWork.CommitAsync();

            return new ConfirmRescheduleResponse
            {
                DetailId = detailId,
                Message = request.IsAccepted ? "Reschedule confirmed" : "Reschedule rejected",
                Status = bookingDetail.BookdetailStatus
            };
        }
    }
}

