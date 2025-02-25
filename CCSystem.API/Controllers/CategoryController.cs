using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.BLL.DTOs.Category;
using CCSystem.API.Constants;

[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

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
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
    {
        CategoryResponse category = await _categoryService.CreateCategoryAsync(request);
        return Ok(category);
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

    /// <summary>
    /// Delete a category.
    /// </summary>
    [HttpDelete(APIEndPointConstant.Category.DeleteCategoryEndpoint)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        bool isDeleted = await _categoryService.DeleteCategoryAsync(id);
        if (!isDeleted) return NotFound(new { Message = "Category not found." });
        return Ok(new { Message = "Category deleted successfully." });
    }
}
