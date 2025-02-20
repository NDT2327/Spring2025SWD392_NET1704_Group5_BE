using CCSystem.API.Constants;
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
    /// Controller for managing categories.
    /// </summary>
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryRepository">The category repository.</param>
        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet(APIEndPointConstant.Category.GetAllCategoriesEndpoint)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get category by Id
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns>Category</returns>
        [HttpGet(APIEndPointConstant.Category.GetCategoryByIdEndpoint)]
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
        /// Create a new category
        /// </summary>
        /// <param name="dto">Category data transfer object</param>
        /// <returns>Created category</returns>
        [HttpPost(APIEndPointConstant.Category.CreateCategoryEndpoint)]
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
        /// Update a category
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <param name="category">Updated category data</param>
        /// <returns>No content</returns>
        [HttpPut(APIEndPointConstant.Category.UpdateCategoryEndpoint)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            await _categoryRepository.UpdateCategoryAsync(category);

            return NoContent();
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns>No content</returns>
        [HttpDelete(APIEndPointConstant.Category.DeleteCategoryEndpoint)]
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