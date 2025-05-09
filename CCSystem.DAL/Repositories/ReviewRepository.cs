﻿using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public class ReviewRepository
    {
        private readonly SP25_SWD392_CozyCareContext _dbContext;

        public ReviewRepository(SP25_SWD392_CozyCareContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _dbContext.Reviews.ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _dbContext.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        public async Task AddAsync(Review review)
        {
            await _dbContext.Reviews.AddAsync(review);
        }

        public async Task UpdateAsync(Review review)
        {
            _dbContext.Reviews.Update(review);
            await Task.CompletedTask;
        }




        public async Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(int customerId)
        {
            return await _dbContext.Set<Review>()
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();
        }
        public async Task<List<Review>> GetReviewsByDetailIdAsync(int detailId)
        {
            return await _dbContext.Reviews
                .Where(r => r.DetailId == detailId)
                .ToListAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var review = await _dbContext.Reviews.FindAsync(id);
            if (review != null)
            {
                _dbContext.Reviews.Remove(review);
            }
        }
    }
}
