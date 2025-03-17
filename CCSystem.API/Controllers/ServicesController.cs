using CCSystem.API.Authorization;
using CCSystem.API.Constants;
using CCSystem.API.Validators.Services;
using CCSystem.BLL.Constants;
using CCSystem.Infrastructure.DTOs.Category;
using CCSystem.Infrastructure.DTOs.Services;
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
        private ICategoryService _categoryService;
        private IValidator<PostServiceRequest> _postServiceValidator;
        private IValidator<SearchServiceRequest> _searchServiceValidator;
        private IValidator<ServiceIdRequest> _serviceIdValidator;
        private IValidator<CategoryIdRequest> _categoryIdValidator;

        public ServicesController(IServiceHomeService serviceHomeService, IValidator<PostServiceRequest> postServiceValidator,
            IValidator<SearchServiceRequest> searchServiceValidator, IValidator<ServiceIdRequest> serviceIdValidator, ICategoryService categoryService, IValidator<CategoryIdRequest> categoryIdValidator)
        {
            this._serviceHomeService = serviceHomeService;
            this._postServiceValidator = postServiceValidator;
            this._searchServiceValidator = searchServiceValidator;
            this._serviceIdValidator = serviceIdValidator;
            this._categoryService = categoryService;
            this._categoryIdValidator = categoryIdValidator;
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
        /// <summary>
        /// Cập nhật thông tin dịch vụ dựa trên ID.
        /// </summary>
        /// <param name="id">ID của dịch vụ cần cập nhật.</param>
        /// <param name="request">Dữ liệu mới của dịch vụ.</param>
        /// <returns>Trả về kết quả thành công hoặc lỗi.</returns>
        [HttpPut(APIEndPointConstant.Service.UpdateServiceEndpoint)]
        public async Task<IActionResult> UpdateService(int id, [FromForm] PostServiceRequest request)
        {
            try
            {
                // Validate the request using the UpdateServiceValidator
                ValidationResult validationResult = await _postServiceValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    // Nếu không hợp lệ, tạo thông điệp lỗi từ validation
                    string errors = ErrorUtil.GetErrorsString(validationResult);
                    throw new BadRequestException(errors);
                }

                // Nếu validation thành công, thực hiện cập nhật dịch vụ
                await _serviceHomeService.UpdateServiceAsync(id, request);

                return Ok(new { Message = "Service updated successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        /// <summary>
        /// Vô hiệu hóa dịch vụ bằng cách đặt IsActive = false.
        /// </summary>
        /// <param name="id">ID của dịch vụ cần vô hiệu hóa.</param>
        /// <returns>Trả về kết quả thành công hoặc lỗi.</returns>
        [HttpPut(APIEndPointConstant.Service.DeleteServiceEndpoint)]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                await _serviceHomeService.DeleteServiceAsync(id);
                return Ok(new { Message = "Service deleted successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

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
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
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

        #region Get Service By Id
        /// <summary>
        /// Retrieves detailed information of a service by its Id.
        /// </summary>
        /// <param name="id">The Id of the service to retrieve.</param>
        /// <returns>
        /// Detailed information of the service.
        /// </returns>
        /// <response code="200">Service information retrieved successfully.</response>
        /// <response code="404">No service found with the provided Id.</response>
        /// <response code="500">An error occurred in the system.</response>
        /// <exception cref="NotFoundException">Thrown when no service exists with the provided Id.</exception>
        /// <exception cref="Exception">Throws a system exception if an error occurs.</exception>
        [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpGet(APIEndPointConstant.Service.GetServiceByIdEndpoint)]
        public async Task<IActionResult> GetServiceByIdAsync([FromRoute] ServiceIdRequest serviceId)
        {
            ValidationResult validationResult = await _serviceIdValidator.ValidateAsync(serviceId);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var service = await _serviceHomeService.GetServiceById(serviceId.Id);
            return Ok(service);
        }
        #endregion

        #region Get Services By Category Id
        /// <summary>
        /// Retrieves a list of services based on the specified category id.
        /// </summary>
        /// <param name="categoryId">The identifier of the category.</param>
        /// <returns>A list of services associated with the given category.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/services/category/1
        /// </remarks>
        /// <response code="200">Returns a list of services for the specified category.</response>
        /// <response code="400">Invalid category id or business logic error.</response>
        /// <response code="500">Unexpected system error.</response>
        /// <exception cref="NotFoundException">Thrown if the specified category does not exist.</exception>
        [ProducesResponseType(typeof(IEnumerable<ServiceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        [HttpGet(APIEndPointConstant.Service.GetByCategoryId)]
        public async Task<IActionResult> GetServicesByCategoryIdAsync([FromRoute] CategoryIdRequest request)
        {
            ValidationResult validationResult = await _categoryIdValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var services = await _serviceHomeService.GetServicesByCategoryId(request.Id);
            return Ok(services);

        }
        #endregion


    }
}
