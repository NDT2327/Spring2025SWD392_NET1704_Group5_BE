using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CCSystem.Presentation.Pages.Authentications
{
    public class RegisterModel : PageModel
    {
        private readonly AuthenticationService _authService;

        [BindProperty]
        public AccountRegisterRequest AccountRegisterRequest { get; set; } = new();

        public string Message { get; set; }

        public RegisterModel(AuthenticationService autService)
        {
            _authService = autService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();

            }
            try
            {
                var response = await _authService.RegisterAsync(AccountRegisterRequest);
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
