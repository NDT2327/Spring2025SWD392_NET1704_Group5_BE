using AutoMapper;
using CCSystem.BLL.DTOs.AccountTokens;
using CCSystem.DAL.Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Profiles.AccountTokens
{
    public class AccountTokenProfile : Profile
    {
        public AccountTokenProfile()
        {
            CreateMap<AccToken, AccountToken>().ReverseMap();
        }
    }
}
