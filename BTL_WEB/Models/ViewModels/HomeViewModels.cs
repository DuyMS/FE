namespace BTL_WEB.Models.ViewModels;

public class HomePageViewModel
{
    public int PetsCount { get; set; }
    public int BranchesCount { get; set; }
    public int StaffCount { get; set; }
    public int HappyClients { get; set; }
    public List<HomeFeaturedServiceViewModel> FeaturedServices { get; set; } = [];
    public List<HomeTestimonialViewModel> Testimonials { get; set; } = [];
}

public class HomeFeaturedServiceViewModel
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}

public class HomeTestimonialViewModel
{
    public string CustomerName { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
}

public class UserDashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<DashboardPetViewModel> Pets { get; set; } = [];
    public List<DashboardAppointmentViewModel> UpcomingAppointments { get; set; } = [];
}

public class DashboardPetViewModel
{
    public int PetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AdoptionStatus { get; set; } = string.Empty;
}

public class DashboardAppointmentViewModel
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class UserProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
