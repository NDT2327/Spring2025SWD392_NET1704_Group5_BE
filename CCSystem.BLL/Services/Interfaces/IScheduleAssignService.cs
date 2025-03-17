using CCSystem.Infrastructure.DTOs.ScheduleAssign;
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
        //Task UpdateAsync(PostScheduleAssignRequest request);
        Task<List<ScheduleAssignmentResponse>> GetAssignsByHousekeeper(int housekeeperId);
        Task ChangeAssignStatus(PatchAssignStatusRequest request);
        Task CompleteAssignmentAndNotifyCustomer(CompleteAssignmentRequest request);
        Task ConfirmAssignment(ConfirmAssignmentRequest request);
        Task RequestCancel(CancelAssignmentRequest request);
        Task<List<ScheduleAssignmentResponse>> GetCancelRequests();
        Task ConfirmCancelRequest(ConfirmCancelAssignmentRequest request);

	}
}
