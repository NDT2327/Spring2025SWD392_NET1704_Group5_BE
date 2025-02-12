using AutoMapper;
using CCSystem.BLL.Constants;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.DTOs.AccountTokens;
using CCSystem.BLL.DTOs.JWTs;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Enums;
using CCSystem.DAL.Infrastructures;
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
                    throw new BadRequestException("Email đã tồn tại.");
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
                if (existedAccount != null && existedAccount.Password.Equals(accountRequest.Password) == false)
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


        public Task<AccountTokenResponse> ReGenerateTokensAsync(AccountTokenRequest accountTokenRequest, JWTAuth jwtAuth)
        {
            throw new NotImplementedException();
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
    }
}
