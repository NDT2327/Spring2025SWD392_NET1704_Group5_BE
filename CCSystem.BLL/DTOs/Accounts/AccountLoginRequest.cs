using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Accounts
{
    public class AccountLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
