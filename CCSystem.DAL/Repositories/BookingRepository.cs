﻿using CCSystem.DAL.DBContext;
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
                    .Include(b => b.PromotionCodeNavigation)
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
                    //.Include(b => b.Customer)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking by ID", ex);
            }
        }
        public async Task<Booking> GetByPromotionCodeAsync(string promotionCode)
        {
            return await _context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.PromotionCode == promotionCode);
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

        public async Task<Booking> GetBookingByIdAndCustomer(int bookingId, int customerId)
        {

            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.CustomerId == customerId);

        }

        public async Task<List<Booking>> GetAllBookingAsync()
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.Customer)  
                    .Select(b => new Booking
                    {
                        BookingId = b.BookingId,
                        CustomerId = b.CustomerId,
                        Customer = new Account { Email = b.Customer.Email },
                        PromotionCode = b.PromotionCode,
                        BookingDate = b.BookingDate,
                        TotalAmount = b.TotalAmount,
                        BookingStatus = b.BookingStatus,
                        PaymentStatus = b.PaymentStatus,
                        Notes = b.Notes,
                        PaymentMethod = b.PaymentMethod
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all bookings", ex);
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
