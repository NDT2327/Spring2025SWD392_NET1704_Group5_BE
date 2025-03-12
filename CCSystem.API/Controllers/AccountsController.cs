using CCSystem.API.Authorization;
using CCSystem.API.Constants;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CCSystem.BLL.Constants.MessageConstant;

namespace CCSystem.API.Controllers
{
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IValidator<UpdateAccountRequest> _updateAccountValidator;
        private IAccountService _accountService;
        private IValidator<AccountSearchRequest> _searchAccountValidator;

        public AccountsController(IAccountService accountService, IValidator<AccountSearchRequest> searchAccountValidator, IValidator<UpdateAccountRequest> updateAccountValidator)
        {
            this._accountService = accountService;
            this._searchAccountValidator = searchAccountValidator;
            this._updateAccountValidator = updateAccountValidator;
        }

        #region Search Accounts
        /// <summary>
        /// Search accounts based on given criteria.
        /// </summary>
        /// <param name="searchRequest">An object containing search criteria such as email, role, full name, etc.</param>
        /// <returns>
        /// A list of accounts matching the search criteria.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///     
        ///         GET /api/accounts/search?email=john&role=Admin
        /// </remarks>
        /// <response code="200">Search accounts successfully.</response>
        /// <response code="400">Some error about request data or business logic.</response>
        /// <response code="404">No matching accounts found.</response>
        /// <response code="500">Some error about the system.</response>
        /// <exception cref="BadRequestException">Thrown when the request data or business logic is invalid.</exception>
        /// <exception cref="NotFoundException">Thrown when no accounts are found matching the criteria.</exception>
        /// <exception cref="Exception">Thrown when a system error occurs.</exception>
        [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpGet(APIEndPointConstant.Account.SearchAccountsEndpoint)]
        public async Task<IActionResult> SearchAccountsAsync([FromQuery] AccountSearchRequest searchRequest)
        {
            ValidationResult validationResult = await this._searchAccountValidator.ValidateAsync(searchRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            List<GetAccountResponse> accounts = await this._accountService.SearchAccountsAsync(searchRequest);

            if (accounts == null || accounts.Count == 0)
            {
                throw new NotFoundException(AccountMessage.NoMatchingAccountsFound);
            }

            return Ok(accounts);
        }
        #endregion

        #region Lock Account
        /// <summary>
        /// Lock an account by accountId
        /// </summary>
        /// <param name="accountId">Account ID to lock</param>
        /// <returns>Success message if locked</returns>
        [HttpPut(APIEndPointConstant.Account.LockAccountEndpoint)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> LockAccount([FromRoute] AccountIdRequest accountId)
        {
            try
            {
                var result = await _accountService.LockAccount(accountId.Id);
                if (!result)
                {
                    return BadRequest(new { message = AccountMessage.AccountNotExistOrAlreadyLocked });
                }
                return Ok(new { message = AccountMessage.AccountLockedSuccessfully });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = AccountMessage.InternalServerError, error = ex.Message });
            }
        }
        #endregion

        #region Unlock Account
        /// <summary>
        /// Unlock an account by accountId
        /// </summary>
        /// <param name="accountId">Account ID to unlock</param>
        /// <returns>Success message if unlocked</returns>
        [HttpPut(APIEndPointConstant.Account.UnlockAccountEndpoint)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> UnlockAccount([FromRoute] AccountIdRequest accountId)
        {
            var result = await _accountService.UnlockAccount(accountId.Id);
            if (!result)
            {
                return BadRequest(new { message = AccountMessage.AccountAlreadyUnlocked });
            }
            return Ok(new { message = AccountMessage.AccountUnlockedSuccessfully });
        }
        #endregion

        #region Get Account Profile
        /// <summary>
        /// Get account profile by accountId
        /// 

        [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [HttpGet(APIEndPointConstant.Account.GetAccountProfileEndpoint)]
        public async Task<IActionResult> GetAccountProfile([FromRoute] AccountIdRequest accountId)
        {
            
            var result = await _accountService.GetAccountByIdAsync(accountId.Id);
            if (result == null)
            {
                return NotFound(new { message = "Account not found" });
            }
            return Ok(result);
        }
        #endregion

        #region View List of Accounts
        /// <summary>
        /// Retrieve a list of accounts
        /// Actor must have Admin permission
        /// 
        [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [HttpGet(APIEndPointConstant.Account.AccountEndpoint)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]

        public async Task<IActionResult> GetAccounts([FromQuery] AccountsListRequest accountsListRequest)
        {
            var accounts = await _accountService.GetAccountsListAsync(accountsListRequest);
            return Ok(accounts);
        }

        #endregion

        #region Update Account
        /// <summary>
        /// 

        [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [HttpPut(APIEndPointConstant.Account.UpdateAccountEndpoint)]
        public async Task<IActionResult> UpdateAccount([FromRoute] int id, [FromForm] UpdateAccountRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = AccountMessage.InvalidRequest });
            }

            var validationResult = await _updateAccountValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                return BadRequest(new { message = errors });
            }

            try
            {
                await _accountService.UpdateAccountAsync(id, request);
                return Ok(new { message = AccountMessage.AccountUpdatedSuccessfully });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = AccountMessage.UpdateError, error = ex.Message });
            }
        }


        #endregion
    }
}