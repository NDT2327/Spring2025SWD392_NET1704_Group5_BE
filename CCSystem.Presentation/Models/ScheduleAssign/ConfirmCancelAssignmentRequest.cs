using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Presentation.Models.ScheduleAssign
{
	public class ConfirmCancelAssignmentRequest
	{
		public int AssignmentId { get; set; }
		public bool IsApproved { get; set; } // true: Hủy, false: Từ chối hủy
	}
}
