using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Services
{
    public class PostServiceRequest
    {
        public int CategoryId { get; set; }

        public string ServiceName { get; set; }

        public string Description { get; set; }

        public IFormFile Image { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public bool? IsActive { get; set; }

    }
}
