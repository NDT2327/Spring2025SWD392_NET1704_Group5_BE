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
using CCSystem.Presentation.Services;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Presentation.Helpers;

namespace CCSystem.Presentation.Pages.Services
{
    public class EditModel : PageModel
    {
        private readonly ServiceService _serviceService;
        private readonly CategoryService _categoryService;
        public EditModel(ServiceService serviceService, CategoryService categoryService)
        {
            _serviceService = serviceService;
            _categoryService = categoryService;

        }

        [BindProperty]
        public PostServiceRequest Service { get; set; } = new PostServiceRequest();

        [BindProperty]
        public int ServiceId {  get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var service = await _serviceService.GetServiceAsync(id.Value);
                if (service == null)
                {
                    return NotFound();
                }
                var categories = await _categoryService.GetAllCategoriesAsync();
                var selectedCategory = categories.FirstOrDefault(c => c.CategoryName == service.CategoryName);
                if (selectedCategory == null) {
                    throw new Exception($"{service.CategoryName} not found");
                
                }
                Service = new PostServiceRequest
                {
                    CategoryId = selectedCategory.CategoryId,
                    ServiceName = service.ServiceName,
                    Description = service.Description,
                    Price = service.Price,
                    Duration = service.Duration,
                    IsActive = service.IsActive,
                };
                ServiceId = service.ServiceId;
                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                return Page();
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
                ToastHelper.ShowError(TempData, $"{ex.Message}");
                return Page();  
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                bool updatedSuccess = await _serviceService.UpdateServiceAsync(ServiceId, Service);
                if (updatedSuccess) {
                    ToastHelper.ShowSuccess(TempData, "Service updated successfully!");
                    return RedirectToPage("./Details", new { id = ServiceId });
                }
                else
                {
                    ToastHelper.ShowError(TempData, "Failed to update service");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError(TempData, $"{ex.Message}");
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                return Page();
            }
        }
    }
}
