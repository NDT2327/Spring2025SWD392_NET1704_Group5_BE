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
                    //.Include(bd => bd.Booking)
                    //.Include(bd => bd.Service)
                    //.Include(bd => bd.ServiceDetail)
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
                    //.Include(bd => bd.ServiceDetail)
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
                    .Where(bd => bd.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.PENDING.ToString()
                              && bd.IsAssign == false)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BookingDetail>> GetChangeScheduleAsync()
        {
            try
            {
                return await _context.BookingDetails
                    .Include(bd => bd.Booking)
                    .Include(bd => bd.Service)
                    .Include(bd => bd.ServiceDetail)
                    .Where(bd => bd.BookdetailStatus == BookingDetailEnums.BookingDetailStatus.CHANGESCHEDULEREQUESTED.ToString())
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<List<BookingDetail>> GetAllAsync()
        {
            return await _context.BookingDetails
                .Include(bd => bd.Service) 
                .Include(bd => bd.ServiceDetail) 
                .ToListAsync();
        }

        public async Task<bool> RescheduleBookingDetail(int detailId, DateOnly newDate, TimeOnly newTime)
        {
            var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.ScheduleAssignments)
                .FirstOrDefaultAsync(bd => bd.DetailId == detailId);

            if (bookingDetail == null)
            {
                return false;
            }

            // Kiểm tra thời gian chuyển lịch phải trước 24h
            if ((bookingDetail.ScheduleDate.ToDateTime(newTime) - DateTime.Now).TotalHours < 24)
            {
                return false;
            }

            // Hủy lịch cũ
            foreach (var assignment in bookingDetail.ScheduleAssignments)
            {
                assignment.Status = "CANCELLED";
            }

            // Cập nhật ngày, giờ và trạng thái
            bookingDetail.ScheduleDate = newDate;
            bookingDetail.ScheduleTime = newTime;
            bookingDetail.BookdetailStatus = "WAITINGCONFIRM";

            return true;
        }
        public async Task<bool> ConfirmReschedule(int detailId, bool isAccepted)
        {
            var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.ScheduleAssignments)
                .FirstOrDefaultAsync(bd => bd.DetailId == detailId);

            if (bookingDetail == null)
            {
                return false;
            }

            if (isAccepted)
            {
                foreach (var assignment in bookingDetail.ScheduleAssignments)
                {
                    assignment.StartTime = bookingDetail.ScheduleTime;
                    assignment.EndTime = bookingDetail.ScheduleTime.AddHours(1);
                    assignment.Status = "CONFIRMED";
                }
                bookingDetail.BookdetailStatus = "PENDING";
            }
            else
            {
                bookingDetail.BookdetailStatus = "CONFIRMED"; // Giữ lịch cũ
            }

            return true;
        }
    }
}

