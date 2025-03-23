using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CCSystem.Presentation.Services;
using CCSystem.Infrastructure.DTOs.Category;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;

namespace CCSystem.Presentation.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly CategoryService _categoryService;

        public IndexModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<CategoryResponse> Category { get; set; } = new List<CategoryResponse>();

        public async Task OnGetAsync()
        {
            var (success, data, errorMessage) = await _categoryService.GetAllCategoriesAsync();
            if (!success)
            {
                ToastHelper.ShowError(TempData, errorMessage);
                Category = new List<CategoryResponse>();
            }
            else
            {
                Category = data;
            }
        }

        public async Task<IActionResult> OnPostLockAsync(int id)
        {
            var success = await _categoryService.LockCategoryAsync(id);
            if (success)
            {
                ToastHelper.ShowSuccess(TempData, Message.Category.LockSuccessfully);
            }
            else
            {
                ToastHelper.ShowError(TempData, Message.Category.LockFailed);
            }
            return RedirectToPage();
        }
    }
}
