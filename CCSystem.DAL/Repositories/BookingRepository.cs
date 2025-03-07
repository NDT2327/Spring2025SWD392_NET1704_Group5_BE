using CCSystem.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CCSystem.DAL.Models;

namespace CCSystem.DAL.Repositories
{
    public class BookingRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public BookingRepository(SP25_SWD392_CozyCareContext context)
        {
            this._context = context;
        }

        public async Task<List<Booking>> GetBookingsByCustomer(int id)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Where(b => b.CustomerId == id)
                .ToListAsync();
        }
        public async Task AddAsync(Booking booking)
        {
            try
            {
                await _context.Bookings.AddAsync(booking);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding booking", ex);
            }
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.PromotionCode)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all bookings", ex);
            }
        }

        public async Task<Booking> GetByIdAsync(int bookingId)
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.Customer)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking by ID", ex);
            }
        }

        public async Task UpdateAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Update(booking);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating booking", ex);
            }
        }

        //public async Task DeleteAsync(int bookingId)
        //{
        //    try
        //    {
        //        var booking = await _context.Bookings.FindAsync(bookingId);
        //        if (booking != null)
        //        {
        //            _context.Bookings.Remove(booking);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error deleting booking", ex);
        //    }
        //}

    }
}
