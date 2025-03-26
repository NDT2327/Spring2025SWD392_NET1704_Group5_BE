namespace CCSystem.Presentation.Models.Payment
{
    public class VnpayCallbackResponse
    {
        public int PaymentId { get; set; }
        public bool IsSuccess { get; set; }
        public string Description { get; set; }
        public string Timestamp { get; set; }
        public long VnpayTransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentResponseDetail PaymentResponse { get; set; }
        public TransactionStatusDetail TransactionStatus { get; set; }
        public BankingInfo BankingInfor { get; set; }
    }

    public class PaymentResponseDetail
    {
        public int Code { get; set; }
        public string Description { get; set; }
    }

    public class TransactionStatusDetail
    {
        public int Code { get; set; }
        public string Description { get; set; }
    }

    public class BankingInfo
    {
        public string BankCode { get; set; }
        public string BankTransactionId { get; set; }
    }
}
