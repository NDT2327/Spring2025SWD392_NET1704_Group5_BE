using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;
using CCSystem.Presentation.Configurations;
using System.Security.Principal;
using CCSystem.Presentation.Models.Accounts;

namespace CCSystem.Presentation.Pages.Accounts
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public CreateModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("AccountAPI");
            _apiEndpoints = apiEndpoints;
        }

        [BindProperty]
        public CreateAccountRequest Account { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await _httpClient.PostAsJsonAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.CreateAccount), Account);
            if (!result.IsSuccessStatusCode)
            {
                ToastHelper.ShowError(TempData, Message.AccountMessage.AccountCreatedFailed);
                return Page();
            }
            ToastHelper.ShowSuccess(TempData, Message.AccountMessage.AccountCreatedSuccessfully);
            return RedirectToPage("./Index");
        }
    }
}
