using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.Services;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCSystem.API.Controllers
{
    /// <summary>
    /// Controller quản lý danh mục dịch vụ.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        /// <summary>
        /// Khởi tạo CategoryController với CategoryRepository.
        /// </summary>
        /// <param name="categoryRepository">Repository danh mục.</param>
        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Lấy tất cả danh mục.
        /// </summary>
        /// <returns>Danh sách danh mục.</returns>
        [HttpGet("getallcategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Lấy danh mục theo ID.
        /// </summary>
        /// <param name="id">ID danh mục.</param>
        /// <returns>Thông tin danh mục.</returns>
        [HttpGet("getcategorybyid/{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        /// <summary>
        /// Tạo danh mục mới.
        /// </summary>
        /// <param name="dto">Dữ liệu tạo danh mục.</param>
        /// <returns>Danh mục vừa tạo.</returns>
        [HttpPost("createcategory")]
        public async Task<ActionResult> CreateCategory([FromBody] CreateCategory dto)
        {
            var category = new Category
            {
                CategoryName = dto.Name,
                Description = dto.Description,
                Image = "",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _categoryRepository.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
        }

        /// <summary>
        /// Xóa danh mục theo ID.
        /// </summary>
        /// <param name="id">ID danh mục cần xóa.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("deletecategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepository.DeleteCategoryAsync(id);

            return NoContent();
        }
    }
}
