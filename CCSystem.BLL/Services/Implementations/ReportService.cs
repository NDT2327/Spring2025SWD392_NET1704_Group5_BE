using CCSystem.BLL.DTOs.Report;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Models;

namespace CCSystem.BLL.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly UnitOfWork _unitOfWork;

        // Constructor để sử dụng UnitOfWork
        public ReportService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Tạo báo cáo mới
        public async Task<ReportResponse> CreateReportAsync(ReportRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request không được để trống");
            }

            var report = new Report
            {
                HousekeeperId = request.HousekeeperId,
                AssignId = request.AssignId,
                WorkDate = request.WorkDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TotalHours = request.TotalHours,
                TaskStatus = request.TaskStatus
            };

            try
            {
                // Thêm báo cáo vào repository
                await _unitOfWork.ReportRepository.AddAsync(report);

                // Cam kết thay đổi và kiểm tra nếu có lỗi
                await _unitOfWork.CommitAsync();

                if (report.RecordId == 0)
                {
                    throw new InvalidOperationException("Không thể tạo báo cáo. RecordId không được gán.");
                }

                return new ReportResponse
                {
                    RecordId = report.RecordId,
                    HousekeeperId = report.HousekeeperId,
                    AssignId = report.AssignId,
                    WorkDate = report.WorkDate,
                    StartTime = report.StartTime,
                    EndTime = report.EndTime,
                    TotalHours = report.TotalHours,
                    TaskStatus = report.TaskStatus
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo báo cáo: " + ex.Message, ex);
            }
        }

        // Cập nhật báo cáo
        public async Task<ReportResponse> UpdateReportAsync(int id, ReportRequest request)
        {
            // Kiểm tra xem request có null không
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request không được null.");
            }

            var report = await _unitOfWork.ReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return null; // Hoặc có thể ném exception tùy theo yêu cầu
            }

            // Kiểm tra các thuộc tính trong request có hợp lệ không
            if (request.HousekeeperId == 0 || request.AssignId == 0)
            {
                throw new ArgumentException("HousekeeperId và AssignId phải có giá trị hợp lệ.");
            }

            report.HousekeeperId = request.HousekeeperId;
            report.AssignId = request.AssignId;
            report.WorkDate = request.WorkDate;
            report.StartTime = request.StartTime;
            report.EndTime = request.EndTime;
            report.TotalHours = request.TotalHours;
            report.TaskStatus = request.TaskStatus;

            await _unitOfWork.ReportRepository.UpdateAsync(report);
            await _unitOfWork.CommitAsync();

            return new ReportResponse
            {
                RecordId = report.RecordId,
                HousekeeperId = report.HousekeeperId,
                AssignId = report.AssignId,
                WorkDate = report.WorkDate,
                StartTime = report.StartTime,
                EndTime = report.EndTime,
                TotalHours = report.TotalHours,
                TaskStatus = report.TaskStatus
            };
        }
        // Xóa báo cáo
        public async Task<bool> DeleteReportAsync(int id)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return false;
            }

            await _unitOfWork.ReportRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return true;
        }

        // Lấy báo cáo theo ID
        public async Task<ReportResponse> GetReportByIdAsync(int id)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return null; // Hoặc có thể ném exception tùy theo yêu cầu
            }

            return new ReportResponse
            {
                RecordId = report.RecordId,
                HousekeeperId = report.HousekeeperId,
                AssignId = report.AssignId,
                WorkDate = report.WorkDate,
                StartTime = report.StartTime,
                EndTime = report.EndTime,
                TotalHours = report.TotalHours,
                TaskStatus = report.TaskStatus
            };
        }

        // Lấy tất cả báo cáo
        public async Task<IEnumerable<ReportResponse>> GetAllReportsAsync()
        {
            var reports = await _unitOfWork.ReportRepository.GetAllAsync();

            return reports.Select(report => new ReportResponse
            {
                RecordId = report.RecordId,
                HousekeeperId = report.HousekeeperId,
                AssignId = report.AssignId,
                WorkDate = report.WorkDate,
                StartTime = report.StartTime,
                EndTime = report.EndTime,
                TotalHours = report.TotalHours,
                TaskStatus = report.TaskStatus
            });
        }
    }
}
