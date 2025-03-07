using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.DTOs.Bookings;
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
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingsService;
        private readonly IValidator<PostBookingRequest> _postBookingRequestValidator;
        private readonly IValidator<BookingIdRequest> _bookingsIdValidator;
        private readonly IValidator<AccountIdRequest> _accountIdValidator;

        public BookingsController(IBookingService bookingsService, IValidator<PostBookingRequest> postBookingRequestValidator, IValidator<BookingIdRequest> bookingsIdRequestValidator, IValidator<AccountIdRequest> accountIdValidator)
        {
            this._bookingsService = bookingsService;
            this._postBookingRequestValidator = postBookingRequestValidator;
            this._bookingsIdValidator = bookingsIdRequestValidator;
            this._accountIdValidator = accountIdValidator;
        }

        #region Create Booking With Details
        /// <summary>
        /// Create a booking along with its booking details.
        /// </summary>
        /// <param name="postBookingRequest">An object containing booking information and its associated details.</param>
        /// <returns>A status response indicating the creation status of the booking.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "customerId": 123,
        ///         "promotionCode": "PROMO2021",
        ///         "notes": "Booking note",
        ///         "paymentMethod": "CreditCard",
        ///         "address": "123 Street",
        ///         "bookingDetails": [
        ///             {
        ///                 "serviceId": 1,
        ///                 "quantity": 2,
        ///                 "scheduleDate": "2025-03-02",
        ///                 "scheduleTime": "14:00:00",
        ///                 "serviceDetailId": 10
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <response code="200">Booking created successfully.</response>
        /// <response code="400">Bad request due to invalid data.</response>
        /// <response code="404">Not found error if customer or booking detail not found.</response>
        /// <response code="500">Internal server error.</response>

        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        //[Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.Booking.BookingEndpoint)]
        public async Task<IActionResult> CreateBookingWithDetailsAsync([FromBody] PostBookingRequest postBookingRequest)
        {
            ValidationResult validationResult = await this._postBookingRequestValidator.ValidateAsync(postBookingRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var response = await _bookingsService.CreateBookingWithDetailsAsync(postBookingRequest);
            return Ok(new { Message = "Booking created successfully", Data = response.BookingId });
        }
        #endregion

        #region Get Booking By Id

        /// <summary>
        /// Retrieves detailed information of a booking by its Id.
        /// </summary>
        /// <param name="id">The Id of the booking to retrieve.</param>
        /// <returns>
        /// Detailed information of the booking.
        /// </returns>
        /// <response code="200">Booking information retrieved successfully.</response>
        /// <response code="404">No booking found with the provided Id.</response>
        /// <response code="500">An error occurred in the system.</response>
        /// <exception cref="NotFoundException">Thrown when no booking exists with the provided Id.</exception>
        /// <exception cref="Exception">Throws a system exception if an error occurs.</exception>
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpGet(APIEndPointConstant.Booking.GetBookingById)]
        public async Task<IActionResult> GetBookingByIdAsync([FromRoute] BookingIdRequest bookingId)
        {
            ValidationResult validationResult = await _bookingsIdValidator.ValidateAsync(bookingId);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var booking = await _bookingsService.GetBookingById(bookingId.Id);
            return Ok(booking);

        }

        #endregion

        #region Get Bookings by Customer Id
        /// <summary>
        /// Get Bookings by Customer Id
        /// </summary>
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpGet(APIEndPointConstant.Booking.GetBookingByCusId)]

        public async Task<IActionResult> GetBookingsByCustomer([FromRoute] AccountIdRequest cusId)
        {
            ValidationResult validationResult = await _accountIdValidator.ValidateAsync(cusId);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var reponses = await _bookingsService.GetBookingsByCustomer(cusId.Id);
            return Ok(reponses);
        }

        #endregion

    }
}
