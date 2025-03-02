using Microsoft.AspNetCore.Mvc;
using CCSystem.BLL.DTOs.Review;
using CCSystem.BLL.Services;
using CCSystem.API.Constants;
using FluentValidation;
using FluentValidation.Results;
using AutoMapper;

namespace CCSystem.API.Controllers
{
    [ApiController]
    [Route(APIEndPointConstant.Review.ReviewEndpoint)] 
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IValidator<ReviewRequest> _validator;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService reviewService, IValidator<ReviewRequest> validator, IMapper mapper)
        {
            _reviewService = reviewService;
            _validator = validator;
            _mapper = mapper;
        }

        [HttpGet(APIEndPointConstant.Review.GetAllReviewsEndpoint)] 
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        // GET: /api/v1/reviews/{id}
        [HttpGet(APIEndPointConstant.Review.GetReviewByIdEndpoint)] 
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound($"Review with ID {id} not found.");
            }
            return Ok(review);
        }

        // POST: /api/v1/reviews/createreview
        [HttpPost(APIEndPointConstant.Review.CreateReviewEndpoint)] 
        public async Task<IActionResult> CreateReview([FromBody] ReviewRequest reviewRequest)
        {
            if (reviewRequest == null)
            {
                return BadRequest("ReviewRequest cannot be null.");
            }

            ValidationResult validationResult = await _validator.ValidateAsync(reviewRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var reviewResponse = await _reviewService.AddReviewAsync(reviewRequest);
            return CreatedAtAction(nameof(GetReviewById), new { id = reviewResponse.ReviewId }, reviewResponse);
        }

        [HttpPut(APIEndPointConstant.Review.UpdateReviewEndpoint)] 
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewRequest reviewRequest)
        {
            var existingReview = await _reviewService.GetReviewByIdAsync(id);
            if (existingReview == null)
            {
                return NotFound($"Review with ID {id} not found.");
            }

            // Gọi service để cập nhật review
            await _reviewService.UpdateReviewAsync(reviewRequest, id);

            return Ok($"Review with ID {id} has been updated successfully.");
        }

        // DELETE: /api/v1/reviews/deletereview/{id}
        [HttpDelete(APIEndPointConstant.Review.DeleteReviewEndpoint)] 
        public async Task<IActionResult> DeleteReview(int id)
        {
            var existingReview = await _reviewService.GetReviewByIdAsync(id);
            if (existingReview == null)
            {
                return NotFound($"Review with ID {id} not found.");
            }

            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
    }
}
