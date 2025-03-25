using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Configurations;
using Microsoft.Extensions.Options;
using System.Text.Json;
using CCSystem.Presentation.Services;
using System.Security.Cryptography.X509Certificates;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;
using CCSystem.BLL.Constants;
using Firebase.Storage;

namespace CCSystem.Presentation.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public List<GetAccountResponse> Accounts { get; set; } = default!;
        public IndexModel(IHttpClientFactory httpClientFactory, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClientFactory.CreateClient("AccountAPI");
            _apiEndpoints = apiEndpoints;
        }
        public async Task OnGetAsync()
        {
            try
            {
                var accounts =  await _httpClient.GetFromJsonAsync<List<GetAccountResponse>>(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.GetAccounts));
                if (accounts == null)
                {
                    accounts = new List<GetAccountResponse>();
                }
                //filter account
                Accounts = accounts.Where(a => a.Role != RoleConstant.Admin).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Accounts = new List<GetAccountResponse>();
            }
        }

        public async Task<IActionResult> OnPostLockAsync(int id)
        {
            var success = await _httpClient.PutAsync(_apiEndpoints.GetFullUrl(_apiEndpoints.Account.LockAccountUrl(id)), null);
            if (success.IsSuccessStatusCode)
            {
                ToastHelper.ShowSuccess(TempData, Message.AccountMessage.AccountLockedSuccessfully);
            }
            else
            {
                ToastHelper.ShowError(TempData, Message.AccountMessage.AccountLockedFailed);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnLockAsync(int id)
        {
            var url = _apiEndpoints.GetFullUrl(_apiEndpoints.Account.UnlockAccountUrl(id));
            var success = await _httpClient.PutAsync(url, null);
            if (success.IsSuccessStatusCode)
            {
                ToastHelper.ShowSuccess(TempData, Message.AccountMessage.AccountUnlockedSuccessfully);
            }
            else
            {
                ToastHelper.ShowError(TempData, Message.AccountMessage.AccountUnlockedFailed);
            }
            return RedirectToPage();
        }
    }
}
