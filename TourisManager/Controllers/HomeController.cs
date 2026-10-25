using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TourisManager.Data;
using TourisManager.Core.Entities;

namespace TourisManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Truy vấn 5 địa điểm (quán ăn) từ Database
            // Bạn có thể dùng thêm .OrderByDescending(x => x.CreateAt) để lấy quán mới nhất
            var locations = await _context.Locations
                                          .Take(5)
                                          .ToListAsync();

            // Truyền danh sách dữ liệu ra View
            return View(locations);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(); // Trả về trang lỗi mặc định
        }
    }
}