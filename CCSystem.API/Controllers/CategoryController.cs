using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCSystem.BLL.DTOs.Category;


namespace CCSystem.API.Controllers
{
    /// <summary>
    /// API controller for managing categories.
    /// </summary>
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryService">The category service.</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get all categories.
        /// </summary>
        /// <returns>List of category responses.</returns>
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get category by ID.
        /// </summary>
        /// <param name="id">Category ID.</param>
        /// <returns>Category response.</returns>
        [HttpGet("get/{id}")]
        public async Task<ActionResult<CategoryResponse>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        /// <summary>
        /// Create a new category.
        /// </summary>
        /// <param name="request">Category request DTO.</param>
        /// <returns>Created category response.</returns>
        [HttpPost("create")]
        public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CategoryRequest request)
        {
            try
            {
                var category = await _categoryService.CreateCategoryAsync(request);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        /// <summary>
        /// Update a category.
        /// </summary>
        /// <param name="id">Category ID.</param>
        /// <param name="request">Updated category request DTO.</param>
        /// <returns>No content.</returns>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequest request)
        {
            await _categoryService.UpdateCategoryAsync(id, request);
            return NoContent();
        }

        /// <summary>
        /// Delete a category.
        /// </summary>
        /// <param name="id">Category ID.</param>
        /// <returns>No content if successful, NotFound otherwise.</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
