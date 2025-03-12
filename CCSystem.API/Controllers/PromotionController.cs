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
using Microsoft.AspNetCore.Components;
using CCSystem.BLL.Services.Implementations;
using static CCSystem.BLL.Constants.MessageConstant;

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
                return NotFound(new { message = PromotionMessage.NotExistPromotion });
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
                return BadRequest(new { message = PromotionMessage.InvalidRequest });
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

        #region Update Promotion
        /// <summary>
        /// Update a promotion.
        /// </summary>
        /// 

        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpPut(APIEndPointConstant.Promotions.UpdatePromotionEndPoint)]
        public async Task<IActionResult> UpdatePromotion([FromRoute] string code,[FromBody] PutPromotionRequest request)
        {
            ValidationResult validationResult = _putPromotionValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            try
            {
                var isUpdated = await _promotionService.UpdatePromotionAsync(code, request);
                if (!isUpdated)
                {
                    return NotFound(new { message = PromotionMessage.NotExistPromotion });
                }

                return Ok(new { message = PromotionMessage.PromotionUpdated });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = PromotionMessage.ValidationFailed, errors = ex.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = PromotionMessage.FailedToUpdatePromotion, details = ex.Message });
            }
        }
        #endregion

        #region Delete Promotion
        /// <summary>
        /// Delete a promotion.
        /// </summary>
        /// 

        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpDelete(APIEndPointConstant.Promotions.DeletePromotionEndPoint)]
        public async Task<IActionResult> DeletePromotion([FromRoute] string code)
        {
            var isDeleted = await _promotionService.DeletePromotionAsync(code);

            if (!isDeleted)
            {
                return NotFound(new { message = PromotionMessage.NotExistPromotion });
            }

            return Ok(new { message = PromotionMessage.PromotionDeleted });
        }
        #endregion
    }
}
