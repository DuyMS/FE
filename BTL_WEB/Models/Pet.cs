using System;
using System.Collections.Generic;

namespace BTL_WEB.Models;

public partial class Pet
{
    public int PetId { get; set; }

    public string Name { get; set; } = null!;

    public string Species { get; set; } = null!;

    public string? Breed { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Color { get; set; }

    public decimal? Weight { get; set; }

    public string? Description { get; set; }

    public string? HealthStatus { get; set; }

    public string? VaccinationStatus { get; set; }

    public string AdoptionStatus { get; set; } = null!;

    public int BranchId { get; set; }

    public int? OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AdoptionContract> AdoptionContracts { get; set; } = new List<AdoptionContract>();

    public virtual ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual User? Owner { get; set; }

    public virtual ICollection<PetImage> PetImages { get; set; } = new List<PetImage>();

    public virtual ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
}
