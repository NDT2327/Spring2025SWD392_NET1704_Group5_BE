using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystemAuthenticationService = CCSystem.Presentation.Services.AuthenticationService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;

namespace CCSystem.Presentation.Pages.Authentications
{
    public class LoginModel : PageModel
    {
        private readonly CCSystemAuthenticationService _authenticationService;
        private readonly CookieHelper _cookieHelper;

        //login request
        [BindProperty]
        public AccountLoginRequest LoginRequest { get; set; } = new();

        public LoginModel(CCSystemAuthenticationService authenticationService, CookieHelper cookieHelper)
        {
            _authenticationService = authenticationService;
            _cookieHelper = cookieHelper;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                //save user's token into cookie
                var response = await _authenticationService.LoginAsync(LoginRequest);

                if (response == null || response.Tokens == null)
                {

                    ToastHelper.ShowError(TempData, Message.AuthenMessage.LoginFailed, 3000);
                    return Page();
                }

                //create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, response.FullName),
                    new Claim(ClaimTypes.Role, response.Role),
                    new Claim(ClaimTypes.NameIdentifier, response.AccountId.ToString()),
                };
                //create identity vs principal
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                //save into cookies
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                _cookieHelper.SetCookie("accessToken", response.Tokens.AccessToken, 60);
                _cookieHelper.SetCookie("refreshToken", response.Tokens.RefreshToken, 60);

                ToastHelper.ShowSuccess(TempData, Message.AuthenMessage.LoginSuccess, 3000);
                return RedirectToPage("/Index");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.ToString());
                ToastHelper.ShowError(TempData, Message.AuthenMessage.LoginFailed, 3000);
                TempData.Keep();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _cookieHelper.RemoveCookie("accessToken");
            _cookieHelper.RemoveCookie("refreshToken");

            ToastHelper.ShowSuccess(TempData, Message.AuthenMessage.LogoutSuccess, 3000);
            return RedirectToPage("/Authentications/Login");
        }
    }
}
