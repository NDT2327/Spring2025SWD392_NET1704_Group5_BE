using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.DTOs.ServiceDetails
{
    public class PutServiceDetailRequest
    {
        //public int ServiceDetailId { get; set; }
        public int ServiceId { get; set; }  // Foreign key
        public string OptionName { get; set; }
        public string OptionType { get; set; }
        public decimal BasePrice { get; set; }
        public string Unit { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
