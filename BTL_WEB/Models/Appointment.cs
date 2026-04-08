using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int UserId { get; set; }

    public int PetId { get; set; }

    public int BranchId { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Pet Pet { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
