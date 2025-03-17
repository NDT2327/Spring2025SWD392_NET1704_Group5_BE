using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public class ServiceRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public ServiceRepository(SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetServicesByCategoryId(int categoryId)
        {
            try
            {
                var services = await _context.Services.Include(s => s.Category)
                    .Where(s => s.Category.CategoryId == categoryId)
                    .ToListAsync();
                return services;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Service>> GetListServicesAsync()
        {
            try
            {
                return await _context.Services.Include(s => s.Category).ToListAsync();
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Service> GetServiceAsync(int id)
        {
            try
            {
                return await _context.Services
                    .Include(s => s.Category)
                    .FirstOrDefaultAsync(s => s.ServiceId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Service>> SearchServiceAsync(
            string serviceName,
            string description,
            decimal? price,
            int? duration,
            bool? isActive,
            int? categoryId)
        {
            try
            {
                IQueryable<Service> query = _context.Services
                                    .AsNoTracking()
                                    .Include(s => s.Category);

                // Nếu tất cả các tham số đều null hoặc rỗng, trả về toàn bộ danh sách
                if (string.IsNullOrWhiteSpace(serviceName) &&
                    string.IsNullOrWhiteSpace(description) &&
                    price == null &&
                    duration == null &&
                    isActive == null &&
                    categoryId == null)
                {
                    return await query.OrderByDescending(s => s.ServiceId).ToListAsync();
                }


                // Áp dụng bộ lọc khi các giá trị không null hoặc không rỗng
                query = query.Where(s =>
                    (categoryId == null || s.CategoryId == categoryId) &&
                    (string.IsNullOrWhiteSpace(serviceName) || s.ServiceName.Contains(serviceName)) &&
                    (string.IsNullOrWhiteSpace(description) || s.Description.Contains(description)) &&
                    (price == null || s.Price == price) &&
                    (duration == null || s.Duration == duration) &&
                    (isActive == null || s.IsActive == isActive)
                );

                query = query.OrderByDescending(s => s.ServiceId);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while searching for services.", ex);
            }
        }
        public async Task Update(Service service)
        {
            _context.Services.Update(service);
            await Task.CompletedTask;
        }

        public async Task DeleteServiceAsync(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);

            if (service == null)
            {
                throw new KeyNotFoundException("Service not found");
            }

            service.IsActive = false;
            service.UpdatedDate = DateTime.UtcNow.AddHours(7);

            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }


        public async Task CreateServiceAsync(Service service)
        {
            try
            {
                await this._context.Services.AddAsync(service);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
