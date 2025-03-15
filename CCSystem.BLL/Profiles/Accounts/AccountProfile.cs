using AutoMapper;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Profiles.Accounts
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<Account, GetAccountResponse>().ReverseMap();
        }
    }
}
