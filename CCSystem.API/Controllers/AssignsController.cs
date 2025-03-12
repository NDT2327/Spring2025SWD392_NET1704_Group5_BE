﻿using Azure.Core;
using CCSystem.API.Authorization;
using CCSystem.API.Constants;
using CCSystem.API.Validators.ScheduleAssign;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Accounts;
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
        private readonly IValidator<PostScheduleAssignRequest> _postScheduleAssignValidator;
        private readonly IValidator<AccountIdRequest> _accountIdValidator;
        private readonly IValidator<PatchAssignStatusRequest> _patchAssignStatusValidator;
        private readonly IValidator<CompleteAssignmentRequest> _completeAssignmentValidator;
        private readonly IValidator<ConfirmAssignmentRequest> _confirmAssignmentValidator;
        public AssignsController(IScheduleAssignService scheduleAssignService, IValidator<AssignIdRequest> assignIdValidator, IValidator<PostScheduleAssignRequest> postScheduleAssignValidator, IValidator<AccountIdRequest> accountIdValidator, IValidator<PatchAssignStatusRequest> patchAssignStatusValidator, IValidator<CompleteAssignmentRequest> completeAssignmentValidator, IValidator<ConfirmAssignmentRequest> confirmAssignmentValidator)
        {
            this._scheduleAssignService = scheduleAssignService;
            this._assignIdValidator = assignIdValidator;
            this._postScheduleAssignValidator = postScheduleAssignValidator;
            this._accountIdValidator = accountIdValidator;
            this._patchAssignStatusValidator = patchAssignStatusValidator;
            this._completeAssignmentValidator = completeAssignmentValidator;
            this._confirmAssignmentValidator = confirmAssignmentValidator;
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
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            var assign = await _scheduleAssignService.GetByIdAsync(request.Id);
            return Ok(assign);
        }
        #endregion

        #region Post Assign Schedule
        ///<summary>
        ///Assign housekeeper in work
        /// </summary>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        //[Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin, PermissionAuthorizeConstant.Staff, PermissionAuthorizeConstant.Housekeeper)]
        [HttpPost(APIEndPointConstant.ScheduleAssign.ScheduleAssignEndpoint)]
        public async Task<IActionResult> PostAssignHousekeeper([FromBody] PostScheduleAssignRequest request)
        {
            ValidationResult validationResult = await _postScheduleAssignValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            await this._scheduleAssignService.AddAsync(request);
            return Ok(new
            {
                Message = MessageConstant.ScheduleAssign.CreatedNewScheduleAssignSuccessfully
            });
        }
        #endregion

        #region Get Assignments By Housekeeper
        /// <summary>
        /// Retrieves schedule assignments for a given housekeeper.
        /// </summary>
        /// <param name="housekeeperId">The ID of the housekeeper.</param>
        /// <returns>A list of schedule assignments.</returns>
        /// <response code="200">Returns the list of assignments.</response>
        /// <response code="404">If the housekeeper does not exist.</response>
        /// <response code="500">If there is an internal server error.</response>
        [ProducesResponseType(typeof(List<ScheduleAssignmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpGet(APIEndPointConstant.ScheduleAssign.GetScheduleAssignByHousekeeperIdEndpoint)]
        public async Task<IActionResult> GetAssignsByHousekeeper([FromRoute] AccountIdRequest request)
        {
            ValidationResult validationResult = await _accountIdValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            var assignments = await _scheduleAssignService.GetAssignsByHousekeeper(request.Id);
            return Ok(assignments);
        }
        #endregion

        #region Change Assignment Status
        /// <summary>
        /// Updates the status of an assignment.
        /// </summary>
        /// <param name="request">The request object containing the assignment ID and new status.</param>
        /// <returns>A success message if the status is updated.</returns>
        /// <response code="200">Assignment status updated successfully.</response>
        /// <response code="400">If the status value is invalid.</response>
        /// <response code="404">If the assignment does not exist.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPatch(APIEndPointConstant.ScheduleAssign.ChangeStatusEndpoint)]
        public async Task<IActionResult> ChangeAssignStatus([FromBody] PatchAssignStatusRequest request)
        {
            ValidationResult validationResult = await _patchAssignStatusValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            await _scheduleAssignService.ChangeAssignStatus(request);
            return Ok(new { Message = "Assignment status updated successfully!" });
        }
        #endregion

        #region Complete Assignment and Notify Customer
        /// <summary>
        /// Completes an assignment and notifies the customer via email.
        /// </summary>
        /// <param name="request">The request object containing assignment and housekeeper information.</param>
        /// <returns>A success message if the operation is completed successfully.</returns>
        /// <response code="200">Assignment completed and customer notified successfully.</response>
        /// <response code="400">If the assignment does not belong to the specified housekeeper.</response>
        /// <response code="404">If the assignment or booking detail is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.ScheduleAssign.CompleteAssignmentEndpoint)]
        public async Task<IActionResult> CompleteAssignmentAndNotifyCustomer([FromBody] CompleteAssignmentRequest request)
        {
            ValidationResult validationResult = await _completeAssignmentValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            await _scheduleAssignService.CompleteAssignmentAndNotifyCustomer(request);
            return Ok(new { Message = "Assignment completed and customer notified successfully!" });
        }
        #endregion

        #region Confirm Assignment
        /// <summary>
        /// Confirms the completion of an assignment by the customer.
        /// </summary>
        /// <param name="request">The request object containing the assignment ID and customer confirmation.</param>
        /// <returns>A success message if the assignment is confirmed successfully.</returns>
        /// <response code="200">Assignment confirmed successfully.</response>
        /// <response code="400">If the assignment cannot be confirmed.</response>
        /// <response code="404">If the assignment does not exist.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.ScheduleAssign.ConfirmAssignmentEndpoint)]
        public async Task<IActionResult> ConfirmAssignment([FromBody] ConfirmAssignmentRequest request)
        {
            ValidationResult validationResult = await _confirmAssignmentValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            await _scheduleAssignService.ConfirmAssignment(request);
            return Ok(new { Message = "Assignment confirmed successfully!" });
        }
        #endregion

    }
}
