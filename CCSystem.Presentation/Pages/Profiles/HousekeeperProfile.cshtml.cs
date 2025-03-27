using CCSystem.Presentation.Configurations;
using CCSystem.Presentation.Constants;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Models.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace CCSystem.Presentation.Pages.Profiles
{
    [Authorize(Roles = "CUSTOMER")]

    public class HousekeeperProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public HousekeeperProfileModel(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }
        public CustomerProfile CustomerProfile { get; set; }


        //get information of housekeeper role
        //experience, review by customer
        public async Task<IActionResult> OnGetAsync()
        {
            //get accountid from cookie
            var accountIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(accountIdString, out int accountId))
            {
                //if cannot get account id -> return to login page
                return RedirectToPage("/Authentications/Login");

            }

            //call api get account
            var accountResponse = await _httpClient.GetAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.GetAccountDetailsUrl(accountId)));
            if (accountResponse.IsSuccessStatusCode)
            {
                var json = await accountResponse.Content.ReadAsStringAsync();
                CustomerProfile = JsonSerializer.Deserialize<CustomerProfile>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new CustomerProfile();
            }
            else
            {
                ToastHelper.ShowError(TempData, "Cannot load account data");
                return Page();
            }

            //check role is HOuskeeper or not
            if(CustomerProfile.Role != Common.Role.Housekeeper)
            {
                ToastHelper.ShowError(TempData, "This account is not a houskeeper");
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}
