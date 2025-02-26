using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Accounts
{
    public class UpdateAccountRequest
    {
        //public int? AccountId { get; set; }
        //public string Email { get; set; }

        //public string Password { get; set; }

        //public string Role { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public double? Rating { get; set; }

        public int? Experience { get; set; }

        public string FullName { get; set; }

        public string Status { get; set; }

    }
}
