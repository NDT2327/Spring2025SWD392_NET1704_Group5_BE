﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Presentation.Models.Accounts
{
    public class OTPCodeVerificationRequest
    {
        public string Email { get; set; }
        public string OTPCode { get; set; }
    }
}
