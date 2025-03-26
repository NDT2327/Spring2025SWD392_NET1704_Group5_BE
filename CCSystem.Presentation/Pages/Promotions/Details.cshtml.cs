using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;

namespace CCSystem.Presentation.Pages.Promotions
{
    public class DetailsModel : PageModel
    {
        private readonly CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext _context;

        public DetailsModel(CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        public Promotion Promotion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions.FirstOrDefaultAsync(m => m.Code == id);
            if (promotion == null)
            {
                return NotFound();
            }
            else
            {
                Promotion = promotion;
            }
            return Page();
        }
    }
}
