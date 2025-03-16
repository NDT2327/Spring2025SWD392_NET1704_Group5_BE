using CCSystem.Infrastructure.DTOs.Bookings;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> GetBooking(int id);
        Task<BookingResponse> GetBookingById(int id);
        Task UpdateBookingAsync(Booking booking);
        Task<BookingResponse> CreateBookingWithDetailsAsync(PostBookingRequest postBookingRequest);
        Task<List<BookingResponse>> GetBookingsByCustomer(int customerId);
        Task<BookingResponse> GetByPromotionCodeAsync(string promotionCode);
        Task<bool> RequestCancelBooking(int bookingId, int customerId);
        Task<bool> ProcessRefundBooking(int bookingId, int staffId);
        Task<List<BookingResponse>> GetCancelRequestedBookingsAsync();

	}
}
