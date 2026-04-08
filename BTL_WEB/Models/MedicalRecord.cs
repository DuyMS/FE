using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class MedicalRecord
{
    public int RecordId { get; set; }

    public int PetId { get; set; }

    public int StaffId { get; set; }

    public DateTime VisitDate { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Notes { get; set; }

    public virtual Pet Pet { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;
}
