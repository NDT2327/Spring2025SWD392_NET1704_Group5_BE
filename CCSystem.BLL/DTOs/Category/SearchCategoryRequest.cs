using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.Category
{
    public class SearchCategoryRequest
    {
        public string CategoryName { get; set; }

        public bool? IsActive { get; set; }
    }
}
