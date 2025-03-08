using CCSystem.DAL.DBContext;
using CCSystem.DAL.Enums;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public class BookingDetailRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public BookingDetailRepository(SP25_SWD392_CozyCareContext context)
        {
            this._context = context;
        }

        public async Task<List<BookingDetail>> GetBookingDetailsByBooking(int id)
        {
            try
            {
                return await _context.BookingDetails
                    .Include(bd => bd.Booking)
                    .Include(bd => bd.Service)
                    .Include(bd => bd.ServiceDetail)
                    .Where(bd => bd.BookingId == id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateBookingDetailAsync(BookingDetail bookingDetail)
        {
            try
            {
                await this._context.BookingDetails.AddAsync(bookingDetail);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BookingDetail> GetBookingDetailById(int id)
        {
            try
            {
                return await _context.BookingDetails
                    .Include(bd => bd.Booking)
                    .Include(bd => bd.ServiceDetail)
                    .Include(bd => bd.Service)
                    .FirstOrDefaultAsync(bd => bd.DetailId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BookingDetail>> GetBookingDetailsByServiceId(int serviceId)
        {
            try
            {
                return await _context.BookingDetails
                    .Include(bd => bd.Booking)
                    .Include(bd => bd.ServiceDetail)
                    .Where(bd => bd.ServiceId == serviceId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BookingDetail>> GetBookingDetailsByServiceDetailId(int serviceDetailId)
        {
            try
            {
                return await _context.BookingDetails
                    .Include(bd => bd.Booking)
                    .Include(bd => bd.Service)
                    .Where(bd => bd.ServiceDetailId == serviceDetailId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BookingDetail> GetBookingDetailByBookingServiceServiceDetail(int bookingId, int serviceId, int serviceDetailId)
        {
            try
            {
                return await _context.BookingDetails
                    .SingleOrDefaultAsync(bd => bd.BookingId == bookingId 
                                                && bd.ServiceId == serviceId 
                                                && bd.ServiceDetailId == serviceDetailId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BookingDetail>> GetListBookingDetailsAsync()
        {
            try
            {
                return await _context.BookingDetails
                    .Include(bd => bd.Booking)
                    .Include(bd => bd.ServiceDetail)
                    .Include(bd => bd.Service)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateBookingDetail(BookingDetail bookingDetail)
        {
            try
            {
                _context.BookingDetails.Update(bookingDetail);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BookingDetail?> GetExistingBookingDetailAsync(int customerId, int serviceId, int serviceDetailId, DateOnly scheduleDate, TimeOnly scheduleTime)
        {
            return await _context.BookingDetails
                .Include(b => b.Booking) // Join với Booking để lấy CustomerId
                .Where(b => b.Booking.CustomerId == customerId
                            && b.ScheduleDate == scheduleDate // Chuyển đổi DateTime -> DateOnly
                            && b.ScheduleTime == scheduleTime // Chuyển đổi TimeSpan -> TimeOnly
                            && b.ServiceId == serviceId
                            && b.ServiceDetailId == serviceDetailId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<BookingDetail>> GetActiveBookingDetailAsync()
        {
            try
            {
                return await _context.BookingDetails
                    .Include(bd => bd.Booking)
                    .Include(bd => bd.Service)
                    .Include(bd => bd.ServiceDetail)
                    .Where(bd => bd.BookdetailStatus != BookingDetailEnums.BookingDetailStatus.ASSIGNED.ToString() 
                              && bd.BookdetailStatus != BookingDetailEnums.BookingDetailStatus.COMPLETED.ToString()
                              && bd.BookdetailStatus != BookingDetailEnums.BookingDetailStatus.CANCELLED.ToString()
                              && bd.IsAssign == false)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
