﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Presentation.Models.ScheduleAssign
{
    public class AssignIdRequest
    {
        [FromRoute(Name = "id")]
        public int Id { get; set; }
    }
}
