using AutoMapper;
using CCSystem.BLL.DTOs.BookingDetails;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Profiles.BookingDetails
{
    public class BookingDetailProfile : Profile
    {
        public BookingDetailProfile()
        {
            CreateMap<BookingDetail, BookingDetailResponse>().ReverseMap();
        }
    }
}
