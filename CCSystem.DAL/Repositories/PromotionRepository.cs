using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CCSystem.DAL.Repositories
{
    public class PromotionRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public PromotionRepository(SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        public async Task<List<Promotion>> GetListPromotionsAsync()
        {
            try
            {
                return await _context.Promotions.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Promotion> GetPromotionByCodeAsync(string code)
        {
            var promo = await _context.Promotions.FirstOrDefaultAsync(p => p.Code == code);

            return promo ?? throw new Exception("Promotion " +code+" not found.");
        }

        public async Task AddPromotionAsync(Promotion promotion)
        {

            var existingPromotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Code == promotion.Code);
            if (existingPromotion != null)
            {
                throw new Exception("Promotion already exists");
            }
            try
            {
                await _context.Promotions.AddAsync(promotion);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the promotion.", ex);
            }
        }

        public async Task<bool> UpdatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task DeletePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }

    }
}
