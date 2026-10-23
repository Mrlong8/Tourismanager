
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Helpers;
using TourisManager.Services;
using TourisManager.Core.Interfaces;
namespace TourisManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Trang tổng quan Admin
        public async Task<IActionResult> Index()
        {
            var stats = await _adminService.GetDashboardStatsAsync();

            ViewBag.TotalUsers = stats.TotalUsers;
            ViewBag.TotalLocations = stats.TotalLocations;

            return View();
        }

        // Danh sách tài khoản có phân trang
        public async Task<IActionResult> User(int? pageNumber)
        {
            const int pageSize = 10;
            int pageIndex = pageNumber ?? 1;

            var users = await _adminService.GetUsersAsync();

            var usersQuery = users.AsQueryable();

            var paginatedUsers = await PaginatedList<Account>.CreateAsync(
                usersQuery,
                pageIndex,
                pageSize);

            return View(paginatedUsers);
        }
    }
}
