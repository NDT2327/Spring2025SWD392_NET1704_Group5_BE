using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Services;

namespace CCSystem.Presentation.Pages.Accounts
{
    public class EditModel : PageModel
    {
        private readonly AccountService _accountService;
        public EditModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public UpdateAccountRequest Account { get; set; } = new();

        [BindProperty]
        public IFormFile? AvatarFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if(account == null) return NotFound();

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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if(AvatarFile != null)
            {
                Account.Avatar = AvatarFile;
            }

            var success = await _accountService.UpdateAccountAsync(id, Account);

            if (success)
            {
                TempData["SuccessMessage"] = "Update Account Successfully!";
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, "Failed to update account");
            return Page();
        }
    }
}
