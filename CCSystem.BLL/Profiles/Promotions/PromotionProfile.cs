using AutoMapper;
using CCSystem.BLL.DTOs.Promotions;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Profiles.Promotions
{
    public class PromotionProfile : Profile
    {
        public PromotionProfile()
        {
            CreateMap<PostPromotionRequest, Promotion>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<PutPromotionRequest, Promotion>()
                .ForMember(dest => dest.Code, opt => opt.Ignore());
        }
    }
}
