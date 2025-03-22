using CCSystem.BLL.Errors;
using CCSystem.BLL.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using CCSystem.DAL.Enums;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.Infrastructure.DTOs.Accounts;
using System.IdentityModel.Tokens.Jwt;


namespace CCSystem.API.Authorization
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private string[] _roles;
        public PermissionAuthorizeAttribute(params string[] roles)
        {
            this._roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                IAccountService accountService = context.HttpContext.RequestServices.GetService<IAccountService>();
                var currentController = context.RouteData.Values["controller"];
                var currentActionName = context.RouteData.Values["action"];
                string email = context.HttpContext.User.Claims.First(x => x.Type.ToLower() == ClaimTypes.Email).Value;
                string accountId = context.HttpContext.User.Claims.First(x => x.Type.ToLower() == JwtRegisteredClaimNames.Sid).Value;

                GetAccountResponse existedAccount = accountService.GetAccountAsync(int.Parse(accountId), context.HttpContext.User.Claims).Result;


                bool isActiveAccount = existedAccount.Status.ToLower().Equals(AccountEnums.Status.ACTIVE.ToString().ToLower()) ? true : false;

                if (isActiveAccount == false)
                {
                    context.Result = new ObjectResult("Unauthorized")
                    {
                        StatusCode = 401,
                        Value = new
                        {
                            Message = JsonConvert.DeserializeObject<List<ErrorDetail>>(ErrorUtil.GetErrorString("Unauthorized", "You are not allowed to access this API."))
                        }
                    };
                }
                var expiredClaim = long.Parse(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiredDate = DateUtil.ConvertUnixTimeToDateTime(expiredClaim);
                if (expiredDate <= DateTime.UtcNow.AddHours(7))
                {
                    context.Result = new ObjectResult("Unauthorized")
                    {
                        StatusCode = 401,
                        Value = new
                        {
                            Message = JsonConvert.DeserializeObject<List<ErrorDetail>>(ErrorUtil.GetErrorString("Unauthorized", "You are not allowed to access this API."))
                        }
                    };
                }
                else
                {

                    var roleClaim = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == ClaimTypes.Role.ToLower());

                    if (roleClaim == null || this._roles.FirstOrDefault(x => x.ToLower().Equals(roleClaim.Value.ToLower())) == null)
                    {
                        context.Result = new ObjectResult("Forbidden")
                        {
                            StatusCode = 403,
                            Value = new
                            {
                                Message = JsonConvert.DeserializeObject<List<ErrorDetail>>(
                                    ErrorUtil.GetErrorString("Forbidden", "You are not allowed to access this function!")
                                )
                            }
                        };
                        return;
                    }
                    else
                    {
                        return;
                    }

                }
            }
            else
            {
                context.Result = new ObjectResult("Unauthorized")
                {
                    StatusCode = 401,
                    Value = new
                    {
                        Message = JsonConvert.DeserializeObject<List<ErrorDetail>>(ErrorUtil.GetErrorString("Unauthorized", "You are not allowed to access this API."))
                    }
                };
            }
        }
    }
}
