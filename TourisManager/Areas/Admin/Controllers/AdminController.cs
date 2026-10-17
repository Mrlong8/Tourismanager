using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data; // Đã khớp với namespace trong hình AppDbContext.cs
using TourisManager.Core.Entities;
using TourisManager.Helpers; // Đã khớp với namespace trong hình PaginatedList.cs
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
            //ViewBag.TotalRestaurants = await _context.Restaurants.CountAsync();

            return View();
        }
        public async Task<IActionResult> User(int? pageNumber)
        {
            int pageSize = 10; // Cố định 10 dòng/trang theo thống nhất
            int pageIndex = pageNumber ?? 1;

            // Lấy danh sách Account (truy vấn IQueryable chưa thực thi ngay)
            var usersQuery = _context.Accounts.AsNoTracking().OrderByDescending(u => u.CreateAt);

            // Gọi hàm phân trang bất đồng bộ
            var paginatedUsers = await PaginatedList<Account>.CreateAsync(usersQuery, pageIndex, pageSize);

            return View(paginatedUsers);
        }
    }
}