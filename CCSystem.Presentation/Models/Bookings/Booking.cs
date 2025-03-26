using CCSystem.Presentation.Models.Profiles;

namespace CCSystem.Presentation.Models.Bookings
{
    public class Booking
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

        public string Address { get; set; }
        public CustomerProfile CustomerProfile { get; set; }

    }
}
