using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using FluentValidation;

namespace CCSystem.Infrastructure.DTOs.Review
{
    public class ReviewRequest
    {
        
        public int CustomerId { get; set; }
        public int DetailId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

   
}