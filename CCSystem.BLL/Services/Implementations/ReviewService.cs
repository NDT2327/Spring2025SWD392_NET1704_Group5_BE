using AutoMapper;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.DTOs.Review;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Utils;
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

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
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

        public async Task<IEnumerable<ReviewResponse>> GetReviewsByCustomerIdAsync(int customerId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetReviewsByCustomerIdAsync(customerId);

            return reviews.Select(r => new ReviewResponse
            {
                ReviewId = r.ReviewId,
                CustomerId = r.CustomerId,
                DetailId = r.DetailId,
                Rating = r.Rating ?? 0,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate ?? DateTime.MinValue
            }).ToList();
        }
        public async Task<List<ReviewResponse>> GetReviewsByDetailIdAsync(int detailId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetReviewsByDetailIdAsync(detailId);
            return _mapper.Map<List<ReviewResponse>>(reviews);
        }
        public async Task AddReviewAsync(ReviewRequest reviewRequest)
        {
            if (reviewRequest == null)
            {
                throw new ArgumentNullException(nameof(reviewRequest), "Request không được để trống.");
            }

            var review = _mapper.Map<Review>(reviewRequest);

            try
            {
                await _unitOfWork.ReviewRepository.AddAsync(review);
                await _unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                throw new BadRequestException(message);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
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
