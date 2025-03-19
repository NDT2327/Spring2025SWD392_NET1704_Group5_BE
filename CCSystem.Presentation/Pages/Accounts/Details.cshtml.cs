using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Presentation.Services;

namespace CCSystem.Presentation.Pages.Accounts
{
    public class DetailsModel : PageModel
    {
        //private readonly CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext _context;
        private readonly AccountService _accountService;

        public DetailsModel(AccountService accountSerivce)
        {
            _accountService = accountSerivce;
        }

        public GetAccountResponse Account { get; set; } 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage("Index");
            }
            Account = await _accountService.GetAccountByIdAsync(id);
            if (Account == null) { 
            
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
