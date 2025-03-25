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
using CCSystem.Infrastructure.DTOs.ServiceDetails;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Presentation.Helpers;

namespace CCSystem.Presentation.Pages.Services
{
    public class CreateModel : PageModel
    {
        private readonly ServiceService _serviceService;
        private readonly CategoryService _categoryService;
        public CreateModel(ServiceService serviceService, CategoryService categoryService)
        {
            _serviceService = serviceService;
            _categoryService = categoryService;

        }

        [BindProperty]
        public PostServiceRequest Service { get; set; } = new PostServiceRequest();

        public async Task<IActionResult> OnGet()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
        ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
            return Page();
        }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                return Page();
            }
            var result = await _serviceService.CreateServiceAsync(Service);
            if (!result)
            {
                ToastHelper.ShowError(TempData, "Create failed");
                return Page();
            }
            ToastHelper.ShowSuccess(TempData, "Create Service succcessfully!");

            return RedirectToPage("./Index");
        }
    }
}
