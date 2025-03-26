using Newtonsoft.Json;

namespace CCSystem.Presentation.Models.Payment
{
    public class PaymentQueryResult
    {
        [JsonProperty("vnp_ResponseCode")]
        public string ResponseCode { get; set; }

        [JsonProperty("vnp_TransactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("vnp_TransactionNo")]
        public string TransactionNo { get; set; }

        [JsonProperty("vnp_TxnRef")]
        public string TransactionId { get; set; }

        [JsonProperty("vnp_Amount")]
        public long Amount { get; set; }

        [JsonProperty("vnp_OrderInfo")]
        public string OrderInfo { get; set; }

        [JsonProperty("vnp_PayDate")]
        public string PayDate { get; set; }
    }
}
