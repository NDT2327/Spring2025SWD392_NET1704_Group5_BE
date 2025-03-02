using CCSystem.BLL.DTOs.Promotions;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public interface IPromotionService
    {
        public Task<List<GetPromotionResponse>> GetAllPromotionsAsync();
        public Task<GetPromotionResponse> GetPromotionByCodeAsync(string code);
        public Task<Promotion> CreatePromotionAsync(PostPromotionRequest request);
        public Task<bool> UpdatePromotionAsync(string code, PutPromotionRequest request);
        public Task<bool> DeletePromotionAsync(string code);
    }
}
