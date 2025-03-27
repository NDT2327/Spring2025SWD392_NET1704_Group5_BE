using CCSystem.Presentation.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace CCSystem.Presentation.Models.Accounts
{
    public class AccountRegisterRequest
    {
        public string Email {  get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; } = Common.Role.Customer;

    }
}
