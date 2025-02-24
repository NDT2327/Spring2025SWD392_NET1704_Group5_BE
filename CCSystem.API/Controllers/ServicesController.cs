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
        private IValidator<SearchServiceRequest> _searchServiceValidator;
        private IValidator<ServiceIdRequest> _serviceIdValidator;

        public ServicesController(IServiceHomeService serviceHomeService, IValidator<PostServiceRequest> postServiceValidator,
            IValidator<SearchServiceRequest> searchServiceValidator, IValidator<ServiceIdRequest> serviceIdValidator)
        {
            this._serviceHomeService = serviceHomeService;
            this._postServiceValidator = postServiceValidator;
            this._searchServiceValidator = searchServiceValidator;
            this._serviceIdValidator = serviceIdValidator;
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

        #region Search Service
        /// <summary>
        /// Search services based on given criteria.
        /// </summary>
        /// <param name="searchServiceRequest">
        /// An object containing search criteria for services.
        /// </param>
        /// <returns>
        /// A list of services matching the search criteria.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET
        ///         CategoryName = MyCategory
        ///         ServiceName = MyService
        ///         Description = Service description
        ///         Price = 100.0
        ///         Duration = 60
        ///         IsActive = true
        /// </remarks>
        /// <response code="200">Return a list of services matching the criteria.</response>
        /// <response code="400">Invalid search criteria or business logic error.</response>
        /// <response code="500">Unexpected system error.</response>
        /// <exception cref="BadRequestException">Throw error if search criteria are invalid.</exception>
        /// <exception cref="NotFoundException">Throw error if related data are not found.</exception>
        /// <exception cref="Exception">Throw error for unexpected system issues.</exception>
        [ProducesResponseType(typeof(IEnumerable<ServiceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpGet(APIEndPointConstant.Service.SearchServiceEndpoint)]
        public async Task<IActionResult> GetSearchServiceAsync([FromQuery] SearchServiceRequest searchServiceRequest)
        {
            ValidationResult validationResult = await _searchServiceValidator.ValidateAsync(searchServiceRequest);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var services = await _serviceHomeService.SearchServiceAsync(searchServiceRequest);

            return Ok(services);
        }
        #endregion

        #region Get List Services
        /// <summary>
        /// Retrieves the list of services.
        /// </summary>
        /// <returns>
        /// A list of services.
        /// </returns>
        /// <response code="200">Services retrieved successfully.</response>
        /// <response code="500">An error occurred in the system.</response>
        /// <exception cref="Exception">Throws a system exception if an error occurs.</exception>
        [ProducesResponseType(typeof(List<ServiceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpGet(APIEndPointConstant.Service.ServiceEndpoint)]
        public async Task<IActionResult> GetListServicesAsync()
        {

            var services = await _serviceHomeService.GetListServicesAsync();
            return Ok(services);
        }
        #endregion

    }
}
