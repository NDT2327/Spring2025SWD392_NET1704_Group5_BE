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

        public async Task CreateBookingWithDetailsAsync(PostBookingRequest postBookingRequest)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var customer = await _unitOfWork.AccountRepository.GetByIdAsync(postBookingRequest.CustomerId);
                if (customer == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistAccountId);
                }
                // Tạo booking mới từ thông tin request (giả sử bạn có hàm tạo booking tương tự)
                var booking = new Booking
                {
                    CustomerId = postBookingRequest.CustomerId,
                    PromotionCode = postBookingRequest.PromotionCode?? null,
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
                    if (bookingDetail == null )
                    {
                        throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingDetailId);
                    }    
                    // Tính tổng tiền (có thể thay đổi logic tính giá theo nghiệp vụ)
                    totalAmount +=  bookingDetail.UnitPrice;
                }

                // Cập nhật lại totalAmount cho booking
                booking.TotalAmount = totalAmount;
                await _unitOfWork.BookingRepository.UpdateAsync(booking);
                await _unitOfWork.CommitAsync();

                // Xác nhận transaction
                await transaction.CommitAsync();

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
                //return _mapper.Map<BookingResponse>(booking);
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
