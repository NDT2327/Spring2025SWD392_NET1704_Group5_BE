using AutoMapper;
using CCSystem.Infrastructure.DTOs.ScheduleAssign;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Profiles.ScheduleAssigns
{
    public class ScheduleAssignProfile : Profile
    {
        public ScheduleAssignProfile()
        {
            CreateMap<ScheduleAssignment, ScheduleAssignmentResponse>()
                .ForMember(dept => dept.HouseKeeperName, opt => opt.MapFrom(src => src.Housekeeper.FullName))
                .ForMember(dept => dept.Email, opt => opt.MapFrom(src => src.Housekeeper.Email))
                .ReverseMap();
        }
	}
}
