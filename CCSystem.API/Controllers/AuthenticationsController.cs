using CCSystem.API.Authorization;
using CCSystem.API.Constants;
using CCSystem.BLL.Constants;
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
        private IValidator<EmailVerificationRequest> _emailVerificationValidator;
        private IValidator<OTPCodeVerificationRequest> _otpCodeVerificationValidator;
        private IValidator<BLL.DTOs.Accounts.ResetPasswordRequest> _resetPasswordValidator;


        public AuthenticationsController(IAuthenticationService authenticationService, IOptions<JWTAuth> jwtAuthOptions,
            IValidator<AccountLoginRequest> accountRequestValidator, IValidator<AccountTokenRequest> accountTokenRequestValidator, IValidator<AccountRegisterRequest> accountRegisterRequestValidator, IValidator<EmailVerificationRequest> emailVerificationValidator, IValidator<OTPCodeVerificationRequest> otpCodeVerificationValidator, IValidator<BLL.DTOs.Accounts.ResetPasswordRequest> resetPasswordValidator)
        {
            this._authenticationService = authenticationService;
            this._jwtAuthOptions = jwtAuthOptions;
            this._accountRequestValidator = accountRequestValidator;
            this._accountTokenRequestValidator = accountTokenRequestValidator;
            this._accountRegisterRequestValidator = accountRegisterRequestValidator;
            this._emailVerificationValidator = emailVerificationValidator;
            this._otpCodeVerificationValidator = otpCodeVerificationValidator;
            this._resetPasswordValidator = resetPasswordValidator;
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
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.Authentication.Register)]
        //[PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> PostRegisterAsync([FromBody] AccountRegisterRequest account)
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

        #region Verify Email
        /// <summary>
        /// Verify email before resetting password.
        /// </summary>
        /// <param name="emailVerificationRequest">
        /// EmailVerificationRequest object contains Email property.
        /// </param>
        /// <returns>
        /// A success message about the sentting OTP code to Email.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "email": "abc@gmail.com"
        ///         }
        /// </remarks>
        /// <response code="200">Sent OTP Code to Email Successfully.</response>
        /// <response code="404">Some Error about request data that are not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.Authentication.EmailVerificationEndpoint)]
        public async Task<IActionResult> PostVerifyEmail([FromBody] EmailVerificationRequest emailVerificationRequest)
        {
            ValidationResult validationResult = await this._emailVerificationValidator.ValidateAsync(emailVerificationRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._authenticationService.VerifyEmailToResetPasswordAsync(emailVerificationRequest);
            return Ok(new
            {
                Message = MessageConstant.VerificationMessage.SentEmailConfirmationSuccessfully
            });
        }
        #endregion

        #region Verify OTP Code
        /// <summary>
        /// Compare sent OTP Code in the system with receiver's OTP Code. 
        /// </summary>
        /// <param name="otpCodeVerificationRequest">
        /// OTPCodeVerificationRequest object contains Email property and OTPCode property.
        /// </param>
        /// <returns>
        /// A success message when the OTP Code in the system matchs to receiver's OTP Code.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "email": "abc@gmail.com",
        ///             "otpCode": "000000"
        ///         }
        /// </remarks>
        /// <response code="200">Sent OTP Code to Email Successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data that are not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.Authentication.OTPVerificationEndpoint)]
        public async Task<IActionResult> PostConfirmOTPCode([FromBody] OTPCodeVerificationRequest otpCodeVerificationRequest)
        {
            ValidationResult validationResult = await this._otpCodeVerificationValidator.ValidateAsync(otpCodeVerificationRequest);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._authenticationService.ConfirmOTPCodeToResetPasswordAsync(otpCodeVerificationRequest);
            return Ok(new
            {
                Message = MessageConstant.VerificationMessage.ConfirmedOTPCodeSuccessfully
            });
        }
        #endregion 

        #region Reset Password
        /// <summary>
        /// A new password will be updated after the email is verified before.
        /// </summary>
        /// <param name="resetPassword">
        /// ResetPassword object contains Email property and new password property.
        /// Notice that the new password must be hashed with MD5 algorithm before sending to Login API.
        /// </param>
        /// <returns>
        /// A success message about the resetring password procedure.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT
        ///         {
        ///             "email": "abc@gmail.com"
        ///             "newPassword": "********"
        ///         }
        /// </remarks>
        /// <response code="200">A success message about the resetring password procedure.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPut(APIEndPointConstant.Authentication.PasswordResetation)]
        public async Task<IActionResult> PutResetPasswordAsync([FromBody] BLL.DTOs.Accounts.ResetPasswordRequest resetPassword)
        {
            ValidationResult validationResult = await this._resetPasswordValidator.ValidateAsync(resetPassword);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            await this._authenticationService.ChangePasswordAsync(resetPassword);
            return Ok(new
            {
                Message = MessageConstant.AuthenticationMessage.ResetPasswordSuccessfully
            });
        }
        #endregion

        #region Re-GenerateTokens API
        /// <summary>
        /// Re-generate pair token from the old pair token that are provided by the MBKC system before.
        /// </summary>
        /// <param name="accountToken">
        /// AccountToken Object contains access token property and refresh token property.
        /// </param>
        /// <returns>
        /// The new pair token (Access token, Refresh token) to continue access into the MBKC system.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "accessToken": "abcxyz"
        ///             "refreshToken": "klmnopq"
        ///         }
        /// </remarks>
        /// <response code="200">Re-Generate Token Successfully.</response>
        /// <response code="404">Some Error about request data that are not found.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="NotFoundException">Throw Error about request data that are not found.</exception>
        /// <exception cref="BadRequestException">Throw Error about request data and logic bussiness.</exception>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [ProducesResponseType(typeof(AccountTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeConstant.ApplicationJson)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpPost(APIEndPointConstant.Authentication.ReGenerationTokens)]
        public async Task<IActionResult> PostReGenerateTokensAsync([FromBody] AccountTokenRequest accountToken)
        {
            ValidationResult validationResult = await this._accountTokenRequestValidator.ValidateAsync(accountToken);
            if (validationResult.IsValid == false)
            {
                string errors = ErrorUtil.GetErrorsString(validationResult);
                throw new BadRequestException(errors);
            }
            AccountTokenResponse accountTokenResponse = await this._authenticationService.ReGenerateTokensAsync(accountToken, this._jwtAuthOptions.Value);
            return Ok(accountTokenResponse);
        }
        #endregion
    }
}
