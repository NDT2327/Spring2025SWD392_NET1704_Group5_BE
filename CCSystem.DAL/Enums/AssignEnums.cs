using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Enums
{
    public class AssignEnums
    {
        public enum Status
        {
            ASSIGNED,
            INPROGRESS,
            COMPLETED,
            CANCELLED,
            WAITINGCONFIRM
        }
    }
}
