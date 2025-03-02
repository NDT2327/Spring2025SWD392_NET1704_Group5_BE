using CCSystem.BLL.DTOs.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCSystem.BLL.Services
{
    public interface IReportService
    {
        Task<ReportResponse> CreateReportAsync(ReportRequest request);
        Task<ReportResponse> UpdateReportAsync(int id, ReportRequest request);
        Task<bool> DeleteReportAsync(int id);
        Task<ReportResponse> GetReportByIdAsync(int id);
        Task<IEnumerable<ReportResponse>> GetAllReportsAsync();
    }
}
