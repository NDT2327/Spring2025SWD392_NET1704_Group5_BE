using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.BookingDetails
{
    public class PostBookingDetailRequest
    {
        public int BookingId { get; set; }

        public int ServiceId { get; set; }

        public DateOnly ScheduleDate { get; set; }

        public int Quantity { get; set; }

        //public decimal UnitPrice { get; set; }

        public TimeOnly ScheduleTime { get; set; }

        public int ServiceDetailId { get; set; }
    }
}
