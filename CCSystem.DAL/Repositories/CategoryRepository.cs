using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public void DeleteCategory(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}