using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.BookingDetails;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Enums;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class BookingDetailService : IBookingDetailService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BookingDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork) unitOfWork;
            this._mapper = mapper;
        }

        public async Task CreateBookingDetailAsync(PostBookingDetailRequest postBookingDetailRequest)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(postBookingDetailRequest.BookingId);
                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }
                var service = await _unitOfWork.ServiceRepository.GetServiceAsync(postBookingDetailRequest.ServiceId);
                if (service == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistServiceId);
                }
                var serviceDetail = await _unitOfWork.ServiceDetailRepository.GetByIdAsync(postBookingDetailRequest.ServiceDetailId);
                if (serviceDetail == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistServiceDetailId);
                }

                var bookingDetail = new BookingDetail()
                {
                    BookingId = postBookingDetailRequest.BookingId,
                    Quantity = postBookingDetailRequest.Quantity,
                    ScheduleDate = postBookingDetailRequest.ScheduleDate,
                    ScheduleTime = postBookingDetailRequest.ScheduleTime,
                    UnitPrice = postBookingDetailRequest.Quantity * serviceDetail.BasePrice.Value,
                    ServiceId = postBookingDetailRequest.ServiceId,
                    ServiceDetailId = postBookingDetailRequest.ServiceDetailId,
                    IsAssign = false,
                    BookdetailStatus = BookingDetailEnums.BookingDetailStatus.PENDING.ToString(),
                };
                await _unitOfWork.BookingDetailRepository.CreateBookingDetailAsync(bookingDetail);
                await _unitOfWork.CommitAsync();

            }
            catch(BadRequestException ex)
            {
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                throw new BadRequestException(message);
            }
            catch (NotFoundException ex)
            {
                string message = ErrorUtil.GetErrorString("Failed to create booking detail", ex.Message);
                throw new NotFoundException(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<BookingDetailResponse>> GetBookDetailByBooking(int bookingId)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }
                var detailLists = await _unitOfWork.BookingDetailRepository
                    .GetBookingDetailsByBooking(booking.BookingId);
                var reponses = _mapper.Map<List<BookingDetailResponse>>(detailLists);
                return reponses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BookingDetailResponse> GetBookingDetailById(int id)
        {
            try
            {
                var bDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailById(id);
                if (bDetail == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingDetailId);
                }
                var response = _mapper.Map<BookingDetailResponse>(bDetail);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
