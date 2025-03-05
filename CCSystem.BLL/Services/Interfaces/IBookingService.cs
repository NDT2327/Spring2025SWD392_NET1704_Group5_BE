using CCSystem.BLL.DTOs.Bookings;
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
        Task CreateBookingWithDetailsAsync(PostBookingRequest postBookingRequest);
    }
}
