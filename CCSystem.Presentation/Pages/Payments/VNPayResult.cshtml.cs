using CCSystem.Infrastructure.DTOs.Payments;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace CCSystem.Presentation.Pages.Payments
{
    public class VNPayResultModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public VNPayResultModel(HttpClient httpClient, ApiEndpoints apiEndpoints)
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
                PayDate = Request.Query["vnp_PayDate"].FirstOrDefault(),

            };

            OrderId = queryParams.TransactionId;

            //call api CallBackVnPay
            var callbackUrl = $"{_apiEndpoints.GetFullUrl(_apiEndpoints.Payment.CallBackVnPay)}?{Request.QueryString}";
            var callbackResult = await _httpClient.GetAsync(callbackUrl);

            //if success
            if (callbackResult.IsSuccessStatusCode)
            {
                var response = await callbackResult.Content.ReadAsStringAsync();
                var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(response);

                //map data from paymentresponse
                TransactionStatus = paymentResponse.Status switch
                {
                    "Success" => "Success",
                    "Pending" => "Pending",
                    _ => "Failed"
                };

                Message = paymentResponse.Status == "Success" ? "Payment Successfully!" : paymentResponse.Status == "Pending" ? "In Processing..." : "Paymend failed.";
                Amount = (paymentResponse.Amount).ToString("C", new System.Globalization.CultureInfo("vi-VN"));
                TransactionNo = paymentResponse.TransactionId;
                PayDate = paymentResponse.PaymentDate?.ToString("dd/MM/yyyy HH:mm:ss") ?? queryParams.PayDate;
            }
            else
            {
                //fallback if api failed
                TransactionStatus = queryParams.ResponseCode == "00" ? "Success" : "Fail";
                Message = queryParams.ResponseCode switch
                {
                    "00" => "Payment Successfully!",
                    "07" => "Suspond",
                    "09" => "Your account do not have enough balance.",
                    "24" => "Transaction is canceled by user.",
                    _ => $"Transaction failed (Error Code:{queryParams.ResponseCode}). Please check again."
                };
                Amount = (queryParams.Amount / 100).ToString("C", new System.Globalization.CultureInfo("vi-VN"));
                TransactionNo = queryParams.TransactionNo;
                PayDate = queryParams.PayDate;
            }

            if(TransactionStatus == "Success")
            {
                ToastHelper.ShowSuccess(TempData, Message);
            }else if(TransactionStatus == "Pending")
            {
                ToastHelper.ShowWarning(TempData, Message);
            }
            else
            {
                ToastHelper.ShowError(TempData, Message);
            }
        }
    }
}
