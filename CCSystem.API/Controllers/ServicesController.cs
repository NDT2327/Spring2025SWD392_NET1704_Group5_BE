using CCSystem.API.Authorization;
using CCSystem.API.Constants;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Services;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCSystem.API.Controllers
{
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private IServiceHomeService _serviceHomeService;
        private IValidator<PostServiceRequest> _postServiceValidator;

        public ServicesController(IServiceHomeService serviceHomeService, IValidator<PostServiceRequest> postServiceValidator)
        {
            this._serviceHomeService = serviceHomeService;
            this._postServiceValidator = postServiceValidator;
        }

        #region Create Service
        /// <summary>
        /// Create new service.
        /// </summary>
        /// <param name="postServiceRequest">
        /// An object that includes information about the service.
        /// </param>
        /// <returns>
        /// A success message about creating new service.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         POST
        ///         CategoryId = 1
        ///         ServiceName = MyService
        ///         Description = Service description
        ///         Image = [Image file]
        ///         Price = 100.0
        ///         Duration = 60
        ///         IsActive = true
        /// </remarks>
        /// <response code="200">Created new service successfully.</response>
        /// <response code="400">Some error about request data and business logic.</response>
        /// <response code="500">Some error about the system.</response>
        /// <exception cref="BadRequestException">Throw error about request data and business logic.</exception>
        /// <exception cref="NotFoundException">Throw error when related data are not found.</exception>
        /// <exception cref="Exception">Throw error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpPost(APIEndPointConstant.Service.ServiceEndpoint)]
        public async Task<IActionResult> PostCreateServiceAsync([FromForm] PostServiceRequest postServiceRequest)
        {
            // Giả sử bạn đã có validator cho PostServiceRequest
            ValidationResult validationResult = await _postServiceValidator.ValidateAsync(postServiceRequest);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            await this._serviceHomeService.CreateServiceAsync(postServiceRequest);

            return Ok(new
            {
                Message = MessageConstant.ServiceMessage.CreatedNewServiceSuccessfully
            });
        }
        #endregion

    }
}
