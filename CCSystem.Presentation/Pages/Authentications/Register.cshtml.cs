using CCSystem.Presentation.Models.Accounts;
using CCSystem.Presentation.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CCSystem.Presentation.Pages.Authentications
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        [BindProperty]
        public AccountRegisterRequest AccountRegisterRequest { get; set; } = default!;

        public string Message { get; set; } = string.Empty;

        public RegisterModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("AuthenticationAPI");
            _apiEndpoints = apiEndpoints;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();

            }
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Authentication.Register), AccountRegisterRequest);
                response.EnsureSuccessStatusCode();
                var registerAccount = await response.Content.ReadFromJsonAsync<AccountResponse>();
                Message = "Registration Successfully";
                return RedirectToPage("/Login");

            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
                return Page();

            }
        }
    }
}
