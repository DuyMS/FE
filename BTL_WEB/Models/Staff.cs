using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public int UserId { get; set; }

    public int BranchId { get; set; }

    public string Position { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public decimal? Salary { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
}
