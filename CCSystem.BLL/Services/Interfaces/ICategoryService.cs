using CCSystem.DAL.Models;
using CCSystem.Infrastructure.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCSystem.Infrastructure.DTOs.Accounts;
using CCSystem.Infrastructure.DTOs.Category;
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