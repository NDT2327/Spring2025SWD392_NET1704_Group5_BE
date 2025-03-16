using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.BookingDetails
{
    public class ConfirmRescheduleResponse
    {
        public int DetailId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
