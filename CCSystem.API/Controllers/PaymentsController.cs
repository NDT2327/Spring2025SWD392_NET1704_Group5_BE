using Azure.Core;
using CCSystem.API.Constants;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Payments;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Implementations;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace CCSystem.API.Controllers
{

    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        //private readonly VnpayPayment _vnpayPayment;
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;
        private IBookingService _bookingService;


        public PaymentController(IPaymentService paymentService, IVnpay vnPayservice, IConfiguration configuration, IBookingService bookingService)
        {
            this._paymentService = paymentService;
            this._vnpay = vnPayservice;
            this._configuration = configuration;

            this._vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
            this._bookingService = bookingService;
        }

        #region Create Payment Url
        /// <summary>
        /// Tạo url thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <returns></returns>
        [HttpPost(APIEndPointConstant.Payment.PaymentEndpoint)]
        public async Task<ActionResult<string>> CreatePaymentUrl([FromBody] CreatePaymentRequest request)
        {
            try
            {
                // Lấy Booking từ DB
                var booking = await _bookingService.GetBooking(request.BookingId);
                if (booking == null)
                    return NotFound("Booking không tồn tại.");
                if (booking.PaymentStatus == "PAID")
                {
                    throw new BadRequestException(MessageConstant.BookingMessage.BookingIsPaid);
                }

                // Tạo Payment mới liên kết với Booking
                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    CustomerId = booking.CustomerId,
                    Amount = (decimal)booking.TotalAmount,
                    PaymentMethod = "VNPay",
                    Status = "PENDING",
                    CreatedDate = DateTime.UtcNow,
                    TransactionId = Guid.NewGuid().ToString(),
                };
                

                // Tạo request để lấy URL thanh toán
                var paymentRequest = new PaymentRequest
                {
                    PaymentId = payment.PaymentId, // ID của payment trong DB
                    Money = (double)booking.TotalAmount,
                    Description = request.Description,
                    IpAddress = NetworkHelper.GetIpAddress(HttpContext),
                    BankCode = BankCode.ANY,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                var paymentUrl = _vnpay.GetPaymentUrl(paymentRequest);
                await _paymentService.CreatePaymentAsync(payment);
                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Ipn Action
        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        /// </summary>
        /// <returns></returns>
        //[HttpGet("IpnAction")]
        [HttpGet(APIEndPointConstant.Payment.PaymentIpnAction)]
        public async Task<IActionResult> IpnAction()
        {
            if (!Request.QueryString.HasValue)
                return NotFound("Không tìm thấy thông tin thanh toán.");

            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);

                // Tìm Payment theo PaymentId (gửi khi tạo URL thanh toán)
                var payment = await _paymentService.GetPaymentByIdAsync((int)paymentResult.PaymentId);
                if (payment == null)
                    return NotFound("Không tìm thấy giao dịch thanh toán.");

                // Cập nhật TransactionId từ VNPay
                payment.TransactionId = paymentResult.VnpayTransactionId.ToString();

                if (paymentResult.IsSuccess)
                {
                    // Cập nhật trạng thái thanh toán
                    payment.Status = "SUCCESS";
                    payment.PaymentDate = DateTime.UtcNow;

                    // Cập nhật trạng thái Booking
                    var booking = await _bookingService.GetBooking(payment.BookingId);
                    if (booking != null)
                    {
                        booking.PaymentStatus = "PAID";
                        booking.BookingStatus = "CONFIRMED";
                        await _bookingService.UpdateBookingAsync(booking);
                    }

                    await _paymentService.UpdatePaymentAsync(payment);
                    return Ok("Thanh toán thành công.");
                }
                else
                {
                    // Nếu thanh toán thất bại
                    payment.Status = "FAILED";
                    await _paymentService.UpdatePaymentAsync(payment);
                    return BadRequest("Thanh toán thất bại.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Payment Callback Vnpay
        /// <summary>
        /// Trả kết quả thanh toán về cho người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet(APIEndPointConstant.Payment.PaymentCallbackVnpay)]
        public async Task<ActionResult<PaymentResult>> Callback()
        {
            if (!Request.QueryString.HasValue)
            {
                return NotFound("Không tìm thấy thông tin thanh toán.");
            }

            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                Console.WriteLine($"Callback Received: {JsonConvert.SerializeObject(paymentResult)}");

                var payment = await _paymentService.GetPaymentByIdAsync((int)paymentResult.PaymentId);
                if (payment == null)
                {
                    Console.WriteLine("Không tìm thấy thanh toán.");
                    return NotFound("Không tìm thấy thông tin thanh toán.");
                }

                // Cập nhật thông tin Payment
                payment.TransactionId = paymentResult.VnpayTransactionId.ToString();
                payment.Status = paymentResult.IsSuccess ? "SUCCESS" : "FAILED";
                payment.PaymentMethod = paymentResult.PaymentMethod;
                payment.UpdatedDate = DateTime.UtcNow;

                var booking = await _bookingService.GetBooking(payment.BookingId);
                if (booking != null)
                {
                    booking.PaymentStatus = "PAID";
                    booking.BookingStatus = "CONFIRMED";
                    await _bookingService.UpdateBookingAsync(booking);
                }

                await _paymentService.UpdatePaymentAsync(payment);
                Console.WriteLine("Cập nhật thanh toán thành công!");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Callback: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }

}
