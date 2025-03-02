using AutoMapper;
using CCSystem.BLL.DTOs.Promotions;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetPromotionResponse>> GetAllPromotionsAsync()
        {
            var promotions = await _unitOfWork.PromotionRepository.GetListPromotionsAsync();
            var response = promotions.ConvertAll(p => new GetPromotionResponse
            {
                Code = p.Code,
                DiscountAmount = p.DiscountAmount,
                DiscountPercent = p.DiscountPercent,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                MinOrderAmount = p.MinOrderAmount,
                MaxDiscountAmount = p.MaxDiscountAmount,
                CreatedDate = p.CreatedDate
            });
            return response;
        }

        public async Task<GetPromotionResponse> GetPromotionByCodeAsync(string code)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetPromotionByCodeAsync(code);
            if (promotion == null) return null;

            return new GetPromotionResponse
            {
                Code = promotion.Code,
                DiscountAmount = promotion.DiscountAmount,
                DiscountPercent = promotion.DiscountPercent,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                MinOrderAmount = promotion.MinOrderAmount,
                MaxDiscountAmount = promotion.MaxDiscountAmount,
                CreatedDate = promotion.CreatedDate
            };
        }

        public async Task<Promotion> CreatePromotionAsync(PostPromotionRequest request)
        {
            GetPromotionResponse existingPromotion;
            try { 
                existingPromotion = await GetPromotionByCodeAsync(request.Code); 
            }catch
            {
                existingPromotion = null;
            }

            if (existingPromotion != null)
            {
                throw new Exception("Promotion already exists");
            }
            try
            {
                var promotion = _mapper.Map<Promotion>(request); // This will now work
                promotion.CreatedDate = DateTime.UtcNow; // Ensure CreatedDate is set
                await _unitOfWork.PromotionRepository.AddPromotionAsync(promotion);
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating promotion.", ex);
            }
        }

        public async Task<bool> UpdatePromotionAsync(string code, PutPromotionRequest request)
        {
            var existingPromotion = await _unitOfWork.PromotionRepository.GetPromotionByCodeAsync(code);
            if (existingPromotion == null)
            {
                throw new Exception("Promotion not found.");
            }
            _mapper.Map(request, existingPromotion); // Update the existing entity with new values

            return await _unitOfWork.PromotionRepository.UpdatePromotionAsync(existingPromotion);
        }

        public async Task<bool> DeletePromotionAsync(string code)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetPromotionByCodeAsync(code);

            if (promotion == null)
            {
                return false; // Promotion not found
            }

            await _unitOfWork.PromotionRepository.DeletePromotionAsync(promotion);
            return true;
        }
    }
}
