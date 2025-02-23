using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CCSystem.BLL.DTOs.Services;
using CCSystem.DAL.Models;

namespace CCSystem.BLL.Profiles.Services
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<CCSystem.DAL.Models.Service, ServiceResponse>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.CategoryName));
        }
    }
}
