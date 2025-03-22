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
using CCSystem.Infrastructure.DTOs.Category;
using CCSystem.Presentation.Helpers;
using CCSystem.Presentation.Constants;

namespace CCSystem.Presentation.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly CategoryService _categoryService;
        public CreateModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CategoryRequest Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var success = await _categoryService.CreateCategoryAsync(Category);
                if (!success)
                {

                    // Hiển thị lỗi trên giao diện
                    ToastHelper.ShowError(TempData, Message.Category.CreatedFailed);
                    return Page();
                }

                ToastHelper.ShowSuccess(TempData, Message.Category.CreatedSuccessfully);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                Console.WriteLine($"❌ [EXCEPTION] Error creating category: {ex.Message}");
                Console.WriteLine($"🔍 StackTrace: {ex.StackTrace}");

                // Hiển thị lỗi trên giao diện
                ToastHelper.ShowError(TempData, "Lỗi xảy ra khi tạo danh mục. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}
