using System;

namespace CCSystem.BLL.DTOs.Promotions
{
    public class GetPromotionResponse
    {
        public string Code { get; set; }
        public decimal? DiscountAmount { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
