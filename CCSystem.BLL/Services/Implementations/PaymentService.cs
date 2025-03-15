using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.Infrastructure.DTOs.Payments;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Enums;
using CCSystem.Infrastructure.Infrastructures;
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
using VNPAY.NET.Models;

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
        public async Task<IEnumerable<CCSystem.Infrastructure.DTOs.Payments.PaymentResponse>> GetPaymentsByCustomerIdAsync(int customerId)
        {
            var payments = await _unitOfWork.PaymentRepository.GetPaymentsByCustomerIdAsync(customerId);

            return payments.Select(p => new CCSystem.Infrastructure.DTOs.Payments.PaymentResponse
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
        public async Task<CCSystem.Infrastructure.DTOs.Payments.PaymentResponse?> GetByBookingIdAsync(int bookingId)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByBookingIdAsync(bookingId);

            if (payment == null)
                return null;

            return new CCSystem.Infrastructure.DTOs.Payments.PaymentResponse
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

        public async Task UpdatePaymentWithBooking(int paymentId, PutPaymentWithBooking request)
        {
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPaymentId);
                }
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(payment.BookingId);
                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }
                payment.TransactionId = request.TransactionId;
                payment.Status = request.Status;
                payment.PaymentMethod = request.PaymentMethod;
                var validStatuses = new List<string>
                {
                    PaymentEnums.Status.SUCCESS.ToString(),
                    PaymentEnums.Status.FAILED.ToString(),
                    PaymentEnums.Status.PENDING.ToString()
                };

                if (!validStatuses.Contains(request.Status))
                {
                    throw new BadRequestException(MessageConstant.PaymentMessage.StatusMustSuccessOrFailedorPending);
                }

                if (payment.Status == PaymentEnums.Status.SUCCESS.ToString())
                {

                    booking.PaymentStatus = BookingEnums.PaymentStatus.PAID.ToString();
                    booking.BookingStatus = BookingEnums.BookingStatus.CONFIRMED.ToString();
                    await _unitOfWork.BookingRepository.UpdateAsync(booking);

                }

                payment.UpdatedDate = DateTime.UtcNow;

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
