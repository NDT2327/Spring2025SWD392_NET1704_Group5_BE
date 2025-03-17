using AutoMapper;
using CCSystem.Infrastructure.DTOs.Bookings;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Profiles.Bookings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingResponse>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Customer.Email))
                .ReverseMap();
        }
    }
}
