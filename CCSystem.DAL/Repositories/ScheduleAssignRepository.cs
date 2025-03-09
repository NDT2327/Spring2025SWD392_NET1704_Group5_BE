using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public class ScheduleAssignRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public ScheduleAssignRepository(SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        // Get all assignments
        public async Task<List<ScheduleAssignment>> GetAllAsync()
        {
            return await _context.ScheduleAssignments
                .Include(sa => sa.Housekeeper)
                .Include(sa => sa.Detail)
                .ToListAsync();
        }

        // Get assignment by ID
        public async Task<ScheduleAssignment> GetByIdAsync(int id)
        {
            return await _context.ScheduleAssignments
                .FirstOrDefaultAsync(sa => sa.AssignmentId == id);
        }

        // Create new assignment
        public async Task AddAsync(ScheduleAssignment assignment)
        {
            await _context.ScheduleAssignments.AddAsync(assignment);
        }

        // Update assignment
        public async Task UpdateAsync(ScheduleAssignment assignment)
        {
            _context.ScheduleAssignments.Update(assignment);
        }

        // Delete assignment
        public async Task DeleteAsync(int id)
        {
            var assignment = await _context.ScheduleAssignments.FindAsync(id);
            if (assignment != null)
            {
                _context.ScheduleAssignments.Remove(assignment);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ScheduleAssignment, bool>> predicate)
        {
            return await _context.ScheduleAssignments.AnyAsync(predicate);
        }

        public async Task<List<ScheduleAssignment>> GetAssignsByHousekeeper(int housekeeperId)
        {
            return await _context.ScheduleAssignments
                .Where(sa => sa.HousekeeperId == housekeeperId)
                .ToListAsync();

        }

        public async Task<List<ScheduleAssignment>> GetAssignmentByDetailId(int detailId)
        {
            return await _context.ScheduleAssignments
                .Where(sa => sa.DetailId == detailId)
                .ToListAsync();
        }

    }
}
