namespace CCSystem.Presentation.Models.Payment
{
    public class PaymentResponse
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string PaymentDate { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Notes { get; set; }
        public string TransactionId { get; set; }
    }
}
