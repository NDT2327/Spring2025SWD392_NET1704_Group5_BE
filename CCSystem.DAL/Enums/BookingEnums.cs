using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Enums
{
    public class BookingEnums
    {
        public enum BookingStatus
        {
            PENDING,
            CONFIRMED,
            COMPLETED,
            CANCELED,
            CANCELREQUESTED
        }

        public enum PaymentStatus
        {
            PAID,
            PENDING,
            FAILED,
            REFUNDED
        }
    }
}
