using AutoMapper;
using CCSystem.BLL.DTOs.ScheduleAssign;
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
                .ReverseMap();
        }
    }
}
