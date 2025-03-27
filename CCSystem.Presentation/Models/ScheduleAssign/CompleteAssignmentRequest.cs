using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Presentation.Models.ScheduleAssign
{
    public class CompleteAssignmentRequest
    {
        public int AssignmentId { get; set; }
        public int HousekeeperId { get; set; }
    }
}
