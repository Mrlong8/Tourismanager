using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize] // Bắt buộc tài khoản phải đăng nhập mới được truy cập
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
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

            return View(location);
        }

        // --- CẬP NHẬT HỒ SƠ QUÁN ĂN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Location model)
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

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật hồ sơ quán ăn thành công!";

            return RedirectToAction("Index");
        }

        // --- MỞ GIAO DIỆN FORM THÊM NHÀ HÀNG ---
        [HttpGet]
        public IActionResult CreateLocation()
        {
            return View(new Location());
        }

        // --- XỬ LÝ LƯU NHÀ HÀNG VÀO DATABASE ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(Location model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.AccountId = userId;

            if (string.IsNullOrEmpty(model.CategoryId))
            {
                model.CategoryId = "CAT_DEFAULT";
            }

            _context.Locations.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
