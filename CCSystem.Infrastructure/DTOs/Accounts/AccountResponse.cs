﻿using CCSystem.Infrastructure.DTOs.AccountTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.Accounts
{
    public class AccountResponse
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public AccountTokenResponse Tokens { get; set; }
    }
}
