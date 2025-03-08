using CCSystem.BLL.DTOs.BookingDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IBookingDetailService
    {
        Task CreateBookingDetailAsync(PostBookingDetailRequest postBookingDetailRequest);
        Task<BookingDetailResponse> GetBookingDetailById(int id);
        Task<List<BookingDetailResponse>> GetBookDetailByBooking(int bookingId);
        Task<List<BookingDetailResponse>> GetBookingDetailsByServiceIdAsync(int serviceId);
        Task<List<BookingDetailResponse>> GetBookingDetailsByServiceDetailIdAsync(int serviceDetailId);
    }
}
