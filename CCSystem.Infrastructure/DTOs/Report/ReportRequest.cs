using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using FluentValidation;


namespace CCSystem.Infrastructure.DTOs.Report
{
    public class ReportRequest
    {
        public int HousekeeperId { get; set; }
        public int AssignId { get; set; }
        public DateOnly WorkDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public double TotalHours { get; set; }
        public string TaskStatus { get; set; } = string.Empty;
    }
}