using CCSystem.API.Constants;
using CCSystem.Infrastructure.DTOs.BookingDetails;
using CCSystem.Infrastructure.DTOs.Bookings;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Implementations;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCSystem.API.Controllers
{
    [ApiController]
    public class BookingDetailsController : ControllerBase
    {
        private readonly IBookingDetailService _bookingDetailService;
        private readonly IValidator<BookingDetailIdRequest> _bookingDetailIdValidator;
        private readonly IValidator<BookingIdRequest> _bookingIdValidator;

        public BookingDetailsController(IBookingDetailService bookingDetailService, IValidator<BookingDetailIdRequest> bookingDetailIdValidator, IValidator<BookingIdRequest> bookingIdValidator)
        {
            this._bookingDetailService = bookingDetailService;
            this._bookingDetailIdValidator = bookingDetailIdValidator;
            this._bookingIdValidator = bookingIdValidator;
        }

        #region Get Booking Detail By Id
        /// <summary>
        /// Retrieves detailed information of a booking detail by its Id.
        /// </summary>
        /// <param name="id">The Id of the booking detail to retrieve.</param>
        /// <returns>
        /// Detailed information of the booking detail.
        /// </returns>
        /// <response code="200">Booking detail information retrieved successfully.</response>
        /// <response code="404">No booking detail found with the provided Id.</response>
        /// <response code="500">An error occurred in the system.</response>
        /// <exception cref="NotFoundException">Thrown when no booking detail exists with the provided Id.</exception>
        /// <exception cref="Exception">Throws a system exception if an error occurs.</exception>
        [ProducesResponseType(typeof(BookingDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpGet(APIEndPointConstant.BookingDetail.GetBDetailByIdEndpoint)]
        public async Task<IActionResult> GetBookingDetailByIdAsync([FromRoute] BookingDetailIdRequest bookingDetailId)
        {
            ValidationResult validationResult = await _bookingDetailIdValidator.ValidateAsync(bookingDetailId);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var bookingDetail = await _bookingDetailService.GetBookingDetailById(bookingDetailId.Id);

            return Ok(bookingDetail);
        }
        #endregion

        #region Get Booking Details By Booking Id
        /// <summary>
        /// Get detailed information from a booking Id.
        /// </summary>
        [ProducesResponseType(typeof(BookingDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpGet(APIEndPointConstant.BookingDetail.GetBDetailByBookIdEndpoint)]
        public async Task<IActionResult> GetBookingDetailByBookingId([FromRoute] BookingIdRequest request)
        {
            ValidationResult validationResult = await _bookingIdValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var listBookingDetail = await _bookingDetailService.GetBookDetailByBooking(request.Id);
            return Ok(listBookingDetail);
        }
        #endregion

        #region Get Active Booking Detail
        /// <summary>
        /// Get Active booking detail information.
        /// </summary>
        /// <returns>
        /// List of active booking details.
        /// </returns>
        /// <response code="200">Active booking details retrieved successfully.</response>
        /// <response code="404">No active booking details found.</response>
        /// <response code="500">An error occurred in the system.</response>
        [ProducesResponseType(typeof(IEnumerable<BookingDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpGet(APIEndPointConstant.BookingDetail.GetActiveBDetailEndpoint)]
        public async Task<IActionResult> GetActiveBookingDetail()
        {
            var activeBookingDetails = await _bookingDetailService.GetActiveBookingDetail();

            return Ok(activeBookingDetails);
        }
		#endregion

		/// <summary>
		/// Get list of BookingDetails by ServicelId
		/// </summary>
		[HttpGet(APIEndPointConstant.BookingDetail.GetBookingDetailByServiceId)]

        public async Task<IActionResult> GetBookingDetailsByServiceId(int id)
        {
            var bookingDetails = await _bookingDetailService.GetBookingDetailsByServiceIdAsync(id);
            return bookingDetails.Count > 0 ? Ok(bookingDetails) : NotFound($"Không tìm thấy BookingDetails với ServiceId: {id}");
        }

		/// <summary>
		/// Get list of BookingDetails by ServiceDetailId
		/// </summary>
		[HttpGet(APIEndPointConstant.BookingDetail.GetBookingDetailByServiceDetailId)]
        public async Task<IActionResult> GetBookingDetailsByServiceDetailId(int id)
        {
            var bookingDetails = await _bookingDetailService.GetBookingDetailsByServiceDetailIdAsync(id);
            return bookingDetails.Count > 0 ? Ok(bookingDetails) : NotFound($"Không tìm thấy BookingDetails với ServiceDetailId: {id}");
        }
        /// <summary>
        /// API lấy danh sách tất cả các chi tiết đặt lịch (BookingDetail).
        /// </summary>
        /// <returns>Danh sách BookingDetail dưới dạng HTTP 200 OK.</returns>
        [HttpGet(APIEndPointConstant.BookingDetail.GetAllBDetailEndpoint)]
        public async Task<IActionResult> GetAllBookingDetail()
        {
            var bookingDetails = await _bookingDetailService.GetAllAsync();
            return Ok(bookingDetails);
        }

        /// <summary>
        /// Reschedule Booking Detail appointment.
        /// </summary>
        /// <param name="id">ID của Booking Detail.</param>
        /// <param name="request">Yêu cầu đặt lại lịch.</param>
        /// <returns>Trả về thông tin đặt lại lịch.</returns>
        [HttpPost(APIEndPointConstant.BookingDetail.RescheduleBookingDetail)]
        public async Task<IActionResult> RescheduleBookingDetail(int id, [FromBody] RescheduleRequest request)
        {
            var response = await _bookingDetailService.RescheduleBookingDetail(id, request);
            return Ok(response);
        }

		/// <summary>
		/// Confirm rescheduling.
		/// </summary>
		/// <param name="id">ID của Booking Detail.</param>
		/// <param name="request">Yêu cầu xác nhận đặt lại lịch.</param>
		/// <returns>Trả về trạng thái xác nhận.</returns>
		[HttpPost(APIEndPointConstant.BookingDetail.ConfirmReschedule)]
        public async Task<IActionResult> ConfirmReschedule(int id, [FromBody] ConfirmRescheduleRequest request)
        {
            var response = await _bookingDetailService.ConfirmReschedule(id, request);
            return Ok(response);
        }
    }


}


