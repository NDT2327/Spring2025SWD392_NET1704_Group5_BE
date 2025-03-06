using CCSystem.BLL.DTOs.ScheduleAssign;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services.Interfaces
{
    public interface IScheduleAssignService
    {
        Task<List<ScheduleAssignmentResponse>> GetAllAsync();
        Task<ScheduleAssignmentResponse> GetByIdAsync(int id);
        Task AddAsync(PostScheduleAssignRequest request);
        Task UpdateAsync(PostScheduleAssignRequest request);
    }
}
