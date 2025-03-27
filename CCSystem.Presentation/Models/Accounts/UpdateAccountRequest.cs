using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CCSystem.Presentation.Models.Accounts
{
    public class UpdateAccountRequest
    {

        public string Address { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public IFormFile Avatar { get; set; }
        public string Gender { get; set; }
        public double? Rating { get; set; }
        public int? Experience { get; set; }
        public string Status { get; set; }


        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }

    }
}