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

namespace CCSystem.Presentation.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly AccountService _accountService;

        public List<GetAccountResponse>? Accounts { get; set; } = new List<GetAccountResponse> ();
        public IndexModel(AccountService accountService)
        {
            _accountService = accountService;
        }        
    
        public async Task OnGetAsync()
        {
            try
            {
                var accounts = await _accountService.GetAccountsAsync();
                if (accounts == null)
                {
                    Accounts = accounts;
                }
            }
            catch (Exception ex) { 
                Console.WriteLine(ex);
            }
        }
    }
}
