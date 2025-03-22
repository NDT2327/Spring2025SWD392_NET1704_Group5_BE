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

namespace CCSystem.Presentation.Pages.Services
{
    public class EditModel : PageModel
    {
        private readonly CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext _context;

        public EditModel(CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Service Service { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service =  await _context.Services.FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }
            Service = service;
           ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
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

            _context.Attach(Service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(Service.ServiceId))
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

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }
    }
}
