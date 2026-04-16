using System.ComponentModel.DataAnnotations;
using BTL_WEB.Models;
using BTL_WEB.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL_WEB.Controllers;

public class ServicesController : Controller
{
    private readonly PetCareHubContext _context;

    public ServicesController(PetCareHubContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, string? status, int page = 1, int pageSize = 10)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        var query = _context.Services
            .Include(s => s.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim();
            query = query.Where(s => s.ServiceName.Contains(keyword));
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(s => s.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s => s.Status == status);
        }

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
        if (page > totalPages)
        {
            page = totalPages;
        }

        var services = await query
            .OrderBy(s => s.ServiceName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories = await _context.ServiceCategories
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
        ViewBag.SearchTerm = searchTerm ?? string.Empty;
        ViewBag.CategoryId = categoryId;
        ViewBag.Status = status ?? string.Empty;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalItems = totalItems;

        return View(services);
    }

    [HttpGet]
    public async Task<IActionResult> Catalog()
    {
        var groups = await _context.ServiceCategories
            .AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .Select(c => new ServiceCategoryGroupViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Services = c.Services
                    .Where(s => s.Status == "Active")
                    .OrderBy(s => s.ServiceName)
                    .Select(s => new ServiceCardViewModel
                    {
                        ServiceId = s.ServiceId,
                        ServiceName = s.ServiceName,
                        Description = s.Description,
                        Price = s.Price,
                        DurationMinutes = s.DurationMinutes
                    })
                    .ToList()
            })
            .ToListAsync();

        return View(new ServiceCatalogViewModel { Groups = groups });
    }

    [HttpGet]
    public async Task<IActionResult> GetAjax(int id)
    {
        var service = await _context.Services
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.ServiceId == id);

        if (service is null)
        {
            return NotFound(new { success = false, message = "Không tìm thấy dịch vụ." });
        }

        return Json(new { success = true, data = ToRowData(service) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] ServiceAjaxInput input)
    {
        if (!ModelState.IsValid)
        {
            var message = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage
                ?? "Dữ liệu không hợp lệ.";
            return BadRequest(new { success = false, message });
        }

        var category = await _context.ServiceCategories.FindAsync(input.CategoryId);
        if (category is null)
        {
            return BadRequest(new { success = false, message = "Danh mục dịch vụ không hợp lệ." });
        }

        var service = new Service
        {
            CategoryId = input.CategoryId,
            ServiceName = input.ServiceName.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
            Price = input.Price,
            DurationMinutes = input.DurationMinutes,
            Status = input.Status.Trim()
        };

        try
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            await _context.Entry(service).Reference(s => s.Category).LoadAsync();

            return Json(new { success = true, message = "Đã thêm dịch vụ thành công.", data = ToRowData(service) });
        }
        catch (DbUpdateException)
        {
            return BadRequest(new { success = false, message = "Không thể thêm dịch vụ. Vui lòng thử lại." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAjax(int id, [FromForm] ServiceAjaxInput input)
    {
        if (!ModelState.IsValid)
        {
            var message = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage
                ?? "Dữ liệu không hợp lệ.";
            return BadRequest(new { success = false, message });
        }

        var service = await _context.Services
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.ServiceId == id);

        if (service is null)
        {
            return NotFound(new { success = false, message = "Không tìm thấy dịch vụ." });
        }

        var category = await _context.ServiceCategories.FindAsync(input.CategoryId);
        if (category is null)
        {
            return BadRequest(new { success = false, message = "Danh mục dịch vụ không hợp lệ." });
        }

        service.CategoryId = input.CategoryId;
        service.ServiceName = input.ServiceName.Trim();
        service.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
        service.Price = input.Price;
        service.DurationMinutes = input.DurationMinutes;
        service.Status = input.Status.Trim();

        try
        {
            await _context.SaveChangesAsync();
            service.Category = category;

            return Json(new { success = true, message = "Đã cập nhật dịch vụ thành công.", data = ToRowData(service) });
        }
        catch (DbUpdateException)
        {
            return BadRequest(new { success = false, message = "Không thể cập nhật dịch vụ. Vui lòng thử lại." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service is null)
        {
            return NotFound(new { success = false, message = "Không tìm thấy dịch vụ." });
        }

        try
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id, message = "Đã xóa dịch vụ thành công." });
        }
        catch (DbUpdateException)
        {
            return BadRequest(new { success = false, message = "Không thể xóa dịch vụ vì đang có dữ liệu liên quan." });
        }
    }

    private static object ToRowData(Service service)
    {
        return new
        {
            id = service.ServiceId,
            serviceName = service.ServiceName,
            categoryId = service.CategoryId,
            categoryName = service.Category.CategoryName,
            price = service.Price,
            priceText = $"{service.Price:N0} đ",
            durationMinutes = service.DurationMinutes,
            status = service.Status,
            description = service.Description ?? string.Empty
        };
    }
}

public class ServiceAjaxInput
{
    [Required(ErrorMessage = "Tên dịch vụ là bắt buộc.")]
    [StringLength(100)]
    public string ServiceName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Danh mục dịch vụ không hợp lệ.")]
    public int CategoryId { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Giá dịch vụ không hợp lệ.")]
    public decimal Price { get; set; }

    [Range(1, 1440, ErrorMessage = "Thời lượng phải lớn hơn 0 phút.")]
    public int DurationMinutes { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    [StringLength(20)]
    public string Status { get; set; } = "Active";

    [StringLength(255)]
    public string? Description { get; set; }
}
