﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CCSystem.DAL.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int CustomerId { get; set; }

    public int DetailId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual Account Customer { get; set; }

    public virtual BookingDetail Detail { get; set; }
}