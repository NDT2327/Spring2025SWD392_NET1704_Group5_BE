using CCSystem.API.Constants;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.DTOs.Services;
using CCSystem.BLL.DTOs.Category;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Services;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.Models;
using CCSystem.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CCSystem.API.Authorization;
using CCSystem.BLL.Constants;
using CCSystem.BLL.Errors;
using CCSystem.BLL.Utils;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.DTOs.Category;
using CCSystem.API.Constants;

namespace CCSystem.API.Controllers
{
    /// <summary>
    /// Controller for managing categories.
    /// </summary>
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories.
    /// </summary>
    [HttpGet(APIEndPointConstant.Category.GetAllCategoriesEndpoint)]
    public async Task<IActionResult> GetAllCategories()
    {
        IEnumerable<CategoryResponse> categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID.
    /// </summary>
    [HttpGet(APIEndPointConstant.Category.GetCategoryByIdEndpoint)]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        CategoryResponse? category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound(new { Message = "Category not found." });
        return Ok(category);
    }

    /// <summary>
    /// Create a new category.
    /// </summary>
    [HttpPost(APIEndPointConstant.Category.CreateCategoryEndpoint)]
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
        [HttpPut(APIEndPointConstant.Category.UpdateCategoryEndpoint)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequest request)
    {
        await _categoryService.UpdateCategoryAsync(id, request);
        return Ok(new { Message = "Category updated successfully." });
    }

        #region Delete Category
        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns>No content</returns>
        [HttpDelete(APIEndPointConstant.Category.DeleteCategoryEndpoint)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryService.DeleteCategoryAsync(id);

            return NoContent();
        }
        #endregion

        #region Search Category
        /// <summary>
        /// Search categories
        /// </summary>
        /// <returns>List of categories</returns>
        /// <response code="200">Return a list of services matching the criteria.</response>
        /// <response code="400">Invalid search criteria or business logic error.</response>
        /// <response code="500">Unexpected system error.</response>
        /// <exception cref="BadRequestException">Throw error if search criteria are invalid.</exception>
        /// <exception cref="NotFoundException">Throw error if related data are not found.</exception>
        /// <exception cref="Exception">Throw error for unexpected system issues.</exception>
        [ProducesResponseType(typeof(IEnumerable<ServiceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeConstant.ApplicationJson)]
        [HttpGet(APIEndPointConstant.Category.SearchCategoryEndpoint)]
        public async Task<IActionResult> GetSearchCategoryAsync([FromQuery] SearchCategoryRequest searchCategoryRequest)
        {
            string categoryName = searchCategoryRequest.CategoryName;
            bool? isActive = searchCategoryRequest.IsActive;
            var categories = await _categoryService.SearchCategoryAsync(categoryName, isActive);
            return Ok(categories);
        }
        #endregion
    }
}
