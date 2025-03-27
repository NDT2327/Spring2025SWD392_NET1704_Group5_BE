using CCSystem.Presentation.Models.Profiles;

namespace CCSystem.Presentation.Models.BookingDetails
{
    public class BookingDetail
    {
        public int DetailId { get; set; }

        public int BookingId { get; set; }

        public int ServiceId { get; set; }

        public string ServiceName { get; set; }

        public int? ServiceDetailId { get; set; }

        public string ServiceDetailName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public DateOnly ScheduleDate { get; set; }

        public TimeOnly ScheduleTime { get; set; }

        public bool? IsAssign { get; set; }

        public string BookdetailStatus { get; set; }

    }
}
