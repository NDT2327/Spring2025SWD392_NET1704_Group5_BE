using CCSystem.Infrastructure.DTOs.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;
using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Models.Accounts;

namespace CCSystem.Presentation.Pages.Authentications
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        private readonly CookieHelper _cookieHelper;

        //login request
        [BindProperty]
        public AccountLoginRequest LoginRequest { get; set; } = default!;

        public LoginModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints, CookieHelper cookieHelper)
        {
            _httpClient = httpClientFactory.CreateClient("AuthenticationAPI");
            _apiEndpoints = apiEndpoints;
            _cookieHelper = cookieHelper;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return  Page();
            try
            {
                //save user's token into cookie
                var response =  await _httpClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Authentication.Login), LoginRequest);
                var accountResponse = await response.Content.ReadFromJsonAsync<AccountResponse>();
                if (!response.IsSuccessStatusCode || accountResponse?.Tokens == null)
                {

                    ToastHelper.ShowError(TempData, Message.AuthenMessage.LoginFailed, 3000);
                    return Page();
                }

                //create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, LoginRequest.Email),
                    new Claim(ClaimTypes.Role, accountResponse.Role),
                    new Claim(ClaimTypes.NameIdentifier, accountResponse.AccountId.ToString()),
                };
                //create identity vs principal
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                //save into cookies
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                _cookieHelper.SetCookie("accessToken", accountResponse.Tokens.AccessToken, 60);
                _cookieHelper.SetCookie("refreshToken", accountResponse.Tokens.RefreshToken, 60);
                ToastHelper.ShowSuccess(TempData, Message.AuthenMessage.LoginSuccess, 3000);
                return RedirectToPage("/Index");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.ToString());
                ToastHelper.ShowError(TempData, Message.AuthenMessage.LoginFailed, 3000);
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
