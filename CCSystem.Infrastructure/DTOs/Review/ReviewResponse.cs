using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCSystem.DAL.Models;
namespace CCSystem.Infrastructure.DTOs.Review
{

    public class ReviewResponse
    {
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public int DetailId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
    }
}