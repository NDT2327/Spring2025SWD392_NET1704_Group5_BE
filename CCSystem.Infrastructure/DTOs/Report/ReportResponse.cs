using System;
using CCSystem.DAL.Models;

namespace CCSystem.Infrastructure.DTOs.Report
{
    public class ReportResponse
    {
        public int RecordId { get; set; }
        public int HousekeeperId { get; set; }
        public int AssignId { get; set; }
        public DateOnly WorkDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public double TotalHours { get; set; }
        public string TaskStatus { get; set; } = string.Empty;
    }
}
