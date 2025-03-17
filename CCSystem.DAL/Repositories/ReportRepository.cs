using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public class ReportRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public ReportRepository(SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        public async Task<List<Report>> GetAllAsync()
        {
            return await _context.Reports.ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports.FindAsync(id);
        }
        public async Task<IEnumerable<Report>> GetReportsByHousekeeperIdAsync(int housekeeperId)
        {
            return await _context.Set<Report>()
                .Where(r => r.HousekeeperId == housekeeperId)
                .ToListAsync();
        }
        public async Task<List<Report>> GetByAssignIdAsync(int assignId)
        {
            return await _context.Reports
                .Include(r => r.Assign)
                .Include(r => r.Housekeeper)
                .Where(r => r.AssignId == assignId)
                .ToListAsync();
        }

        public async Task AddAsync(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            await _context.Reports.AddAsync(report);
        }


        public async Task UpdateAsync(Report report)
        {
            _context.Reports.Update(report) ;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string newStatus)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                throw new KeyNotFoundException($"Report with ID {id} not found.");
            }

            report.TaskStatus = newStatus; 
            _context.Reports.Update(report); 
            await _context.SaveChangesAsync(); 
        }
    }
}
