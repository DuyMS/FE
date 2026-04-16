namespace BTL_WEB.Models.ViewModels;

public class PetsPageViewModel
{
    public string? SearchTerm { get; set; }
    public string? Species { get; set; }
    public string? Breed { get; set; }
    public int? BranchId { get; set; }
    public string? Status { get; set; }
    public List<PetSummaryViewModel> Pets { get; set; } = [];
    public List<PetImageSummaryViewModel> Images { get; set; } = [];
    public List<VaccinationSummaryViewModel> Vaccinations { get; set; } = [];
    public List<MedicalRecordSummaryViewModel> MedicalRecords { get; set; } = [];
}

public class AppointmentsPageViewModel
{
    public string? Status { get; set; }
    public DateTime? Date { get; set; }
    public int? SelectedServiceId { get; set; }
    public List<AppointmentSummaryViewModel> Appointments { get; set; } = [];
    public List<AppointmentServiceSummaryViewModel> AppointmentServices { get; set; } = [];
}

public class AdoptionsPageViewModel
{
    public string? Status { get; set; }
    public List<AvailablePetCardViewModel> AvailablePets { get; set; } = [];
    public List<AdoptionRequestSummaryViewModel> Requests { get; set; } = [];
    public List<AdoptionContractSummaryViewModel> Contracts { get; set; } = [];
    public List<PaymentSummaryViewModel> Payments { get; set; } = [];
}

public class AvailablePetCardViewModel
{
    public int PetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Gender { get; set; }
    public string AdoptionStatus { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string? PrimaryImageUrl { get; set; }
}

public class PetDetailViewModel
{
    public int PetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Gender { get; set; }
    public string MicrochipId { get; set; } = string.Empty;
    public string? HealthStatus { get; set; }
    public decimal? Weight { get; set; }
    public string AdoptionStatus { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string BranchAddress { get; set; } = string.Empty;
    public string? BranchHotline { get; set; }
    public string? Description { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public List<VaccinationHistoryItemViewModel> Vaccinations { get; set; } = [];
}

public class VaccinationHistoryItemViewModel
{
    public string VaccineName { get; set; } = string.Empty;
    public DateOnly VaccinationDate { get; set; }
    public string? Notes { get; set; }
}

public class ServiceCatalogViewModel
{
    public List<ServiceCategoryGroupViewModel> Groups { get; set; } = [];
}

public class ServiceCategoryGroupViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<ServiceCardViewModel> Services { get; set; } = [];
}

public class ServiceCardViewModel
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
}

public class SystemPageViewModel
{
    public string? Keyword { get; set; }
    public List<UserSummaryViewModel> Users { get; set; } = [];
    public List<StaffSummaryViewModel> Staff { get; set; } = [];
    public List<PaymentSummaryViewModel> Payments { get; set; } = [];
}

public class PetSummaryViewModel
{
    public int PetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Gender { get; set; }
    public string? VaccinationStatus { get; set; }
    public string AdoptionStatus { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string? OwnerName { get; set; }
}

public class PetImageSummaryViewModel
{
    public int ImageId { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class VaccinationSummaryViewModel
{
    public int VaccinationId { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string VaccineName { get; set; } = string.Empty;
    public DateOnly VaccinationDate { get; set; }
    public DateOnly? NextDueDate { get; set; }
    public string StaffName { get; set; } = string.Empty;
}

public class MedicalRecordSummaryViewModel
{
    public int RecordId { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public string StaffName { get; set; } = string.Empty;
}

public class AppointmentSummaryViewModel
{
    public int AppointmentId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public DateTime AppointmentDateTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class AppointmentServiceSummaryViewModel
{
    public int AppointmentId { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class AdoptionRequestSummaryViewModel
{
    public int RequestId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ReviewedBy { get; set; }
}

public class AdoptionContractSummaryViewModel
{
    public int ContractId { get; set; }
    public int RequestId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public decimal AdoptionFee { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PaymentSummaryViewModel
{
    public int PaymentId { get; set; }
    public int? AppointmentId { get; set; }
    public int? ContractId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
}

public class UserSummaryViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class StaffSummaryViewModel
{
    public int StaffId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
