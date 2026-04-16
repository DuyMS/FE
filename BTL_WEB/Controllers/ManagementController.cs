using BTL_WEB.Models;
using BTL_WEB.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BTL_WEB.Controllers;

public class ManagementController : Controller
{
    private readonly PetCareHubContext _context;

    public ManagementController(PetCareHubContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Pets(string? searchTerm, string? species, string? breed, int? branchId, string? status)
    {
        var petsQuery = _context.Pets
            .AsNoTracking()
            .Include(p => p.Branch)
            .Include(p => p.Owner)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim();
            petsQuery = petsQuery.Where(p => p.Name.Contains(keyword) || (p.Breed != null && p.Breed.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(species))
        {
            petsQuery = petsQuery.Where(p => p.Species == species);
        }

        if (!string.IsNullOrWhiteSpace(breed))
        {
            petsQuery = petsQuery.Where(p => p.Breed == breed);
        }

        if (branchId.HasValue && branchId.Value > 0)
        {
            petsQuery = petsQuery.Where(p => p.BranchId == branchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            petsQuery = petsQuery.Where(p => p.Status == status || p.AdoptionStatus == status);
        }

        var pets = await petsQuery
            .OrderByDescending(p => p.CreatedAt)
            .Take(40)
            .Select(p => new PetSummaryViewModel
            {
                PetId = p.PetId,
                Name = p.Name,
                Species = p.Species,
                Breed = p.Breed,
                Gender = p.Gender,
                VaccinationStatus = p.VaccinationStatus,
                AdoptionStatus = p.AdoptionStatus,
                Status = p.Status,
                BranchName = p.Branch.BranchName,
                OwnerName = p.Owner != null ? p.Owner.FullName : null
            })
            .ToListAsync();

        var petIds = pets.Select(p => p.PetId).ToList();

        var images = await _context.PetImages
            .AsNoTracking()
            .Where(i => petIds.Contains(i.PetId))
            .OrderByDescending(i => i.UploadedAt)
            .Take(60)
            .Select(i => new PetImageSummaryViewModel
            {
                ImageId = i.ImageId,
                PetId = i.PetId,
                PetName = i.Pet.Name,
                ImageUrl = i.ImageUrl,
                IsPrimary = i.IsPrimary,
                UploadedAt = i.UploadedAt
            })
            .ToListAsync();

        var vaccinations = await _context.Vaccinations
            .AsNoTracking()
            .Where(v => petIds.Contains(v.PetId))
            .OrderByDescending(v => v.VaccinationDate)
            .Take(60)
            .Select(v => new VaccinationSummaryViewModel
            {
                VaccinationId = v.VaccinationId,
                PetId = v.PetId,
                PetName = v.Pet.Name,
                VaccineName = v.VaccineName,
                VaccinationDate = v.VaccinationDate,
                NextDueDate = v.NextDueDate,
                StaffName = v.Staff.User.FullName
            })
            .ToListAsync();

        var medicalRecords = await _context.MedicalRecords
            .AsNoTracking()
            .Where(m => petIds.Contains(m.PetId))
            .OrderByDescending(m => m.VisitDate)
            .Take(60)
            .Select(m => new MedicalRecordSummaryViewModel
            {
                RecordId = m.RecordId,
                PetId = m.PetId,
                PetName = m.Pet.Name,
                VisitDate = m.VisitDate,
                Diagnosis = m.Diagnosis,
                Treatment = m.Treatment,
                StaffName = m.Staff.User.FullName
            })
            .ToListAsync();

        ViewBag.SpeciesOptions = await _context.Pets
            .AsNoTracking()
            .Select(p => p.Species)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        ViewBag.BreedOptions = await _context.Pets
            .AsNoTracking()
            .Where(p => p.Breed != null && p.Breed != string.Empty)
            .Select(p => p.Breed!)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync();

        ViewBag.BranchOptions = await _context.Branches
            .AsNoTracking()
            .OrderBy(b => b.BranchName)
            .Select(b => new { b.BranchId, b.BranchName })
            .ToListAsync();

        var model = new PetsPageViewModel
        {
            SearchTerm = searchTerm,
            Species = species,
            Breed = breed,
            BranchId = branchId,
            Status = status,
            Pets = pets,
            Images = images,
            Vaccinations = vaccinations,
            MedicalRecords = medicalRecords
        };

        return View(model);
    }

    public async Task<IActionResult> PetDetail(int id)
    {
        var pet = await _context.Pets
            .AsNoTracking()
            .Include(p => p.Branch)
            .Include(p => p.PetImages)
            .FirstOrDefaultAsync(p => p.PetId == id);

        if (pet is null)
        {
            return NotFound();
        }

        var vaccinations = await _context.Vaccinations
            .AsNoTracking()
            .Where(v => v.PetId == id)
            .OrderByDescending(v => v.VaccinationDate)
            .Take(10)
            .Select(v => new VaccinationHistoryItemViewModel
            {
                VaccineName = v.VaccineName,
                VaccinationDate = v.VaccinationDate,
                Notes = v.Notes
            })
            .ToListAsync();

        var model = new PetDetailViewModel
        {
            PetId = pet.PetId,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            Gender = pet.Gender,
            MicrochipId = $"MC-{pet.PetId:D6}",
            HealthStatus = pet.HealthStatus,
            Weight = pet.Weight,
            AdoptionStatus = pet.AdoptionStatus,
            Status = pet.Status,
            BranchName = pet.Branch.BranchName,
            BranchAddress = pet.Branch.Address,
            BranchHotline = pet.Branch.Phone,
            Description = pet.Description,
            ImageUrls = pet.PetImages
                .OrderByDescending(i => i.IsPrimary)
                .ThenByDescending(i => i.UploadedAt)
                .Select(i => i.ImageUrl)
                .ToList(),
            Vaccinations = vaccinations
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAppointment(int petId, int branchId, DateTime appointmentDateTime, int? selectedServiceId, string? notes)
    {
        var userId = await ResolveCurrentUserIdAsync();
        if (!userId.HasValue)
        {
            TempData["ErrorMessage"] = "Không xác định được người dùng hiện tại.";
            return RedirectToAction(nameof(Appointments));
        }

        var petExists = await _context.Pets.AnyAsync(p => p.PetId == petId);
        var branchExists = await _context.Branches.AnyAsync(b => b.BranchId == branchId);

        if (!petExists || !branchExists || appointmentDateTime <= DateTime.Now)
        {
            TempData["ErrorMessage"] = "Thông tin lịch hẹn không hợp lệ.";
            return RedirectToAction(nameof(Appointments));
        }

        var appointment = new Appointment
        {
            UserId = userId.Value,
            PetId = petId,
            BranchId = branchId,
            AppointmentDateTime = appointmentDateTime,
            Status = "Pending",
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            CreatedAt = DateTime.Now
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        if (selectedServiceId.HasValue && selectedServiceId.Value > 0)
        {
            var service = await _context.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServiceId == selectedServiceId.Value && s.Status == "Active");

            if (service is not null)
            {
                _context.AppointmentServices.Add(new AppointmentService
                {
                    AppointmentId = appointment.AppointmentId,
                    ServiceId = service.ServiceId,
                    Quantity = 1,
                    UnitPrice = service.Price
                });
                await _context.SaveChangesAsync();
            }
        }

        TempData["SuccessMessage"] = "Đã tạo lịch hẹn thành công.";
        return RedirectToAction(nameof(Appointments));
    }

    public async Task<IActionResult> Appointments(string? status, DateTime? date, int? selectedServiceId)
    {
        var appointmentsQuery = _context.Appointments
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Pet)
            .Include(a => a.Branch)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            appointmentsQuery = appointmentsQuery.Where(a => a.Status == status);
        }

        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            appointmentsQuery = appointmentsQuery.Where(a => a.AppointmentDateTime.Date == targetDate);
        }

        var appointments = await appointmentsQuery
            .OrderByDescending(a => a.AppointmentDateTime)
            .Take(50)
            .Select(a => new AppointmentSummaryViewModel
            {
                AppointmentId = a.AppointmentId,
                UserId = a.UserId,
                UserName = a.User.FullName,
                PetId = a.PetId,
                PetName = a.Pet.Name,
                BranchId = a.BranchId,
                BranchName = a.Branch.BranchName,
                AppointmentDateTime = a.AppointmentDateTime,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync();

        ViewBag.UserOptions = await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.FullName)
            .Take(100)
            .Select(u => new { u.UserId, u.FullName })
            .ToListAsync();

        ViewBag.PetOptions = await _context.Pets
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Take(100)
            .Select(p => new { p.PetId, p.Name })
            .ToListAsync();

        ViewBag.BranchOptions = await _context.Branches
            .AsNoTracking()
            .OrderBy(b => b.BranchName)
            .Select(b => new { b.BranchId, b.BranchName })
            .ToListAsync();

        ViewBag.ServiceOptions = await _context.Services
            .AsNoTracking()
            .Where(s => s.Status == "Active")
            .OrderBy(s => s.ServiceName)
            .Select(s => new { s.ServiceId, s.ServiceName })
            .ToListAsync();

        var appointmentIds = appointments.Select(a => a.AppointmentId).ToList();

        var appointmentServices = await _context.AppointmentServices
            .AsNoTracking()
            .Where(s => appointmentIds.Contains(s.AppointmentId))
            .OrderByDescending(s => s.AppointmentId)
            .Take(100)
            .Select(s => new AppointmentServiceSummaryViewModel
            {
                AppointmentId = s.AppointmentId,
                ServiceId = s.ServiceId,
                ServiceName = s.Service.ServiceName,
                Quantity = s.Quantity,
                UnitPrice = s.UnitPrice
            })
            .ToListAsync();

        var model = new AppointmentsPageViewModel
        {
            Status = status,
            Date = date,
            SelectedServiceId = selectedServiceId,
            Appointments = appointments,
            AppointmentServices = appointmentServices
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdoptionRequest(int petId, string message)
    {
        var userId = await ResolveCurrentUserIdAsync();
        if (!userId.HasValue)
        {
            TempData["ErrorMessage"] = "Không xác định được người dùng hiện tại.";
            return RedirectToAction(nameof(Adoptions));
        }

        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId);

        if (pet is null || pet.AdoptionStatus != "Available")
        {
            TempData["ErrorMessage"] = "Không thể tạo yêu cầu nhận nuôi với dữ liệu đã chọn.";
            return RedirectToAction(nameof(Adoptions));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập nội dung yêu cầu nhận nuôi.";
            return RedirectToAction(nameof(Adoptions));
        }

        var request = new AdoptionRequest
        {
            UserId = userId.Value,
            PetId = petId,
            RequestDate = DateTime.Now,
            Status = "Pending",
            Message = message.Trim()
        };

        _context.AdoptionRequests.Add(request);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã gửi yêu cầu nhận nuôi thành công.";
        return RedirectToAction(nameof(Adoptions));
    }

    public async Task<IActionResult> Adoptions(string? status)
    {
        var requestsQuery = _context.AdoptionRequests
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Pet)
            .Include(r => r.ReviewedByStaff)
                .ThenInclude(s => s!.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            requestsQuery = requestsQuery.Where(r => r.Status == status);
        }

        var requests = await requestsQuery
            .OrderByDescending(r => r.RequestDate)
            .Take(50)
            .Select(r => new AdoptionRequestSummaryViewModel
            {
                RequestId = r.RequestId,
                UserId = r.UserId,
                UserName = r.User.FullName,
                PetId = r.PetId,
                PetName = r.Pet.Name,
                RequestDate = r.RequestDate,
                Status = r.Status,
                ReviewedBy = r.ReviewedByStaff != null ? r.ReviewedByStaff.User.FullName : null
            })
            .ToListAsync();

        var contracts = await _context.AdoptionContracts
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Pet)
            .OrderByDescending(c => c.SignedDate)
            .Take(50)
            .Select(c => new AdoptionContractSummaryViewModel
            {
                ContractId = c.ContractId,
                RequestId = c.RequestId,
                UserId = c.UserId,
                UserName = c.User.FullName,
                PetId = c.PetId,
                PetName = c.Pet.Name,
                AdoptionFee = c.AdoptionFee,
                Status = c.Status
            })
            .ToListAsync();

        var payments = await _context.Payments
            .AsNoTracking()
            .Where(p => p.ContractId != null)
            .OrderByDescending(p => p.PaymentDate)
            .Take(50)
            .Select(p => new PaymentSummaryViewModel
            {
                PaymentId = p.PaymentId,
                AppointmentId = p.AppointmentId,
                ContractId = p.ContractId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus
            })
            .ToListAsync();

        var availablePets = await _context.Pets
            .AsNoTracking()
            .Include(p => p.Branch)
            .Include(p => p.PetImages)
            .Where(p => p.AdoptionStatus == "Available")
            .OrderByDescending(p => p.CreatedAt)
            .Take(60)
            .Select(p => new AvailablePetCardViewModel
            {
                PetId = p.PetId,
                Name = p.Name,
                Species = p.Species,
                Breed = p.Breed,
                Gender = p.Gender,
                AdoptionStatus = p.AdoptionStatus,
                BranchName = p.Branch.BranchName,
                PrimaryImageUrl = p.PetImages
                    .OrderByDescending(i => i.IsPrimary)
                    .ThenByDescending(i => i.UploadedAt)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync();

        var model = new AdoptionsPageViewModel
        {
            Status = status,
            AvailablePets = availablePets,
            Requests = requests,
            Contracts = contracts,
            Payments = payments
        };

        return View(model);
    }

    private async Task<int?> ResolveCurrentUserIdAsync()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(claimValue, out var userId))
        {
            return userId;
        }

        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Status == "Active")
            .OrderBy(u => u.UserId)
            .Select(u => (int?)u.UserId)
            .FirstOrDefaultAsync();
    }

    public async Task<IActionResult> System(string? keyword)
    {
        var usersQuery = _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var key = keyword.Trim();
            usersQuery = usersQuery.Where(u => u.Username.Contains(key) || u.FullName.Contains(key) || u.Email.Contains(key));
        }

        var users = await usersQuery
            .OrderByDescending(u => u.CreatedAt)
            .Take(60)
            .Select(u => new UserSummaryViewModel
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                RoleName = u.Role.RoleName,
                Status = u.Status
            })
            .ToListAsync();

        var staff = await _context.Staff
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Branch)
            .OrderByDescending(s => s.StaffId)
            .Take(60)
            .Select(s => new StaffSummaryViewModel
            {
                StaffId = s.StaffId,
                UserId = s.UserId,
                FullName = s.User.FullName,
                BranchId = s.BranchId,
                BranchName = s.Branch.BranchName,
                Position = s.Position,
                Status = s.Status
            })
            .ToListAsync();

        var payments = await _context.Payments
            .AsNoTracking()
            .OrderByDescending(p => p.PaymentDate)
            .Take(50)
            .Select(p => new PaymentSummaryViewModel
            {
                PaymentId = p.PaymentId,
                AppointmentId = p.AppointmentId,
                ContractId = p.ContractId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus
            })
            .ToListAsync();

        var model = new SystemPageViewModel
        {
            Keyword = keyword,
            Users = users,
            Staff = staff,
            Payments = payments
        };

        return View(model);
    }
}
