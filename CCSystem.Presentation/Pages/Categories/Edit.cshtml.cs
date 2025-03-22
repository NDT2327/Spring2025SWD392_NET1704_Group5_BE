using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CCSystem.Infrastructure.DTOs.Category;
using CCSystem.Presentation.Services;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;
using CCSystem.BLL.Constants;

namespace CCSystem.Presentation.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly CategoryService _categoryService;

        public EditModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public CategoryRequest Category { get; set; } = new();

        [BindProperty]
        public int CategoryId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category =  await _categoryService.GetCategoryAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            CategoryId = category.CategoryId;
            Category.CategoryName = category.CategoryName ?? string.Empty;
            Category.Description = category.Description ?? string.Empty;
            Category.IsActive = category.IsActive;
            Console.WriteLine($"CategoryId khi GET: {CategoryId}");


            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"CategoryId khi POST: {CategoryId}");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (CategoryId == 0) {
                ToastHelper.ShowWarning(TempData, "Category ID khong hop le");
                return Page();
            }
            var success = await _categoryService.UpdateCategoryAsync(CategoryId, Category);
            if (!success) {
                ToastHelper.ShowError(TempData, MessageConstant.CategoryMessage.UpdateCategoryFailed);
                return Page();
            }
            else
            {
                ToastHelper.ShowInfo(TempData, MessageConstant.CategoryMessage.CategoryUpdated);
            }
            return RedirectToPage("./Index");
        }
    }
}
