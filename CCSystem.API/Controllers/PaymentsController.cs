using Azure.Core;
using CCSystem.API.Constants;
using CCSystem.BLL.Constants;
using CCSystem.Infrastructure.DTOs.Payments;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Implementations;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Models;
using FluentValidation;
using FluentValidation.Results;
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
        private IValidator<PutPaymentWithBooking> _putPaymentWithBookingValidator;

        public PaymentController(IPaymentService paymentService, IVnpay vnPayservice, IConfiguration configuration, IBookingService bookingService, IValidator<PutPaymentWithBooking> putPaymentWithBookingValidator)
        {
            this._paymentService = paymentService;
            this._vnpay = vnPayservice;
            this._configuration = configuration;

            this._vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
            this._bookingService = bookingService;
            this._putPaymentWithBookingValidator = putPaymentWithBookingValidator;
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
                await _paymentService.CreatePaymentAsync(payment);


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
                payment.CreatedDate = paymentRequest.CreatedDate;
                await _paymentService.UpdatePaymentAsync(payment);
                var paymentUrl = _vnpay.GetPaymentUrl(paymentRequest);
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

        /// <summary>
        /// Lấy danh sách thanh toán theo ID khách hàng.
        /// </summary>
        /// <param name="id">ID của khách hàng</param>
        /// <returns>Danh sách các khoản thanh toán</returns>
        [HttpGet(APIEndPointConstant.Payment.GetPaymentByCustomerId)]
        public async Task<IActionResult> GetPaymentsByCustomerId([FromRoute] int id)
        {
            var payments = await _paymentService.GetPaymentsByCustomerIdAsync(id);
            return payments.Any() ? Ok(payments) : NotFound("No payments found for this customer.");
        }

        /// <summary>
        /// Lấy thông tin thanh toán theo BookingId.
        /// </summary>
        /// <param name="id">ID của đơn đặt chỗ (Booking)</param>
        /// <returns>Thông tin thanh toán</returns>
        [HttpGet(APIEndPointConstant.Payment.GetPaymentByBookingId)]
        public async Task<IActionResult> GetByBookingId([FromRoute] int id) 
        {
            var paymentDto = await _paymentService.GetByBookingIdAsync(id);

            if (paymentDto == null)
                return NotFound($"Không tìm thấy Payment với BookingId: {id}");

            return Ok(paymentDto);
        }

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

        #region Update Payment With Booking
        /// <summary>
        /// Update payment details and synchronize with the corresponding booking.
        /// </summary>
        /// <param name="paymentId">The ID of the payment to update.</param>
        /// <param name="request">The payment update request.</param>
        /// <returns>Updated payment and booking details.</returns>
        /// <response code="200">Payment updated successfully.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="404">Payment or booking not found.</response>
        /// <response code="500">An internal server error occurred.</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPut(APIEndPointConstant.Payment.PaymentEndpoint)]
        public async Task<IActionResult> UpdatePaymentWithBooking(int paymentId, [FromBody] PutPaymentWithBooking request)
        {
            ValidationResult validationResult = await _putPaymentWithBookingValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            await _paymentService.UpdatePaymentWithBooking(paymentId, request);

            return Ok(new { Message = "Payment updated successfully." });
        }
        #endregion

    }
}