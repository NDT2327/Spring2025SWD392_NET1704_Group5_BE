using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CCSystem.DAL.Repositories.CategoryRepository;

namespace CCSystem.DAL.Repositories
{
    public class CategoryRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public CategoryRepository(SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            await _context.Categories.AddAsync(category);
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
        }

        public async Task DeleteCategory(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            // Chuyển trạng thái từ ACTIVE sang INACTIVE
            category.IsActive = false;
            _context.Categories.Update(category);
        }


        public async Task<Category> GetCategoryByName(string name)
        {
            try
            {
                return await _context.Categories.FirstOrDefaultAsync(s => s.CategoryName == name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Category>> SearchCategoryAsync(string categoryName, bool? isActive)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(c => c.CategoryName.Contains(categoryName));
            }
            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var categories = await query.ToListAsync();

            return categories;
        }
        

    }
}