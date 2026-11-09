using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller,Admin")] // Bắt buộc tài khoản phải có quyền Seller hoặc Admin mới được truy cập
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DashboardController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // --- TRẠM KIỂM SOÁT KHI VÀO DASHBOARD ---
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1. Lấy AccountId của người đang đăng nhập (từ Cookie)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Chọc vào Database kiểm tra xem Seller này đã có Location (gian hàng) nào chưa
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.AccountId == userId);

            // 3. Nếu chưa có -> Ép buộc chuyển hướng sang trang "Thêm nhà hàng mới"
            if (location == null)
            {
                return RedirectToAction("CreateLocation");
            }

            // Thống kê dữ liệu thực tế cho Seller
            ViewBag.FoodCount = await _context.Foods.CountAsync(f => f.LocationId == location.LocationId);
            ViewBag.OrderCount = await _context.Orders.CountAsync(o => o.LocationId == location.LocationId);
            ViewBag.PendingOrderCount = await _context.Orders.CountAsync(o => o.LocationId == location.LocationId && o.OrderStatus == OrderStatus.Pending);
            ViewBag.Revenue = await _context.Orders
                .Where(o => o.LocationId == location.LocationId && o.OrderStatus == OrderStatus.Completed)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var reviews = await _context.Reviews
                .Where(r => r.LocationId == location.LocationId)
                .ToListAsync();
            ViewBag.ReviewCount = reviews.Count;
            ViewBag.AverageRating = reviews.Count > 0 ? Math.Round(reviews.Average(r => (double)r.Rating), 1) : 0;

            return View(location);
        }

        // --- CẬP NHẬT HỒ SƠ QUÁN ĂN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Location model, IFormFile? imageFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == model.LocationId && l.AccountId == userId);

            if (location == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin gian hàng!";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                location.Name = model.Name;
            }
            location.Address = model.Address;
            location.Description = model.Description;

            // Xử lý upload ảnh quán ăn nếu có tải file mới
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadDir = Path.Combine(_environment.WebRootPath, "Image", "Location");
                Directory.CreateDirectory(uploadDir);
                var extension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadDir, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                location.ImageUrl = $"/Image/Location/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                location.ImageUrl = model.ImageUrl.Trim();
            }

            // Cập nhật tọa độ ghim vị trí quán
            if (model.Latitude != 0 && model.Longitude != 0)
            {
                location.Latitude = model.Latitude;
                location.Longitude = model.Longitude;
            }

            // Đồng bộ AreaId nếu chưa có
            if (string.IsNullOrEmpty(location.AreaId) && !string.IsNullOrEmpty(location.Address))
            {
                var areas = await _context.Areas.ToListAsync();
                var matchedArea = areas.FirstOrDefault(a => location.Address.Contains(a.Name, StringComparison.OrdinalIgnoreCase));
                if (matchedArea != null)
                {
                    location.AreaId = matchedArea.AreaId;
                }
            }

            // Đồng bộ Category chuẩn "Nhà hàng"
            if (string.IsNullOrEmpty(location.CategoryId) || location.CategoryId == "CAT_DEFAULT")
            {
                var restaurantCat = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Nhà hàng")
                                 ?? await _context.Categories.FirstOrDefaultAsync();
                location.CategoryId = restaurantCat?.CategoryId ?? "fm-cat-00";
            }

            // Đảm bảo có bản ghi trong bảng LocationCategory
            var hasCategoryLink = await _context.LocationCategories.AnyAsync(lc => lc.LocationId == location.LocationId);
            if (!hasCategoryLink && !string.IsNullOrEmpty(location.CategoryId))
            {
                _context.LocationCategories.Add(new LocationCategory
                {
                    LocationId = location.LocationId,
                    CategoryId = location.CategoryId
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật hồ sơ quán ăn thành công!";

            return RedirectToAction("Index");
        }

        // --- MỞ GIAO DIỆN FORM THÊM NHÀ HÀNG ---
        [HttpGet]
        public IActionResult CreateLocation()
        {
            return View(new Location { Latitude = 21.028511m, Longitude = 105.854167m });
        }

        // --- XỬ LÝ LƯU NHÀ HÀNG VÀO DATABASE ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(Location model, IFormFile? imageFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.AccountId = userId;

            // Gán danh mục chuẩn "Nhà hàng"
            if (string.IsNullOrEmpty(model.CategoryId) || model.CategoryId == "CAT_DEFAULT")
            {
                var restaurantCat = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Nhà hàng")
                                 ?? await _context.Categories.FirstOrDefaultAsync();
                model.CategoryId = restaurantCat?.CategoryId ?? "fm-cat-00";
            }

            // Tự động gán AreaId nếu địa chỉ khớp với Tỉnh/Thành trong cơ sở dữ liệu
            if (string.IsNullOrEmpty(model.AreaId) && !string.IsNullOrEmpty(model.Address))
            {
                var areas = await _context.Areas.ToListAsync();
                var matchedArea = areas.FirstOrDefault(a => model.Address.Contains(a.Name, StringComparison.OrdinalIgnoreCase));
                if (matchedArea != null)
                {
                    model.AreaId = matchedArea.AreaId;
                }
            }

            // Đảm bảo có tọa độ mặc định nếu người dùng chưa chọn
            if (model.Latitude == 0 && model.Longitude == 0)
            {
                model.Latitude = 21.028511m;
                model.Longitude = 105.854167m;
            }

            // Xử lý upload ảnh quán ăn
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadDir = Path.Combine(_environment.WebRootPath, "Image", "Location");
                Directory.CreateDirectory(uploadDir);
                var extension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadDir, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                model.ImageUrl = $"/Image/Location/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                model.ImageUrl = model.ImageUrl.Trim();
            }

            _context.Locations.Add(model);

            // Gán liên kết danh mục
            if (!string.IsNullOrEmpty(model.CategoryId))
            {
                _context.LocationCategories.Add(new LocationCategory
                {
                    LocationId = model.LocationId,
                    CategoryId = model.CategoryId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
