using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Bookings
{
    public class BookingResponse
    {
        public int BookingId { get; set; }

        public int CustomerId { get; set; }

        public string PromotionCode { get; set; }

        public DateTime? BookingDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string BookingStatus { get; set; }

        public string PaymentStatus { get; set; }

        public string Notes { get; set; }

        public string PaymentMethod { get; set; }
    }
}
