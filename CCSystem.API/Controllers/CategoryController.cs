﻿using CCSystem.API.Constants;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Infrastructure.DTOs.Services;
using CCSystem.Infrastructure.DTOs.Category;
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
using CCSystem.Infrastructure.DTOs.Category;
using CCSystem.API.Constants;
using static CCSystem.BLL.Constants.MessageConstant;
using static CCSystem.API.Constants.APIEndPointConstant;

namespace CCSystem.API.Controllers
{
    /// <summary>
    /// Controller for managing categories.
    /// </summary>
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        /// <summary>
        /// Constructor để inject service quản lý danh mục.
        /// </summary>
        /// <param name="categoryService">Service xử lý danh mục</param>
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
            var categories = await _categoryService.GetAllCategoriesAsync();

            foreach (var category in categories)
            {
                Console.WriteLine($"Category: {category.CategoryName}, Image: {category.ImageUrl}");
            }

            return Ok(new { message = CategoryMessage.Success, data = categories });
        }

        /// <summary>
        /// Get category by ID.
        /// </summary>
        [HttpGet(APIEndPointConstant.Category.GetCategoryByIdEndpoint)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = CategoryMessage.CategoryNotFound });
            }

            Console.WriteLine($"[DEBUG] Fetched Category: {category.CategoryName}, ImageUrl: {category.ImageUrl}");

            return Ok(new { message = CategoryMessage.Success, Data = category });
        }

        /// <summary>
        /// Create a new category.
        /// </summary>
        [HttpPost(APIEndPointConstant.Category.CreateCategoryEndpoint)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryRequest request)
        {
            await _categoryService.CreateCategoryAsync(request);
            return Ok(new { message = CategoryMessage.CategoryCreated });
        }

        /// <summary>
        /// Update a category.
        /// </summary>
        [HttpPut(APIEndPointConstant.Category.UpdateCategoryEndpoint)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryRequest request)
        {
            await _categoryService.UpdateCategoryAsync(id, request);
            return Ok(new { message = CategoryMessage.CategoryUpdated });
        }

        #region Delete Category
        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns>No content</returns>
        [HttpPut(APIEndPointConstant.Category.DeleteCategoryEndpoint)]
        [PermissionAuthorize(PermissionAuthorizeConstant.Admin)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = CategoryMessage.CategoryNotFound });
            }

            await _categoryService.DeleteCategoryAsync(id);
            return Ok(new { message = CategoryMessage.CategoryDeleted });
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
