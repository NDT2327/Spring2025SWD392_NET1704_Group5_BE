using Microsoft.AspNetCore.Mvc;
using CCSystem.BLL.DTOs.Review;
using CCSystem.BLL.Services;
using CCSystem.API.Constants;
using FluentValidation;
using FluentValidation.Results;
using AutoMapper;

namespace CCSystem.API.Controllers
{
    /// <summary>
    /// Controller quản lý đánh giá (Review).
    /// </summary>
    [ApiController]
    [Route(APIEndPointConstant.Review.ReviewEndpoint)]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IValidator<ReviewRequest> _validator;
        private readonly IMapper _mapper;
        /// <summary>
        /// Khởi tạo ReviewController với các dịch vụ liên quan.
        /// </summary>
        /// <param name="reviewService">Dịch vụ quản lý đánh giá</param>
        /// <param name="validator">Trình xác thực đánh giá</param>
        /// <param name="mapper">Trình ánh xạ dữ liệu</param>
        public ReviewController(IReviewService reviewService, IValidator<ReviewRequest> validator, IMapper mapper)
        {
            _reviewService = reviewService;
            _validator = validator;
            _mapper = mapper;
        }
        /// <summary>
        /// Lấy danh sách tất cả đánh giá.
        /// </summary>
        /// <returns>Danh sách đánh giá</returns>
        [HttpGet(APIEndPointConstant.Review.GetAllReviewsEndpoint)]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        /// <summary>
        /// Lấy đánh giá theo ID.
        /// </summary>
        /// <param name="id">ID của đánh giá</param>
        /// <returns>Thông tin đánh giá nếu tồn tại</returns>
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

        /// <summary>
        /// Tạo một đánh giá mới.
        /// </summary>
        /// <param name="reviewRequest">Thông tin đánh giá</param>
        /// <returns>Trả về NoContent nếu thành công</returns>
        [HttpPost(APIEndPointConstant.Review.CreateReviewEndpoint)]
        public async Task<IActionResult> CreateReview([FromBody] ReviewRequest reviewRequest)
        {
            if (reviewRequest == null)
            {
                return BadRequest(new { message = "ReviewRequest cannot be null." });
            }

            ValidationResult validationResult = await _validator.ValidateAsync(reviewRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed.",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            try
            {
                await _reviewService.AddReviewAsync(reviewRequest); 
                return Ok(new { message = "Review created successfully." }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the review.", error = ex.Message });
            }
        }
        /// <summary>
        /// Cập nhật đánh giá theo ID.
        /// </summary>
        /// <param name="id">ID của đánh giá</param>
        /// <param name="reviewRequest">Dữ liệu đánh giá cập nhật</param>
        /// <returns>Thông báo cập nhật thành công</returns>

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

        /// <summary>
        /// Xóa đánh giá theo ID.
        /// </summary>
        /// <param name="id">ID của đánh giá</param>
        /// <returns>Trả về NoContent nếu thành công</returns>
        [HttpDelete(APIEndPointConstant.Review.DeleteReviewEndpoint)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var existingReview = await _reviewService.GetReviewByIdAsync(id);
            if (existingReview == null)
            {
                return NotFound(new { message = $"Review with ID {id} not found." });
            }

            try
            {
                await _reviewService.DeleteReviewAsync(id); 
                return Ok(new { message = $"Review with ID {id} deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the review.", error = ex.Message });
            }
        }
    }
}