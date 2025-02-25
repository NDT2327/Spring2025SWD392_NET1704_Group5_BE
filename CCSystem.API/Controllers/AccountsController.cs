using CCSystem.API.Authorization;
using CCSystem.API.Constants;
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

namespace CCSystem.API.Controllers
{
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;
        private IValidator<AccountSearchRequest> _searchAccountValidator;

        public AccountsController(IAccountService accountService, IValidator<AccountSearchRequest> searchAccountValidator)
        {
            this._accountService = accountService;
            this._searchAccountValidator = searchAccountValidator;
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
            // (Tùy chọn) Validate request tìm kiếm nếu có validator tương ứng
            ValidationResult validationResult = await this._searchAccountValidator.ValidateAsync(searchRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            // Gọi service để thực hiện tìm kiếm tài khoản theo các tiêu chí được truyền vào
            List<GetAccountResponse> accounts = await this._accountService.SearchAccountsAsync(searchRequest);

            // Nếu cần, có thể kiểm tra thêm xem danh sách kết quả có rỗng hay không và ném NotFoundException nếu cần.
            if (accounts == null || accounts.Count == 0)
            {
                throw new NotFoundException("Không tìm thấy tài khoản nào phù hợp với tiêu chí.");
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
        [HttpPost(APIEndPointConstant.Account.LockAccountEndpoint)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> LockAccount([FromRoute] AccountIdRequest accountId)
        {
            var result = await _accountService.LockAccount(accountId.Id);
            if (!result)
            {
                return NotFound(new { message = "Account not found" });
            }
            return Ok(new { message = "Account locked successfully" });
        }
        #endregion

        #region Unlock Account
        /// <summary>
        /// Unlock an account by accountId
        /// </summary>
        /// <param name="accountId">Account ID to unlock</param>
        /// <returns>Success message if unlocked</returns>
        [HttpPost(APIEndPointConstant.Account.UnlockAccountEndpoint)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> UnlockAccount([FromRoute] AccountIdRequest accountId)
        {
            var result = await _accountService.UnlockAccount(accountId.Id);
            if (!result)
            {
                return NotFound(new { message = "Account not found" });
            }
            return Ok(new { message = "Account unlocked successfully" });
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
        [Authorize]
        [HttpGet(APIEndPointConstant.Account.GetAccountProfileEndpoint)]
        public async Task<IActionResult> GetAccountProfile([FromBody] AccountIdRequest idRequest)
        {
            var claims = User.Claims;
            var result = await _accountService.GetAccountAsync(idRequest.Id, claims);
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
        public async Task<IActionResult> UpdateAccount([FromRoute] AccountIdRequest idRequest, [FromBody] UpdateAccountRequest updateAccountRequest)
        {
            var claims = User.Claims;
            await _accountService.UpdateAccountAsync(idRequest.Id, updateAccountRequest, claims);
            return Ok();
        }
        #endregion
    }
}