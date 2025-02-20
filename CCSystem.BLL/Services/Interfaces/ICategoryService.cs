using CCSystem.DAL.Models;
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
    }
}