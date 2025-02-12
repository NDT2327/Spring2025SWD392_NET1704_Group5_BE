using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Accounts
{
    public class GetAccountResponse
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }

    }
}
