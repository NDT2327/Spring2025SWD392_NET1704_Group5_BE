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
    public class ServiceDetailRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public ServiceDetailRepository(SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        public async Task<ServiceDetail> GetByIdAsync(int id)
        {
           var ServiceDetail = await _context.ServiceDetails.FindAsync(id);
            if (ServiceDetail == null)
            {
                throw new Exception("Service Detail not found");
            }
            return ServiceDetail;
        }
        public async Task<ServiceDetail> AddAsync(ServiceDetail serviceDetail)
        {
            await _context.ServiceDetails.AddAsync(serviceDetail);
            await _context.SaveChangesAsync();
            return serviceDetail;
        }

        public async Task<bool> UpdateAsync(ServiceDetail serviceDetail)
        {
            try
            {
                var existingServiceDetail = await _context.ServiceDetails.FindAsync(serviceDetail.ServiceDetailId);
                if (existingServiceDetail == null)
                {
                    return false; // Not found
                }

                // Update fields
                existingServiceDetail.ServiceId = serviceDetail.ServiceId;
                existingServiceDetail.OptionName = serviceDetail.OptionName;
                existingServiceDetail.OptionType = serviceDetail.OptionType;
                existingServiceDetail.BasePrice = serviceDetail.BasePrice;
                existingServiceDetail.Unit = serviceDetail.Unit;
                existingServiceDetail.Duration = serviceDetail.Duration;
                existingServiceDetail.Description = serviceDetail.Description;
                existingServiceDetail.IsActive = serviceDetail.IsActive;

                _context.Entry(existingServiceDetail).State = EntityState.Modified;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the service detail.", ex);
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var serviceDetail = await _context.ServiceDetails.FindAsync(id);
            if (serviceDetail == null)
            {
                return false; // Not found
            }

            _context.ServiceDetails.Remove(serviceDetail);
            await _context.SaveChangesAsync();
            return true; // Successfully deleted
        }

    }
}
