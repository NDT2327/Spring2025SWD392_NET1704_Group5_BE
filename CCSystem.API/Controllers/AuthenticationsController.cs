using CCSystem.API.Authorization;
using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.DTOs.AccountTokens;
using CCSystem.BLL.DTOs.JWTs;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace CCSystem.API.Controllers
{
    [ApiController]
    public class AuthenticationsController : ControllerBase
    {
        private IAuthenticationService _authenticationService;
        private IOptions<JWTAuth> _jwtAuthOptions;
        private IValidator<AccountLoginRequest> _accountRequestValidator;
        private IValidator<AccountTokenRequest> _accountTokenRequestValidator;
        private IValidator<AccountRegisterRequest> _accountRegisterRequestValidator;

        public AuthenticationsController(IAuthenticationService authenticationService, IOptions<JWTAuth> jwtAuthOptions,
            IValidator<AccountLoginRequest> accountRequestValidator, IValidator<AccountTokenRequest> accountTokenRequestValidator, IValidator<AccountRegisterRequest> accountRegisterRequestValidator)
        {
            this._authenticationService = authenticationService;
            this._jwtAuthOptions = jwtAuthOptions;
            this._accountRequestValidator = accountRequestValidator;
            this._accountTokenRequestValidator = accountTokenRequestValidator;
            this._accountRegisterRequestValidator = accountRegisterRequestValidator;
            
        }

        #region Register
        /// <summary>
        /// Register a new account into the system.
        /// </summary>
        /// <param name="account">
        /// Account object contains Email and Password properties.
        /// Notice that the password must be hashed with MD5 algorithm before sending to Register API.
        /// 
        /// </param>
        /// <returns>
        /// An object with a JSON format that contains Account Id, Email, Role name, and a pair token (access token, refresh token).
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "email": "abc@gmail.com",
        ///         "password": "********"
        ///     }
        /// </remarks>
        /// <response code="200">Register Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic business.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.MultipartFormData)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.Authentication.Register)]
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> PostRegisterAsync([FromForm] AccountRegisterRequest account)
        {
            // Validate the incoming request using the dedicated validator for Register
            ValidationResult validationResult = await this._accountRegisterRequestValidator.ValidateAsync(account);
            if (!validationResult.IsValid)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            // Call the service to register the new account.
            // The RegisterAsync method should handle:
            // - Checking for email duplication,
            // - Hashing the password,
            // - Creating the account in the database,
            // - And generating JWT tokens.
            AccountResponse accountResponse = await this._authenticationService.RegisterAsync(account, this._jwtAuthOptions.Value);

            return Ok(accountResponse);
        }
        #endregion

        #region Login API
        /// <summary>
        /// Login to access into the system by your account.
        /// </summary>
        /// <param name="account">
        /// Account object contains Email property and Password property. 
        /// Notice that the password must be hashed with MD5 algorithm before sending to Login API.
        /// </param>
        /// <returns>
        /// An Object with a json format that contains Account Id, Email, Role name, and a pair token (access token, refresh token).
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "email": "abc@gmail.com"
        ///             "password": "********"
        ///         }
        /// </remarks>
        /// <response code="200">Login Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.Authentication.Login)]
        public async Task<IActionResult> PostLoginAsync([FromBody] AccountLoginRequest account)
        {
            ValidationResult validationResult = await this._accountRequestValidator.ValidateAsync(account);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }

            AccountResponse accountResponse = await this._authenticationService.LoginAsync(account, this._jwtAuthOptions.Value);
            return Ok(accountResponse);
        }
        #endregion
    }
}
