using CCSystem.DAL.Models;
using CCSystem.BLL.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCSystem.BLL.DTOs.Accounts;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(CreateCategory createCategory);
        Task<List<CategoryResponse>> SearchCategoryAsync(string categoryName, bool? isActive);
    }
}