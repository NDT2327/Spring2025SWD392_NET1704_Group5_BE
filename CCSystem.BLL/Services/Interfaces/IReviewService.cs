using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.DTOs.Review;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync();
        Task<ReviewResponse?> GetReviewByIdAsync(int id);
        Task AddReviewAsync(ReviewRequest reviewRequest);
        Task UpdateReviewAsync(ReviewRequest reviewRequest, int reviewId);
        Task DeleteReviewAsync(int id);
        Task<IEnumerable<ReviewResponse>> GetReviewsByCustomerIdAsync(int customerId);
        public Task<List<ReviewResponse>> GetReviewsByDetailIdAsync(int detailId);
    }
}
