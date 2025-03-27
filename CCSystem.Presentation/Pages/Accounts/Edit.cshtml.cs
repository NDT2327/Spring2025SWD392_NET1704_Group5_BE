using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Configurations;
using System.Text.Json;
using System.Net.Http.Headers;
using CCSystem.Presentation.Models.Accounts;

namespace CCSystem.Presentation.Pages.Accounts
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;
        public EditModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("AccountAPI");
            _apiEndpoints = apiEndpoints;
        }

        [BindProperty]
        public UpdateAccountRequest Account { get; set; } = default!;

        [BindProperty]
        public IFormFile? AvatarFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string json = string.Empty;
            var response = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.GetAccountDetailsUrl(id)));
            if (response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                return NotFound();
            }
            var account = JsonSerializer.Deserialize<GetAccountResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                 ?? new GetAccountResponse();

            Account = new UpdateAccountRequest
            {
                Address = account.Address,
                Phone = account.Phone,
                FullName = account.FullName,
                Gender = account.Gender,
                Status = account.Status,
                Experience = account.Experience,
                Year = account.DateOfBirth?.Year,
                Month = account.DateOfBirth?.Month,
                Day = account.DateOfBirth?.Day,

            };


            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(Account.Address ?? ""), "Address");
            content.Add(new StringContent(Account.Phone ?? ""), "Phone");
            content.Add(new StringContent(Account.FullName ?? ""), "FullName");
            content.Add(new StringContent(Account.Gender ?? ""), "Gender");
            content.Add(new StringContent(Account.Status ?? ""), "Status");

            if (Account.Rating.HasValue) content.Add(new StringContent(Account.Rating.Value.ToString()), "Rating");
            if (Account.Experience.HasValue) content.Add(new StringContent(Account.Experience.Value.ToString()), "Experience");
            if (Account.Year.HasValue) content.Add(new StringContent(Account.Year.Value.ToString()), "Year");
            if (Account.Month.HasValue) content.Add(new StringContent(Account.Month.Value.ToString()), "Month");
            if (Account.Day.HasValue) content.Add(new StringContent(Account.Day.Value.ToString()), "Day");

            // Process Avatar
            if (AvatarFile != null)
            {
                var stream = AvatarFile.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(AvatarFile.ContentType);
                content.Add(fileContent, "Avatar", AvatarFile.FileName);
            }

            var response = await _httpClient.PutAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.UpdateAccountUrl(id)), content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Update Account Successfully!";
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to update account");
            return Page();
        }
    }
}
