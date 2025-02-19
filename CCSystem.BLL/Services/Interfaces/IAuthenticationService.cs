using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.DTOs.AccountTokens;
using CCSystem.BLL.DTOs.JWTs;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<AccountResponse> RegisterAsync(AccountRegisterRequest accountRequest, JWTAuth jwtAuth);
        public Task<AccountResponse> LoginAsync(AccountLoginRequest accountRequest, JWTAuth jwtAuth);
        public Task<AccountTokenResponse> ReGenerateTokensAsync(AccountTokenRequest accountTokenRequest, JWTAuth jwtAuth);
        public Task VerifyEmailToResetPasswordAsync(EmailVerificationRequest emailVerificationRequest);
        public Task ConfirmOTPCodeToResetPasswordAsync(OTPCodeVerificationRequest otpCodeVerificationRequest);
        public Task ChangePasswordAsync(DTOs.Accounts.ResetPasswordRequest resetPassword);

    }
}
