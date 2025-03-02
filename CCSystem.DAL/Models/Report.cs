﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CCSystem.DAL.Models;

public partial class Report
{
    public int RecordId { get; set; }
    public int HousekeeperId { get; set; }
    public int AssignId { get; set; }
    public DateTime WorkDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double TotalHours { get; set; }
    public string TaskStatus { get; set; }
    public virtual ScheduleAssignment Assign { get; set; }
    public virtual Account Housekeeper { get; set; }
}