using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Promotions;
using CCSystem.BLL.Exceptions;
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

            // Convert Promotion entities to GetPromotionResponse DTOs
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

            // Map Promotion entity to GetPromotionResponse DTO
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

                // Check if a promotion with the given code already exists
                existingPromotion = await GetPromotionByCodeAsync(request.Code); 
            }catch
            {
                existingPromotion = null;
            }

            if (existingPromotion != null)
            {
                throw new BadRequestException(MessageConstant.PromotionMessage.AlreadyExistPromotionCode);
            }
            try
            {
                // Map request data to a new Promotion entity
                var promotion = _mapper.Map<Promotion>(request); // This will now work
                promotion.CreatedDate = DateTime.UtcNow; // Ensure CreatedDate is set

                // Save the new promotion to the database
                await _unitOfWork.PromotionRepository.AddPromotionAsync(promotion);
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception(MessageConstant.PromotionMessage.FailedToCreatePromotion + " " + ex.Message);
            }
        }

        public async Task<bool> UpdatePromotionAsync(string code, PutPromotionRequest request)
        {
            // Retrieve the existing promotion by code
            var existingPromotion = await _unitOfWork.PromotionRepository.GetPromotionByCodeAsync(code);
            if (existingPromotion == null)
            {
                throw new NotFoundException(MessageConstant.PromotionMessage.NotExistPromotion);
            }
            // Update the existing promotion using AutoMapper
            _mapper.Map(request, existingPromotion);

            // Save changes to the database
            return await _unitOfWork.PromotionRepository.UpdatePromotionAsync(existingPromotion);
        }

        public async Task<bool> DeletePromotionAsync(string code)
        {
            // Retrieve the promotion by code
            var promotion = await _unitOfWork.PromotionRepository.GetPromotionByCodeAsync(code);

            if (promotion == null)
            {
                return false; // Return false if the promotion does not exist
            }

            // Delete the promotion from the database
            await _unitOfWork.PromotionRepository.DeletePromotionAsync(promotion);
            return true;
        }   
    }
}
