using AutoMapper;
using CCSystem.BLL.DTOs.Review;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewResponse>>(reviews);
        }

        public async Task<ReviewResponse?> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id);
            return review == null ? null : _mapper.Map<ReviewResponse>(review);
        }

        public async Task<ReviewResponse> AddReviewAsync(ReviewRequest reviewRequest)
        {
            var review = _mapper.Map<Review>(reviewRequest);
            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.CommitAsync(); 
            return _mapper.Map<ReviewResponse>(review);
        }

        public async Task UpdateReviewAsync(ReviewRequest reviewRequest, int reviewId)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId);
            if (review != null)
            {
                // Ánh xạ từ ReviewRequest sang Review
                _mapper.Map(reviewRequest, review);

                // Cập nhật review trong repository
                await _unitOfWork.ReviewRepository.UpdateAsync(review);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task DeleteReviewAsync(int id)
        {
            await _unitOfWork.ReviewRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }
}
