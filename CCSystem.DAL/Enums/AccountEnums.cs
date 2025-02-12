using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Enums
{
    public class AccountEnums
    {
        public enum Role
        {
            ADMIN,
            CUSTOMER,
            STAFF,
            HOUSEKEEPER
        }

        public enum Status
        {
            ACTIVE,
            INACTIVE,
            SUSPENDED
        }
    }
}
