using CCSystem.Infrastructure.DTOs.Report;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCSystem.Infrastructure.Infrastructures;
using CCSystem.DAL.Models;
using CCSystem.BLL.Exceptions;
using CCSystem.BLL.Utils;
using CCSystem.DAL.Repositories;
using CCSystem.BLL.Constants;

namespace CCSystem.BLL.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly UnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public async Task CreateReportAsync(ReportRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), MessageConstant.ReportMessage.EmptyRequest);
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
            { // Add the new report to the repository
                await _unitOfWork.ReportRepository.AddAsync(report);

                // Save changes to the database
                await _unitOfWork.CommitAsync();

                if (report.RecordId == 0)
                {
                    throw new InvalidOperationException(MessageConstant.ReportMessage.FailedToCreateReport);
                }
            }
            catch (BadRequestException ex)
            {
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                throw new BadRequestException(message);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }


        public async Task UpdateReportAsync(int id, ReportRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), MessageConstant.ReportMessage.EmptyRequest);
            }

            // Retrieve the report by ID
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                throw new NotFoundException(string.Format(MessageConstant.ReportMessage.ReportNotFound, id));
            }

            // Validate request properties
            if (request.HousekeeperId == 0 || request.AssignId == 0)
            {
                throw new ArgumentException(MessageConstant.ReportMessage.InvalidHousekeeperOrAssignId);
            }

            // Update report properties
            report.HousekeeperId = request.HousekeeperId;
            report.AssignId = request.AssignId;
            report.WorkDate = request.WorkDate;
            report.StartTime = request.StartTime;
            report.EndTime = request.EndTime;
            report.TotalHours = request.TotalHours;
            report.TaskStatus = request.TaskStatus;

            try
            {
                await _unitOfWork.ReportRepository.UpdateAsync(report);
                await _unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string message = ErrorUtil.GetErrorString("BadRequestException", ex.Message);
                throw new BadRequestException(message);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<bool> DeleteReportAsync(int id, string newStatus)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return false; 
            }

            await _unitOfWork.ReportRepository.DeleteAsync(id, newStatus);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<ReportResponse> GetReportByIdAsync(int id)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(id);
            if (report == null)
            {
                return null; // throw an exception based on business logic
            }
            // Map the report entity to a response DTO
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
        public async Task<IEnumerable<ReportResponse>> GetReportsByHousekeeperIdAsync(int housekeeperId)
        {
            var reports = await _unitOfWork.ReportRepository.GetReportsByHousekeeperIdAsync(housekeeperId);
            // Convert Report entities to ReportResponse DTOs
            return reports.Select(r => new ReportResponse
            {
                RecordId = r.RecordId,
                HousekeeperId = r.HousekeeperId,
                AssignId = r.AssignId,
                WorkDate = r.WorkDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                TotalHours = r.TotalHours,
                TaskStatus = r.TaskStatus
            }).ToList();
        }


        public async Task<IEnumerable<ReportResponse>> GetAllReportsAsync()
        {
            var reports = await _unitOfWork.ReportRepository.GetAllAsync();

            // Convert Report entities to ReportResponse DTOs
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
        public async Task<List<ReportResponse>> GetByAssignIdAsync(int assignId)
        {
            var reports = await _unitOfWork.ReportRepository.GetByAssignIdAsync(assignId);

            // Convert Report entities to ReportResponse DTOs
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
            }).ToList();
        }
    }
}
