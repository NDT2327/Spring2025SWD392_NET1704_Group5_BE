namespace CCSystem.Infrastructure.DTOs.ServiceDetails
{
    public class PostServiceDetailResponse
    {
        public int ServiceDetailId { get; set; }
        public int ServiceId { get; set; }
        public string OptionName { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
    }
}
