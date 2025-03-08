using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Payments
{
    public class PutPaymentWithBooking
    {

        public string PaymentMethod { get; set; }

        public string Status { get; set; }

        public string TransactionId { get; set; }


    }
}
