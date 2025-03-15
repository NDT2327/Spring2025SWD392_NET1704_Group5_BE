using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.BookingDetails
{
    public class RescheduleRequest
    {
        public DateOnly NewDate { get; set; }
        public TimeOnly NewTime { get; set; }
    }
}
