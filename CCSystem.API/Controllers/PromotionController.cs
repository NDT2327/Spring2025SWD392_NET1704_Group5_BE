using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Services;
using CCSystem.BLL.DTOs.Promotions;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using CCSystem.DAL.Repositories;
using CCSystem.API.Validators.Promotions;

namespace CCSystem.API.Controllers
{
    public class PromotionController : ControllerBase
    {
        #region Init controller
        private readonly IPromotionService _promotionService;
        private readonly IValidator<PostPromotionRequest> _postPromotionValidator;
        private readonly IValidator<PutPromotionRequest> _putPromotionValidator;

        /// <summary>
        /// Controller for managing promotions.
        /// </summary>
        /// 
        public PromotionController(IPromotionService promotionService, IValidator<PostPromotionRequest> postPromotionValidator, IValidator<PutPromotionRequest> putPromotionValidator)
        {
            this._promotionService = promotionService;
            _postPromotionValidator = postPromotionValidator;
            _putPromotionValidator = putPromotionValidator;
        }

        #endregion

        #region Get all promotions
        /// <summary>
        /// Get all promotions.
        /// </summary>
        /// 
        [HttpGet(APIEndPointConstant.Promotions.PromotionEndPoint)]
        public async Task<IActionResult> GetAllPromotions()
        {
            var promotion = await _promotionService.GetAllPromotionsAsync();
            return Ok(promotion);
        }

        #endregion

        #region Get promotion by code
        /// <summary>
        /// Get a promotion by code.
        /// </summary>
        /// 
        [HttpGet(APIEndPointConstant.Promotions.PromotionByCodeEndPoint)]
        public async Task<IActionResult> GetPromotionByCode(string code)
        {
            var promotion = await _promotionService.GetPromotionByCodeAsync(code);
            if (promotion == null)
            {
                return NotFound(new { message = "Promotion not found." });
            }
            return Ok(promotion);
        }
        #endregion

        #region Create Promotion
        /// <summary>
        /// Create a promotion.
        /// </summary>  
        /// 
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpPost(APIEndPointConstant.Promotions.CreatePromotionEndPoint)]
        public async Task<IActionResult> CreatePromotion([FromBody] PostPromotionRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body cannot be null" });
            }

            var validationResult = _postPromotionValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var promotion = await _promotionService.CreatePromotionAsync(request);
                return Ok(promotion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        #endregion

    }
}
