using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.BookingDetails;
using CCSystem.BLL.DTOs.Bookings;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Enums;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IBookingDetailService _bookDetailService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IBookingDetailService bookingDetailService)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._bookDetailService = bookingDetailService;
        }

        public async Task<BookingResponse> CreateBookingWithDetailsAsync(PostBookingRequest postBookingRequest)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var customer = await _unitOfWork.AccountRepository.GetByIdAsync(postBookingRequest.CustomerId);
                if (customer == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                var promotion = await _unitOfWork.PromotionRepository.GetPromotionByCodeAsync(postBookingRequest.PromotionCode);
                string? promotionCode = promotion?.Code;


                // Tạo booking mới từ thông tin request (giả sử bạn có hàm tạo booking tương tự)
                var booking = new Booking
                {
                    CustomerId = postBookingRequest.CustomerId,
                    PromotionCode = promotionCode,
                    BookingDate = DateTime.UtcNow,
                    BookingStatus = BookingEnums.BookingStatus.PENDING.ToString(),
                    PaymentStatus = BookingEnums.PaymentStatus.PENDING.ToString(),
                    Notes = postBookingRequest.Notes,
                    PaymentMethod = postBookingRequest.PaymentMethod,
                    Address = postBookingRequest.Address,
                    TotalAmount = 0 // Sẽ tính lại sau khi thêm các booking detail
                };

                // Thêm booking vào repository
                await _unitOfWork.BookingRepository.AddAsync(booking);
                await _unitOfWork.CommitAsync();

                decimal totalAmount = 0;

                // Với mỗi dịch vụ trong request, tạo booking detail tương ứng
                foreach (var detail in postBookingRequest.BookingDetails)
                {
                    // Kiểm tra xem khách hàng đã đặt dịch vụ này vào cùng thời gian chưa
                    var existingBookingDetail = await _unitOfWork.BookingDetailRepository
                        .GetExistingBookingDetailAsync(postBookingRequest.CustomerId, detail.ServiceId, detail.ServiceDetailId, detail.ScheduleDate, detail.ScheduleTime);

                    if (existingBookingDetail != null)
                    {
                        throw new BadRequestException("Customer has already booked this service at the selected date and time.");
                    }

                    // Gán BookingId vừa tạo cho mỗi booking detail
                    var postBookingDetailRequest = new PostBookingDetailRequest
                    {
                        BookingId = booking.BookingId,
                        ServiceId = detail.ServiceId,
                        Quantity = detail.Quantity,
                        ScheduleDate = detail.ScheduleDate,
                        ScheduleTime = detail.ScheduleTime,
                        ServiceDetailId = detail.ServiceDetailId,
                        //UnitPrice = detail.Quantity,//postBookingDetailRequest.Quantity*serviceDetail.basePrice

                    };

                    await _bookDetailService.CreateBookingDetailAsync(postBookingDetailRequest);
                    var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailByBookingServiceServiceDetail(
                        booking.BookingId, detail.ServiceId, detail.ServiceDetailId);
                    if (bookingDetail == null)
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingDetailId);
                    }
                    // Tính tổng tiền (có thể thay đổi logic tính giá theo nghiệp vụ)
                    totalAmount += bookingDetail.UnitPrice;
                }

                if (promotion != null)
                {
                    DateTime now = DateTime.UtcNow;

                    // Kiểm tra thời gian hợp lệ của Promotion
                    if (promotion.StartDate > now || promotion.EndDate < now)
                    {
                        throw new BadRequestException("Promotion is not valid at this time.");
                    }

                    // Kiểm tra điều kiện tối thiểu để áp dụng giảm giá
                    if (promotion.MinOrderAmount.HasValue && totalAmount < promotion.MinOrderAmount.Value)
                    {
                        throw new BadRequestException($"Total amount must be at least {promotion.MinOrderAmount.Value} to use this promotion.");
                    }

                    decimal discount = 0;

                    // Giảm giá cố định
                    if (promotion.DiscountAmount.HasValue)
                    {
                        discount = promotion.DiscountAmount.Value;
                    }

                    // Giảm giá theo phần trăm
                    if (promotion.DiscountPercent.HasValue)
                    {
                        decimal percentageDiscount = totalAmount * (decimal)(promotion.DiscountPercent.Value / 100.0);

                        // Áp dụng giới hạn giảm tối đa (nếu có)
                        if (promotion.MaxDiscountAmount.HasValue)
                        {
                            percentageDiscount = Math.Min(percentageDiscount, promotion.MaxDiscountAmount.Value);
                        }

                        // Lấy mức giảm giá lớn nhất giữa DiscountAmount và DiscountPercent
                        discount = Math.Max(discount, percentageDiscount);
                    }

                    // Đảm bảo totalAmount không bị âm
                    totalAmount = Math.Max(totalAmount - discount, 0);
                }

                // Cập nhật lại totalAmount cho booking
                booking.TotalAmount = totalAmount;
                await _unitOfWork.BookingRepository.UpdateAsync(booking);
                await _unitOfWork.CommitAsync();

                // Xác nhận transaction
                await transaction.CommitAsync();
                return _mapper.Map<BookingResponse>(booking);

            }
            catch (BadRequestException ex)
            {
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                throw new BadRequestException(message);
            }
            catch (NotFoundException ex)
            {
                string message = ErrorUtil.GetErrorString("Failed to create booking", ex.Message);
                throw new NotFoundException(message);
            }
            catch (Exception ex)
            {
                // Hoàn tác tất cả thay đổi nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi tạo booking và booking details: {ex.Message}");
            }
        }

        public async Task<Booking> GetBooking(int id)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }
                //return _mapper.Map<BookingResponse>(booking);
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BookingResponse> GetBookingById(int id)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }
                var response = _mapper.Map<BookingResponse>(booking);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BookingResponse>> GetBookingsByCustomer(int customerId)
        {
            try
            {
                var cus = await _unitOfWork.AccountRepository.GetByIdAsync(customerId);
                if (cus == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                var bookings = await _unitOfWork.BookingRepository.GetBookingsByCustomer(cus.AccountId);

                var responses = _mapper.Map<List<BookingResponse>>(bookings);
                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<BookingResponse?> GetByPromotionCodeAsync(string promotionCode)
        {
            var booking = await _unitOfWork.BookingRepository.GetByPromotionCodeAsync(promotionCode);

            if (booking == null)
                return null;

            return _mapper.Map<BookingResponse>(booking);
        }

        public async Task<bool> RequestCancelBooking(int bookingId, int customerId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetBookingByIdAndCustomer(bookingId, customerId);

                if (booking == null)
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);

                var now = DateTime.UtcNow;
                bool allExpired = true;
                decimal refundAmount = 0;

                var bookingDetails = await _unitOfWork.BookingDetailRepository.GetBookingDetailsByBooking(booking.BookingId);

                foreach (var detail in bookingDetails)
                {
                    DateTime scheduleDateTime = detail.ScheduleDate.ToDateTime(detail.ScheduleTime);

                    // Nếu vẫn còn hiệu lực, cộng vào tiền hoàn
                    if ((scheduleDateTime - now).TotalHours > 24)
                    {
                        refundAmount += detail.UnitPrice;
                        allExpired = false;
                    }
                    detail.BookdetailStatus = BookingDetailEnums.BookingDetailStatus.CANCELREQUESTED.ToString();
                    await _unitOfWork.BookingDetailRepository.UpdateBookingDetail(detail);

                }

                if (allExpired)
                {
                    throw new Exception(MessageConstant.BookingMessage.BookingIsExpired);
                }

                // Cập nhật trạng thái Booking
                booking.BookingStatus = BookingEnums.BookingStatus.CANCELREQUESTED.ToString();

                // Nếu có thanh toán trước, tạo giao dịch hoàn tiền
                var payment = await _unitOfWork.PaymentRepository.GetSuccessfulPaymentByBookingIdAsync(booking.BookingId);
                if (payment != null && refundAmount > 0)
                {
                    var refundPayment = new Payment
                    {
                        CustomerId = booking.CustomerId,
                        BookingId = booking.BookingId,
                        Amount = refundAmount,
                        PaymentMethod = payment.PaymentMethod,
                        Status = PaymentEnums.Status.REFUNDREQUESTED.ToString(),
                        CreatedDate = DateTime.Now,
                        Notes = "Yêu cầu hoàn tiền cho các dịch vụ chưa hết hạn"
                    };
                    await _unitOfWork.PaymentRepository.AddAsync(refundPayment);
                }
                await _unitOfWork.BookingRepository.UpdateAsync(booking);
                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();

                // Gửi thông báo cho Staff xử lý
                //await _notificationService.NotifyStaffAsync($"Yêu cầu hủy Booking #{bookingId} cần xử lý!");

                return true;
            }
            catch (Exception ex)
            {
                // Hoàn tác tất cả thay đổi nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi tạo booking và booking details: {ex.Message}");
            }
        }

        public async Task<bool> ProcessRefundBooking(int bookingId, int staffId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);

                if (booking == null)
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);

                if (booking.BookingStatus != BookingEnums.BookingStatus.CANCELREQUESTED.ToString())
                    throw new NotFoundException(MessageConstant.BookingMessage.IsNotRequestCanceled);

                var staff = await _unitOfWork.AccountRepository.GetByIdAsync(staffId);
                if (staff == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                if (staff.Role != AccountEnums.Role.STAFF.ToString())
                {
                    throw new NotFoundException(MessageConstant.AccountMessage.InvalidRole);
                }

                var details = await _unitOfWork.BookingDetailRepository.GetBookingDetailsByBooking(booking.BookingId);

                // Cập nhật trạng thái các BookingDetail đã yêu cầu hủy
                foreach (var detail in details.Where(d => d.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.CANCELREQUESTED.ToString()))
                {
                    detail.BookdetailStatus = BookingDetailEnums.BookingDetailStatus.CANCELLED.ToString();

                    // Nếu có phân công Housekeeper, cập nhật trạng thái `ScheduleAssignment`
                    var assignments = await _unitOfWork.ScheduleAssignRepository.GetAssignmentByDetailId(detail.DetailId);
                    foreach (var assign in assignments)
                    {
                        assign.Status = AssignEnums.Status.CANCELLED.ToString();
                        await _unitOfWork.ScheduleAssignRepository.UpdateAsync(assign);
                    }
                    await _unitOfWork.BookingDetailRepository.UpdateBookingDetail(detail);
                }

                // Cập nhật trạng thái hoàn tiền
                var refundPayment = await _unitOfWork.PaymentRepository.GetRefundRequestedPaymentByBookingIdAsync(booking.BookingId);
                if (refundPayment != null)
                {
                    refundPayment.Status = PaymentEnums.Status.REFUNDED.ToString();
                    refundPayment.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPaymentId);
                }


                booking.BookingStatus = BookingEnums.BookingStatus.CANCELED.ToString();
                booking.PaymentStatus = BookingEnums.PaymentStatus.REFUNDED.ToString();

                await _unitOfWork.PaymentRepository.UpdateAsync(refundPayment);
                await _unitOfWork.BookingRepository.UpdateAsync(booking);

                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();

                // Gửi thông báo cho khách hàng
                //await _notificationService.NotifyCustomerAsync(booking.CustomerId, $"Booking #{bookingId} đã được hoàn tiền!");

                return true;
            }
            catch (Exception ex)
            {
                // Hoàn tác tất cả thay đổi nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi tạo booking và booking details: {ex.Message}");
            }
        }


        public async Task UpdateBookingAsync(Booking booking)
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository.GetByIdAsync(booking.BookingId);
                if (bookings == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }

                await _unitOfWork.BookingRepository.UpdateAsync(booking);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
