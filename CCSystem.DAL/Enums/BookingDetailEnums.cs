using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Enums
{
    public class BookingDetailEnums
    {
        public enum BookingDetailStatus
        {
            PENDING,
            CANCELLED,
            COMPLETED,
            ASSIGNED,
            WAITINGCONFIRM,
            CANCELREQUESTED
        }
    }
}
