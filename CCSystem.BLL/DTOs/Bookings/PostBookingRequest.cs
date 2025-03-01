using CCSystem.BLL.DTOs.BookingDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Bookings
{
    public class PostBookingRequest
    {
        public int CustomerId { get; set; }
        public string? PromotionCode { get; set; } = null;
        //public DateTime? BookingDate { get; set; }
        //public string BookingStatus { get; set; }
        //public string PaymentStatus { get; set; }
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }
        public string Address { get; set; }
        public List<PostBookingDetailRequest> BookingDetails { get; set; } = new List<PostBookingDetailRequest>();
    }
}
