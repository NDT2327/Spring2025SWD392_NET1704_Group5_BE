using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Configurations;
using System.Text.Json;
using CCSystem.Presentation.Models.Accounts;

namespace CCSystem.Presentation.Pages.Accounts
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public DetailsModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("AccountAPI");
            _apiEndpoints = apiEndpoints;
        }

        public GetAccountResponse Account { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.GetAccountDetailsUrl(id)));
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Account = JsonSerializer.Deserialize<GetAccountResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new GetAccountResponse();
            }

            if (Account == null)
            {
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}
