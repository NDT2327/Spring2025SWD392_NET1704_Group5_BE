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

namespace CCSystem.Presentation.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly AccountService _accountService;

        public List<GetAccountResponse>? Accounts { get; set; } = new List<GetAccountResponse>();
        public IndexModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                Accounts = await _accountService.GetAccountsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<IActionResult> OnPostLockAsync(int id)
        {
            var success = await _accountService.LockAccountAsync(id);
            if (success)
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
            var success = await _accountService.UnlockAccountAsync(id);
            if (success)
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
