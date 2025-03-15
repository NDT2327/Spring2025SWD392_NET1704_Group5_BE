using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.ScheduleAssign
{
    public class ScheduleAssignmentResponse
    {
        public int AssignmentId { get; set; }
        public int HousekeeperId { get; set; }
        public int DetailId { get; set; }
        public DateOnly AssignDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
