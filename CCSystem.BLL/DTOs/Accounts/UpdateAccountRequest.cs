using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Accounts
{
    public class UpdateAccountRequest
    {

        public string Address { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public IFormFile Avatar { get; set; }
        public string Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public double? Rating { get; set; }
        public int? Experience { get; set; }
        public string Status { get; set; }
    }
}
