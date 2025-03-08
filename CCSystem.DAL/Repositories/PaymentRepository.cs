using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public class PaymentRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public PaymentRepository(SP25_SWD392_CozyCareContext context)
        {
            this._context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            try
            {
                await _context.Payments.AddAsync(payment);
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý ngoại lệ
                throw new Exception("Error adding payment", ex);
            }
        }

        //public async Task DeleteAsync(int paymentId)
        //{
        //    try
        //    {
        //        var payment = await _context.Payments.FindAsync(paymentId);
        //        if (payment != null)
        //        {
        //            _context.Payments.Remove(payment);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log lỗi hoặc xử lý ngoại lệ
        //        throw new Exception("Error deleting payment", ex);
        //    }
        //}

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            try
            {
                return await _context.Payments
                    .Include(p => p.Booking)
                    .Include(p => p.Customer)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý ngoại lệ
                throw new Exception("Error retrieving all payments", ex);
            }
        }

        public async Task<Payment> GetByIdAsync(int paymentId)
        {
            try
            {
                return await _context.Payments
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý ngoại lệ
                throw new Exception("Error retrieving payment by ID", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByCustomerIdAsync(int customerId)
        {
            return await _context.Set<Payment>()
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }
        public async Task<Payment> GetByBookingIdAsync(int bookingId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy Payment với BookingId: {bookingId}");
            }

            return payment;
        }

        public async Task UpdateAsync(Payment payment)
        {
            try
            {
                _context.Payments.Update(payment);
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý ngoại lệ
                throw new Exception("Error updating payment", ex);
            }
        }
    }
}
