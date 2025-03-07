using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Payments;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;


        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }


        public async Task CreatePaymentAsync(Payment payment)
        {
            try
            {
                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo thanh toán: " + ex.Message);
            }
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                var payments = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
                //return _mapper.Map<PaymentResponse>(payments);
                return payments;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByCustomerIdAsync(int customerId)
        {
            var payments = await _unitOfWork.PaymentRepository.GetPaymentsByCustomerIdAsync(customerId);

            return payments.Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId,
                CustomerId = p.CustomerId,
                BookingId = p.BookingId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                PaymentDate = p.PaymentDate,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate,
                Notes = p.Notes,
                TransactionId = p.TransactionId
            }).ToList();
        }
        public async Task<PaymentResponse?> GetByBookingIdAsync(int bookingId)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByBookingIdAsync(bookingId);

            if (payment == null)
                return null;

            return new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                CustomerId = payment.CustomerId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate,
                CreatedDate = payment.CreatedDate,
                UpdatedDate = payment.UpdatedDate,
                Notes = payment.Notes,
                TransactionId = payment.TransactionId
            };
        }


        public async Task UpdatePaymentAsync(Payment payment)
        {
            try
            {
                var payments = await _unitOfWork.PaymentRepository.GetByIdAsync(payment.PaymentId);
                if (payment == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPaymentId);
                }
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
