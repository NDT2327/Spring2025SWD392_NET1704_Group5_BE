using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Payments
{
    public class QueryTransactionRequest
    {
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
    }
}
