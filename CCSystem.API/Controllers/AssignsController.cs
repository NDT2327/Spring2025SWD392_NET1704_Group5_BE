using CCSystem.API.Authorization;
using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.ScheduleAssign;
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
    public class AssignsController : ControllerBase
    {
        private readonly IScheduleAssignService _scheduleAssignService;
        private readonly IValidator<AssignIdRequest> _assignIdValidator;

        public AssignsController(IScheduleAssignService scheduleAssignService, IValidator<AssignIdRequest> assignIdValidator)
        {
            this._scheduleAssignService = scheduleAssignService;
            this._assignIdValidator = assignIdValidator;
        }

        #region Get List Assigns
        ///<summary>
        /// Get list of Assign
        /// </summary>
        [ProducesResponseType(typeof(List<ScheduleAssignmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin, PermissionAuthorizeConstant.Staff, PermissionAuthorizeConstant.Housekeeper)]
        [HttpGet(APIEndPointConstant.ScheduleAssign.ScheduleAssignEndpoint)]
        public async Task<IActionResult> GetListAsync()
        {
            var responses = await _scheduleAssignService.GetAllAsync();
            return Ok(responses);
        }
        #endregion

        #region Get Assign By Id
        ///<summary>
        /// Get Assign By Id
        /// </summary>
        [ProducesResponseType(typeof(ScheduleAssignmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [HttpGet(APIEndPointConstant.ScheduleAssign.GetScheduleAssignByIdEndpoint)]
        public async Task<IActionResult> GetById([FromRoute] AssignIdRequest request)
        {
            ValidationResult validationResult = await _assignIdValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var assign = await _scheduleAssignService.GetByIdAsync(request.Id);
            return Ok(assign);
        }
        #endregion
    }
}
