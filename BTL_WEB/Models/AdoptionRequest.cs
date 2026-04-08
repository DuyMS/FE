using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class AdoptionRequest
{
    public int RequestId { get; set; }

    public int UserId { get; set; }

    public int PetId { get; set; }

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Message { get; set; }

    public int? ReviewedByStaffId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public virtual AdoptionContract? AdoptionContract { get; set; }

    public virtual Pet Pet { get; set; } = null!;

    public virtual Staff? ReviewedByStaff { get; set; }

    public virtual User User { get; set; } = null!;
}
