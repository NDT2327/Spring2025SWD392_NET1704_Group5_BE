using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Bookings;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Services.Implementations;
using CCSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCSystem.API.Controllers
{
    
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingsService;

        public BookingsController(IBookingService bookingsService)
        {
            this._bookingsService = bookingsService;
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
            await _bookingsService.CreateBookingWithDetailsAsync(postBookingRequest);
            return Ok(new { Message = "Booking created successfully" });
        }
        #endregion
    }
}
