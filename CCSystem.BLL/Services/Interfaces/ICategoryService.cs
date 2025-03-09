using CCSystem.DAL.Models;
using CCSystem.BLL.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCSystem.BLL.DTOs.Accounts;
using CCSystem.BLL.DTOs.Category;
using System.Data.Common;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface ICategoryService
    {


        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(CategoryRequest request);
        Task UpdateCategoryAsync(int id, CategoryRequest request);
        Task DeleteCategoryAsync(int id);
        //Task<Category> CreateCategoryAsync(CategoryRequest request);
        Task<List<CategoryResponse>> SearchCategoryAsync(string categoryName, bool? isActive);
    }
}