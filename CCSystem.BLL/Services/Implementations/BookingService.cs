using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Bookings;
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
    public class BookingService : IBookingService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<Booking> GetBooking(int id)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                //return _mapper.Map<BookingResponse>(booking);
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository.GetByIdAsync(booking.BookingId);
                if (bookings == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistBookingId);
                }

                await _unitOfWork.BookingRepository.UpdateAsync(booking);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
