using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data; // Đã khớp với namespace trong hình AppDbContext.cs

namespace TourisManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context; // Đã đổi tên chuẩn thành AppDbContext

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy số lượng từ các DbSet khai báo trong AppDbContext
            ViewBag.TotalUsers = await _context.Accounts.CountAsync();
            ViewBag.TotalLocations = await _context.Locations.CountAsync();
            ViewBag.TotalRestaurants = await _context.Restaurants.CountAsync();

            return View();
        }
    }
}