using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.ScheduleAssign
{
    public class PostScheduleAssignRequest
    {
        public int HousekeeperId { get; set; }

        public int DetailId { get; set; }

        public string Notes { get; set; }
    }
}
