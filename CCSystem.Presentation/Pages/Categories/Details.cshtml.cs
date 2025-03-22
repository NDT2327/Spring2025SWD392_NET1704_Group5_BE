using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using CCSystem.Presentation.Services;
using CCSystem.Infrastructure.DTOs.Category;

namespace CCSystem.Presentation.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly CategoryService _categoryService;

        public DetailsModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public CategoryResponse Category { get; set; } = new CategoryResponse();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            else
            {
                Category = category;
            }
            return Page();
        }
    }
}
