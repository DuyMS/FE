using BTL_WEB.Models;
using BTL_WEB.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BTL_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly PetCareHubContext _context;

        public HomeController(PetCareHubContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(HomePage));
        }

        public async Task<IActionResult> HomePage()
        {
            var petsCount = await _context.Pets.AsNoTracking().CountAsync();
            var branchesCount = await _context.Branches.AsNoTracking().CountAsync();
            var staffCount = await _context.Staff.AsNoTracking().CountAsync();

            var featuredServices = await _context.Services
                .AsNoTracking()
                .Where(s => s.Status == "Active")
                .OrderBy(s => s.ServiceName)
                .Take(3)
                .Select(s => new HomeFeaturedServiceViewModel
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Description = s.Description,
                    Price = s.Price
                })
                .ToListAsync();

            var model = new HomePageViewModel
            {
                PetsCount = petsCount,
                BranchesCount = branchesCount,
                StaffCount = staffCount,
                HappyClients = Math.Max(120, petsCount * 3),
                FeaturedServices = featuredServices,
                Testimonials =
                [
                    new HomeTestimonialViewModel { CustomerName = "Ngọc Anh", Comment = "Dịch vụ rất tốt, thú cưng được chăm sóc kỹ.", Rating = 5 },
                    new HomeTestimonialViewModel { CustomerName = "Minh Khang", Comment = "Đặt lịch nhanh và nhân viên tư vấn nhiệt tình.", Rating = 5 },
                    new HomeTestimonialViewModel { CustomerName = "Bảo Trân", Comment = "Không gian sạch sẽ, bé nhà mình rất thích.", Rating = 4 }
                ]
            };

            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            username = (username ?? string.Empty).Trim();
            password = (password ?? string.Empty).Trim();

            HttpContext.Session.Remove("LoginUserId");

            if (username.Equals("user", StringComparison.OrdinalIgnoreCase) && password == "user123")
            {
                HttpContext.Session.SetString("LoginRole", "User");
                HttpContext.Session.SetString("LoginUsername", "user");

                var loginUserId = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Status == "Active" && u.Username == "user")
                    .Select(u => (int?)u.UserId)
                    .FirstOrDefaultAsync();

                if (loginUserId.HasValue)
                {
                    HttpContext.Session.SetString("LoginUserId", loginUserId.Value.ToString());
                }

                SeedProfileSession("Người dùng mẫu", "https://i.pravatar.cc/100?img=12", null, "0900000001", "user@petcarehub.local");
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl!);
                }
                return RedirectToAction(nameof(Dashboard));
            }

            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase) && password == "123")
            {
                HttpContext.Session.SetString("LoginRole", "Admin");
                HttpContext.Session.SetString("LoginUsername", "admin");

                var loginUserId = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Status == "Active" && u.Username == "admin")
                    .Select(u => (int?)u.UserId)
                    .FirstOrDefaultAsync();

                if (loginUserId.HasValue)
                {
                    HttpContext.Session.SetString("LoginUserId", loginUserId.Value.ToString());
                }

                SeedProfileSession("Quản trị viên", "https://i.pravatar.cc/100?img=5", null, "0900000002", "admin@petcarehub.local");
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl!);
                }
                return RedirectToAction("System", "Management");
            }

            TempData["LoginError"] = "Sai tài khoản hoặc mật khẩu. Tài khoản mẫu: user/user123 hoặc admin/123.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("LoginRole");
            if (string.IsNullOrWhiteSpace(role))
            {
                return RedirectToAction(nameof(Login));
            }

            if (role == "Admin")
            {
                return RedirectToAction("System", "Management");
            }

            var currentUser = await _context.Users
                .AsNoTracking()
                .Where(u => u.Status == "Active")
                .OrderBy(u => u.UserId)
                .FirstOrDefaultAsync();

            if (currentUser is null)
            {
                return RedirectToAction(nameof(Login));
            }

            var myPets = await _context.Pets
                .AsNoTracking()
                .Where(p => p.OwnerId == currentUser.UserId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .Select(p => new DashboardPetViewModel
                {
                    PetId = p.PetId,
                    Name = p.Name,
                    Species = p.Species,
                    Breed = p.Breed,
                    Status = p.Status,
                    AdoptionStatus = p.AdoptionStatus
                })
                .ToListAsync();

            var upcomingAppointments = await _context.Appointments
                .AsNoTracking()
                .Where(a => a.UserId == currentUser.UserId && a.AppointmentDateTime >= DateTime.Now)
                .OrderBy(a => a.AppointmentDateTime)
                .Take(8)
                .Select(a => new DashboardAppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDateTime = a.AppointmentDateTime,
                    PetName = a.Pet.Name,
                    Status = a.Status,
                    ServiceName = a.AppointmentServices
                        .OrderBy(s => s.Service.ServiceName)
                        .Select(s => s.Service.ServiceName)
                        .FirstOrDefault() ?? "Chưa chọn dịch vụ"
                })
                .ToListAsync();

            var model = new UserDashboardViewModel
            {
                FullName = HttpContext.Session.GetString("ProfileFullName") ?? currentUser.FullName,
                Email = HttpContext.Session.GetString("ProfileEmail") ?? currentUser.Email,
                Pets = myPets,
                UpcomingAppointments = upcomingAppointments
            };

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var role = HttpContext.Session.GetString("LoginRole");
            if (string.IsNullOrWhiteSpace(role))
            {
                return RedirectToAction(nameof(Login));
            }

            var model = new UserProfileViewModel
            {
                FullName = HttpContext.Session.GetString("ProfileFullName") ?? "",
                AvatarUrl = HttpContext.Session.GetString("ProfileAvatarUrl"),
                Phone = HttpContext.Session.GetString("ProfilePhone"),
                Email = HttpContext.Session.GetString("ProfileEmail")
            };

            var dobRaw = HttpContext.Session.GetString("ProfileDob");
            if (DateTime.TryParse(dobRaw, out var dob))
            {
                model.DateOfBirth = dob;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(UserProfileViewModel input)
        {
            var role = HttpContext.Session.GetString("LoginRole");
            if (string.IsNullOrWhiteSpace(role))
            {
                return RedirectToAction(nameof(Login));
            }

            HttpContext.Session.SetString("ProfileFullName", (input.FullName ?? string.Empty).Trim());
            HttpContext.Session.SetString("ProfileAvatarUrl", (input.AvatarUrl ?? string.Empty).Trim());
            HttpContext.Session.SetString("ProfilePhone", (input.Phone ?? string.Empty).Trim());
            HttpContext.Session.SetString("ProfileEmail", (input.Email ?? string.Empty).Trim());
            HttpContext.Session.SetString("ProfileDob", input.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty);

            TempData["ProfileSaved"] = "Đã cập nhật hồ sơ thành công.";
            return RedirectToAction(nameof(Profile));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(HomePage));
        }

        private void SeedProfileSession(string fullName, string avatarUrl, DateTime? dob, string phone, string email)
        {
            HttpContext.Session.SetString("ProfileFullName", fullName);
            HttpContext.Session.SetString("ProfileAvatarUrl", avatarUrl);
            HttpContext.Session.SetString("ProfileDob", dob?.ToString("yyyy-MM-dd") ?? string.Empty);
            HttpContext.Session.SetString("ProfilePhone", phone);
            HttpContext.Session.SetString("ProfileEmail", email);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
