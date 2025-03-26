using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CCSystem.Presentation.Pages.Payments
{
    public class PaymentCallbackVnpayModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public PaymentCallbackVnpayModel(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }

        public string TransactionStatus { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string Message { get; set; }
        public string TransactionNo { get; set; }
        public string PayDate { get; set; }

        public async Task OnGetAsync()
        {
            //parse query string from VNPAY
            var queryParams = new PaymentQueryResult
            {
                ResponseCode = Request.Query["vnp_ResponseCode"].FirstOrDefault(),
                TransactionStatus = Request.Query["vnp_TransactionStatus"].FirstOrDefault(),
                TransactionNo = Request.Query["vnp_TransactionNo"].FirstOrDefault(),
                TransactionId = Request.Query["vnp_TxnRef"].FirstOrDefault(),
                Amount = long.TryParse(Request.Query["vnp_Amount"].FirstOrDefault(), out long amount) ? amount : 0,
                OrderInfo = Request.Query["vnp_OrderInfo"].FirstOrDefault(),
                PayDate = Request.Query["vnp_PayDate"].FirstOrDefault()
            };

            OrderId = queryParams.TransactionId;
            TransactionNo = queryParams.TransactionNo;
            Amount = (queryParams.Amount / 100).ToString("C", new System.Globalization.CultureInfo("vi-VN"));
            if (DateTime.TryParseExact(queryParams.PayDate, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var payDate))
            {
                PayDate = payDate.ToString("dd/MM/yyyy HH:mm:ss");
            }
            else
            {
                PayDate = queryParams.PayDate;
            }

            Console.WriteLine($"Query String - ResponseCode: {queryParams.ResponseCode}, TransactionStatus: {queryParams.TransactionStatus}, TransactionNo: {queryParams.TransactionNo}");
            // Gọi API CallBackVnPay
            var callbackUrl = $"{_apiEndpoints.GetFullUrl(_apiEndpoints.Payment.CallBackVnPay)}?{Request.QueryString}";
            var callbackResult = await _httpClient.GetAsync(callbackUrl);

            if (callbackResult.IsSuccessStatusCode)
            {
                var callbackResponseContent = await callbackResult.Content.ReadAsStringAsync();
                Console.WriteLine($"Callback Response: {callbackResponseContent}");
                PaymentResponse callbackResponse;
                try
                {
                    callbackResponse = JsonSerializer.Deserialize<PaymentResponse>(callbackResponseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Deserialize Error: {ex.Message}");
                    TransactionStatus = "Failed";
                    Message = $"Lỗi phân tích response từ API: {ex.Message}";
                    ToastHelper.ShowError(TempData, Message);
                    return;
                }

                if (callbackResponse != null)
                {
                    if(queryParams.ResponseCode == "00" && queryParams.TransactionStatus == "00")
                    {
                        TransactionStatus = "Success";
                        Message = "Payment Successfully";
                        Console.WriteLine("Overriding API status with VNPay query string: Success");
                    }
                    else
                    {
                        TransactionStatus = callbackResponse.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase) ? "Success" : "Failed";
                        Message = TransactionStatus == "Success" ? "Payment Successfully" : "Payment failure";
                    }
                }
                else
                {
                    TransactionStatus = "Failed";
                    Message = "Unable to confirm payment status from server.";
                }
            }
            else
            {
                // Fallback nếu API thất bại
                TransactionStatus = queryParams.ResponseCode == "00" && queryParams.TransactionStatus == "00" ? "Success" : "Failed";
                Message = queryParams.ResponseCode switch
                {
                    "00" => "Payment Successfully!",
                    "07" => "Suspicious.",
                    "09" => "Account does not have enough balance.",
                    "24" => "Transaction is cancelled by user.",
                    _ => $"(Error: {queryParams.ResponseCode})."
                };
            }

            // Hiển thị thông báo
            if (TransactionStatus == "Success")
            {
                ToastHelper.ShowSuccess(TempData, Message);
            }
            else
            {
                ToastHelper.ShowError(TempData, Message);
            }
        }
    }
}
