using CCSystem.BLL.DTOs.Payments;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task UpdatePaymentAsync(Payment payment);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByCustomerIdAsync(int customerId);
        Task<PaymentResponse?> GetByBookingIdAsync(int bookingId);
        Task UpdatePaymentWithBooking(int paymentId, PutPaymentWithBooking request);
    }
}
