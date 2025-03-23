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

namespace CCSystem.Presentation.Pages.ServiceDetails
{
    public class EditModel : PageModel
    {
        private readonly CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext _context;

        public EditModel(CCSystem.DAL.DBContext.SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ServiceDetail ServiceDetail { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicedetail =  await _context.ServiceDetails.FirstOrDefaultAsync(m => m.ServiceDetailId == id);
            if (servicedetail == null)
            {
                return NotFound();
            }
            ServiceDetail = servicedetail;
           ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName");
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

            _context.Attach(ServiceDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceDetailExists(ServiceDetail.ServiceDetailId))
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

        private bool ServiceDetailExists(int id)
        {
            return _context.ServiceDetails.Any(e => e.ServiceDetailId == id);
        }
    }
}
