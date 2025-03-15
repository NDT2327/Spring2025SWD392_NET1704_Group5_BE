using AutoMapper;
using CCSystem.Infrastructure.DTOs.Payments;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Profiles.Payments
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile() {
            CreateMap<Payment, PaymentResponse>().ReverseMap();
        }
    }
}
