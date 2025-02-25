using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Payments;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
