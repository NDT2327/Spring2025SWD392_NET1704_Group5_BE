﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.BookingDetails
{
    public class BookingDetailIdRequest
    {
        [FromRoute(Name ="id")]
        public int Id { get; set; }
    }
}
