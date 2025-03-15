using CCSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.Accounts
{
    public class AccountRegisterRequest
    {
        public string Email {  get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountEnums.Role? Role { get; set; }

    }
}
