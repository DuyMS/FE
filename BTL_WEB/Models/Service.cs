using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public int CategoryId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual ServiceCategory Category { get; set; } = null!;
}
