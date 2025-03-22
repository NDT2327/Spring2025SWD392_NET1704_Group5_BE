using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.Accounts
{
    public class CreateAccountRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }
}
