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

namespace CCSystem.Presentation.Pages.Promotions
{
    public class EditModel : PageModel
    {
        private readonly CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext _context;

        public EditModel(CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Promotion Promotion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion =  await _context.Promotions.FirstOrDefaultAsync(m => m.Code == id);
            if (promotion == null)
            {
                return NotFound();
            }
            Promotion = promotion;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Promotion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PromotionExists(Promotion.Code))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PromotionExists(string id)
        {
            return _context.Promotions.Any(e => e.Code == id);
        }
    }
}
