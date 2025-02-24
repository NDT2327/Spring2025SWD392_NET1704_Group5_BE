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

namespace CCSystem.API.Controllers
{
    /// <summary>
    /// Controller for managing categories.
    /// </summary>
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;
        private ICategoryService _categoryHomeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryRepository">The category repository.</param>
        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        #region Get All categories
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
        #endregion

        #region Get Category By Id
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
        #endregion

        #region Create Category
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
        #endregion

        #region Update Category
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
        #endregion

        #region Delete Category
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
            var categories = await _categoryHomeService.SearchCategoryAsync(categoryName, isActive);
            return Ok(categories);
        }
        #endregion
    }
}