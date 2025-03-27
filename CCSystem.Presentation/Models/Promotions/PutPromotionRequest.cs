using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.Presentation.Models.Promotions
{
    public class PutPromotionRequest
    {
        //public required string Code { get; set; } // Must Existing Promotion Code (Primary Key)
        public decimal? DiscountAmount { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
    }
}
