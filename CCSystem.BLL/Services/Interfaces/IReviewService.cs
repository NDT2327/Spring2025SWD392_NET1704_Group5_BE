using CCSystem.BLL.DTOs.Review;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync();
        Task<ReviewResponse?> GetReviewByIdAsync(int id);
        Task<ReviewResponse> AddReviewAsync(ReviewRequest reviewRequest);
        Task UpdateReviewAsync(ReviewRequest reviewRequest, int reviewId);
        Task DeleteReviewAsync(int id);
    }
}
