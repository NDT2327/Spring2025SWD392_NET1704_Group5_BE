using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Services;
using CCSystem.BLL.DTOs.ServiceDetails;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CCSystem.API.Controllers
{
    /// <summary>
    /// Controller for managing service details.
    /// </summary>
    [ApiController]
    public class ServiceDetailController : ControllerBase
    {
        #region Init controller 
        private readonly IServiceDetailService _serviceDetailService;
        private readonly IValidator<PostServiceDetailRequest> _postServiceDetailValidator;
        private readonly IValidator<PutServiceDetailRequest> _putServiceDetailValidator;

        /// <summary>
        /// Controller for managing service details.
        /// </summary>
        public ServiceDetailController(IServiceDetailService serviceDetailService, IValidator<PostServiceDetailRequest> postServiceDetailValidator, IValidator<PutServiceDetailRequest> putServiceDetailValidator)
        {
            this._serviceDetailService = serviceDetailService;
            _postServiceDetailValidator = postServiceDetailValidator;
            _putServiceDetailValidator = putServiceDetailValidator;
        }

        #endregion

        #region Get Service Detail

        /// <summary>
        /// Get a service detail by ID.
        /// </summary>
        /// 
        /// <param name="id">The Id of the service detail to retrieve.</param>
        [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpGet(APIEndPointConstant.ServiceDetail.ServiceDetailByIdEndPoint)]
        public async Task<IActionResult> GetServiceDetailById(int id)
        {
            var request = new GetServiceDetailRequest { serviceDetailId = id };
            var serviceDetail = await _serviceDetailService.GetServiceDetailByIdAsync(id);
            return Ok(serviceDetail);
        }
        #endregion

        #region Create Service Detail
        /// <summary>
        /// Create a service detail.
        /// </summary>
        ///
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpPost(APIEndPointConstant.ServiceDetail.CreateServiceDetailEndPoint)]
        public async Task<IActionResult> CreateServiceDetail([FromBody] PostServiceDetailRequest request)
        {
            ValidationResult validationResult = _postServiceDetailValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            try
            {
                var response = await _serviceDetailService.CreateServiceDetailAsync(request);
                return CreatedAtAction(nameof(CreateServiceDetail), new { id = response.ServiceDetailId }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return UnprocessableEntity(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        #endregion

        #region Update Service Detail
        /// <summary>
        /// Update a service detail.
        /// </summary>
        /// 
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpPut(APIEndPointConstant.ServiceDetail.UpdateServiceDetailEndPoint)]

        public async Task<IActionResult> UpdateServiceDetail([FromRoute]int id, [FromBody] PutServiceDetailRequest request)
        {
            ValidationResult validationResult = _putServiceDetailValidator.Validate(request);
            if (!validationResult.IsValid) {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                bool isUpdated = await _serviceDetailService.UpdateServiceDetailAsync(id, request);
                if (!isUpdated)
                {
                    return NotFound(new { Message = "ServiceDetail not found or update failed." });
                }

                return Ok(new { Message = "ServiceDetail updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        #endregion

        #region Delete Service Detail
        /// <summary>
        /// Delete a service detail.
        /// </summary>
        /// 

        [HttpDelete(APIEndPointConstant.ServiceDetail.DeleteServiceDetailEndPoint)]
        public async Task<IActionResult> DeleteServiceDetail([FromRoute] int id)
        {
            try
            {
                await _serviceDetailService.DeleteServiceDetailAsync(id);
                return Ok(new {message = "Deleted suncessfully"});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
        #endregion
    }
}
