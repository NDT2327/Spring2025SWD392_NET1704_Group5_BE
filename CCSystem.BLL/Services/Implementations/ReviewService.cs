using AutoMapper;
using CCSystem.BLL.Constants;
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

            // Ensure safe mapping with default values for nullable fields
            return reviews.Select(r => new ReviewResponse
            {
                ReviewId = r.ReviewId,
                CustomerId = r.CustomerId,
                DetailId = r.DetailId,
                Rating = r.Rating ?? 0,// Default to 0 if null
                Comment = r.Comment,
                ReviewDate = r.ReviewDate ?? DateTime.MinValue// Default to MinValue if null
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
                // Ensure request is not null before processing
                throw new ArgumentNullException(nameof(reviewRequest), MessageConstant.ReviewMessage.EmptyReviewRequest);
                }

                var review = _mapper.Map<Review>(reviewRequest);

                try
                {
                    await _unitOfWork.ReviewRepository.AddAsync(review);
                    await _unitOfWork.CommitAsync();
                }
                catch (BadRequestException ex)
                {
                // Handle and wrap exceptions with meaningful messages
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                    throw new BadRequestException(message);
                }
                catch (NotFoundException ex)
                {
                    string message = ErrorUtil.GetErrorString("NotFoundException", ex.Message);
                    throw new NotFoundException(message);
                }
            }


        public async Task UpdateReviewAsync(ReviewRequest reviewRequest, int reviewId)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId);
            if (review != null)
            {
                // Ensure only updated fields are mapped
                _mapper.Map(reviewRequest, review);

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
