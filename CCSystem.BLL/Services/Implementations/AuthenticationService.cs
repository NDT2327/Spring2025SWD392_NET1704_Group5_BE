using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Infrastructure.DTOs.AccountTokens;
using CCSystem.Infrastructure.DTOs.JWTs;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Enums;
using CCSystem.Infrastructure.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.DAL.Redis.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CCSystem.DAL.SMTPs.Repositories;

namespace CCSystem.BLL.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private IMapper _mapper;
        private UnitOfWork _unitOfWork;

        public AuthenticationService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public async Task<AccountResponse> RegisterAsync(AccountRegisterRequest accountRequest, JWTAuth jwtAuth)
        {
            try
            {
                // Kiểm tra xem email đã tồn tại chưa
                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(accountRequest.Email);
                if (existedAccount != null)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.AlreadyExistEmail);
                }

                // Tạo mới đối tượng Account và hash mật khẩu
                Account newAccount = new Account
                {
                    Email = accountRequest.Email,
                    Password = PasswordUtil.HashPassword(accountRequest.Password),
                    // Gán trạng thái mặc định là ACTIVE, hoặc theo quy định của dự án
                    Status = AccountEnums.Status.ACTIVE.ToString(),
                    Role = accountRequest.Role.ToString(),
                    FullName = accountRequest.FullName,
                    Address = accountRequest.Address,
                    Phone = accountRequest.Phone,
                };

                // Thêm account mới vào repository
                await this._unitOfWork.AccountRepository.CreateAccountAsync(newAccount);

                await this._unitOfWork.CommitAsync();

                // Map đối tượng Account mới tạo sang AccountResponse để trả về cho client
                AccountResponse accountResponse = this._mapper.Map<AccountResponse>(newAccount);

                // Sinh JWT token cho tài khoản mới
                accountResponse = await GenerateTokenAsync(accountResponse, jwtAuth);

                return accountResponse;
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString(MessageConstant.RegisterMessage.FailRegister, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<AccountResponse> LoginAsync(AccountLoginRequest accountRequest, JWTAuth jwtAuth)
        {
            try
            {
                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(accountRequest.Email);
                if (existedAccount == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistEmail);
                }
                if (existedAccount != null && existedAccount.Status == AccountEnums.Status.SUSPENDED.ToString() ||
                    existedAccount != null && existedAccount.Status == AccountEnums.Status.INACTIVE.ToString())
                {
                    throw new BadRequestException(MessageConstant.LoginMessage.DisabledAccount);
                }
                //if (existedAccount != null && existedAccount.Password.Equals(accountRequest.Password) == false)
                //{
                //    throw new BadRequestException(MessageConstant.LoginMessage.InvalidEmailOrPassword);
                //}
                if (!PasswordUtil.VerifyPassword(accountRequest.Password, existedAccount.Password))
                {
                    throw new BadRequestException(MessageConstant.LoginMessage.InvalidEmailOrPassword);
                }
                AccountResponse accountResponse = this._mapper.Map<AccountResponse>(existedAccount);
                accountResponse = await GenerateTokenAsync(accountResponse, jwtAuth);
                return accountResponse;
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Email", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.LoginMessage.InvalidEmailOrPassword) ||
                    ex.Message.Equals(MessageConstant.LoginMessage.DisabledAccount))
                {
                    fieldName = "Login Failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }


        public async Task<AccountTokenResponse> ReGenerateTokensAsync(AccountTokenRequest accountTokenRequest, JWTAuth jwtAuth)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKeyBytes = Encoding.UTF8.GetBytes(jwtAuth.Key);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };


                //Check 1: Access token is valid format
                var tokenVerification = jwtTokenHandler.ValidateToken(accountTokenRequest.AccessToken, tokenValidationParameters, out var validatedToken);

                //Check 2: Check Alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        throw new BadRequestException(MessageConstant.ReGenerationMessage.InvalidAccessToken);
                    }
                }

                //Check 3: check accessToken expried?
                var utcExpiredDate = long.Parse(tokenVerification.Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiredDate = DateUtil.ConvertUnixTimeToDateTime(utcExpiredDate);
                if (expiredDate > DateTime.UtcNow)
                {
                    throw new BadRequestException(MessageConstant.ReGenerationMessage.NotExpiredAccessToken);
                }

                //Check 4: Check refresh token exist in Redis Db
                string accountId = tokenVerification.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
                if (!int.TryParse(accountId, out int accountIdInt))
                {
                    throw new BadRequestException("Invalid account ID in token.");
                }

                AccountToken accountTokenRedisModel = await this._unitOfWork.AccountTokenRedisRepository.GetAccountToken(accountIdInt);
                if (accountTokenRedisModel == null)
                {
                    throw new NotFoundException(MessageConstant.ReGenerationMessage.NotExistAuthenticationToken);
                }

                if (accountTokenRedisModel.RefreshToken.Equals(accountTokenRequest.RefreshToken) == false)
                {
                    throw new NotFoundException(MessageConstant.ReGenerationMessage.NotExistRefreshToken);
                }

                //Check 5: Id of refresh token == id of access token
                var jwtId = tokenVerification.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (accountTokenRedisModel.JWTId.Equals(jwtId) == false)
                {
                    throw new BadRequestException(MessageConstant.ReGenerationMessage.NotMatchAccessToken);
                }

                //Check 6: refresh token is expired
                if (accountTokenRedisModel.ExpiredDate < DateTime.UtcNow)
                {
                    throw new BadRequestException(MessageConstant.ReGenerationMessage.ExpiredRefreshToken);
                }

                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(int.Parse(accountId));
                AccountResponse accountResponse = this._mapper.Map<AccountResponse>(existedAccount);
                accountResponse = await GenerateTokenAsync(accountResponse, jwtAuth);
                return accountResponse.Tokens;
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Authentication Tokens", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Authentication Tokens", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        private async Task<AccountResponse> GenerateTokenAsync(AccountResponse accountResponse, JWTAuth jwtAuth)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuth.Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, accountResponse.Email),
                        new Claim(JwtRegisteredClaimNames.Email, accountResponse.Email),
                        new Claim(JwtRegisteredClaimNames.Sid, accountResponse.AccountId.ToString()),
                        new Claim("role", accountResponse.Role),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = credentials
                };

                var token = jwtTokenHandler.CreateToken(tokenDescription);
                string accessToken = jwtTokenHandler.WriteToken(token);
                string refreshToken = GenerateRefreshToken();

                AccToken accountToken = new AccToken()
                {
                    JWTId = token.Id,
                    RefreshToken = refreshToken,
                    ExpiredDate = DateTime.UtcNow.AddDays(5),
                    AccountId = accountResponse.AccountId
                };

                AccountToken accountTokenRedisModel = this._mapper.Map<AccountToken>(accountToken);

                await this._unitOfWork.AccountTokenRedisRepository.AddAccountToken(accountTokenRedisModel);

                AccountTokenResponse tokens = new AccountTokenResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                accountResponse.Tokens = tokens;
                return accountResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public async Task VerifyEmailToResetPasswordAsync(EmailVerificationRequest emailVerificationRequest)
        {
            try
            {
                Account account = await this._unitOfWork.AccountRepository.GetAccountAsync(emailVerificationRequest.Email);
                if (account == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistEmail);
                }
                EmailVerification emailVerificationRedisModel = this._unitOfWork.EmailRepository.SendEmailToResetPassword(emailVerificationRequest.Email);
                await this._unitOfWork.EmailVerificationRedisRepository.AddEmailVerificationAsync(emailVerificationRedisModel);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Email", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Excception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task ConfirmOTPCodeToResetPasswordAsync(OTPCodeVerificationRequest otpCodeVerificationRequest)
        {
            try
            {
                Account account = await this._unitOfWork.AccountRepository.GetAccountAsync(otpCodeVerificationRequest.Email);
                if (account == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistEmail);
                }
                EmailVerification emailVerificationRedisModel = await this._unitOfWork.EmailVerificationRedisRepository.GetEmailVerificationAsync(otpCodeVerificationRequest.Email);
                if (emailVerificationRedisModel == null)
                {
                    throw new BadRequestException(MessageConstant.VerificationMessage.NotAuthenticatedEmailBefore);
                }
                if (emailVerificationRedisModel.CreatedDate.AddMinutes(10) <= DateTime.Now)
                {
                    throw new BadRequestException(MessageConstant.VerificationMessage.ExpiredOTPCode);
                }
                if (emailVerificationRedisModel.OTPCode.Equals(otpCodeVerificationRequest.OTPCode) == false)
                {
                    throw new BadRequestException(MessageConstant.VerificationMessage.NotMatchOTPCode);
                }
                emailVerificationRedisModel.IsVerified = Convert.ToBoolean((int)EmailVerificationEnum.Status.VERIFIED);
                await this._unitOfWork.EmailVerificationRedisRepository.UpdateEmailVerificationAsync(emailVerificationRedisModel);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Email", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.VerificationMessage.NotAuthenticatedEmailBefore))
                {
                    fieldName = "Email";
                }
                else if (ex.Message.Equals(MessageConstant.VerificationMessage.ExpiredOTPCode)
                    || ex.Message.Equals(MessageConstant.VerificationMessage.NotMatchOTPCode))
                {
                    fieldName = "OTP code";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task ChangePasswordAsync(CCSystem.Infrastructure.DTOs.Accounts.ResetPasswordRequest resetPassword)
        {
            try
            {
                Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(resetPassword.Email);
                if (existedAccount == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistEmail);
                }
                EmailVerification emailVerificationRedisModel = await this._unitOfWork.EmailVerificationRedisRepository.GetEmailVerificationAsync(resetPassword.Email);
                if (emailVerificationRedisModel == null)
                {
                    throw new BadRequestException(MessageConstant.ChangePasswordMessage.NotAuthenticatedEmail);
                }
                if (emailVerificationRedisModel.IsVerified == Convert.ToBoolean((int)EmailVerificationEnum.Status.NOT_VERIFIRED))
                {
                    throw new BadRequestException(MessageConstant.ChangePasswordMessage.NotVerifiedEmail);
                }
                existedAccount.Password = PasswordUtil.HashPassword(resetPassword.NewPassword);
                this._unitOfWork.AccountRepository.UpdateAccount(existedAccount);
                await this._unitOfWork.CommitAsync();
                await this._unitOfWork.EmailVerificationRedisRepository.DeleteEmailVerificationAsync(emailVerificationRedisModel);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Email", ex.Message);
                throw new NotFoundException(error);
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Email", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
