using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using CCSystem.Presentation.Services;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;

namespace CCSystem.Presentation.Pages.Accounts
{
    public class CreateModel : PageModel
    {
        private readonly AccountService _accountService;



        public CreateModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CreateAccountRequest Account { get; set; } = new();

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var success = await _accountService.CeateAccountAsync(Account);
            if (!success)
            {
                ToastHelper.ShowError(TempData, Message.AccountMessage.AccountCreatedFailed);
                return Page();
            }
            ToastHelper.ShowSuccess(TempData, Message.AccountMessage.AccountCreatedSuccessfully);
            return RedirectToPage("./Index");
        }
    }
}
