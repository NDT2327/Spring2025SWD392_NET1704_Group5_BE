using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Enums
{
    public class PaymentEnums
    {
        public enum Status
        {
            SUCCESS,
            FAILED,
            PENDING,
            REFUNDREQUESTED,
            REFUNDED
        }
    }
}
