using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class Vaccination
{
    public int VaccinationId { get; set; }

    public int PetId { get; set; }

    public string VaccineName { get; set; } = null!;

    public DateOnly VaccinationDate { get; set; }

    public DateOnly? NextDueDate { get; set; }

    public int StaffId { get; set; }

    public string? Notes { get; set; }

    public virtual Pet Pet { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;
}
