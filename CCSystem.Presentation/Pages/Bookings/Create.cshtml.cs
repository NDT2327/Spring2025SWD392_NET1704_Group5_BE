using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;

namespace CCSystem.Presentation.Pages.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext _context;

        public CreateModel(CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CustomerId"] = new SelectList(_context.Accounts, "AccountId", "Email");
        ViewData["PromotionCode"] = new SelectList(_context.Promotions, "Code", "Code");
            return Page();
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
