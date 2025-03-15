using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Infrastructure.DTOs.Accounts
{
    public class AccountsListRequest
    {
        //sort: email, email_desc, fullname, fullname_desc, createddate, createddate_desc, editeddate, editeddate_desc
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchByName { get; set; }
        public string Sort { get; set; }
    }
}
