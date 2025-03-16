﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.ScheduleAssign
{
	public class CancelAssignmentRequest
	{
		public int AssignmentId { get; set; }
		public int HousekeeperId { get; set; }
		public string Reason { get; set; }
	}
}
