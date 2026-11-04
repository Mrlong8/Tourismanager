
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Helpers;
using TourisManager.Services;
using TourisManager.Core.Interfaces;
namespace TourisManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Danh sách tài khoản, tìm kiếm và phân trang
        public async Task<IActionResult> Index(
    string? searchString,
    int? pageIndex)
        {
            ViewBag.CurrentSearch = searchString;

            const int pageSize = 3;
            int pageNumber = Math.Max(pageIndex ?? 1, 1);

            var result = await _userService.GetUsersAsync(
                searchString,
                pageNumber,
                pageSize);

            var pagedData = new PaginatedList<Account>(
                result.Users,
                result.TotalCount,
                pageNumber,
                pageSize);

            return View(pagedData);
        }

        // GET: Tạo tài khoản
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tạo tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account user)
        {
            ModelState.Remove(nameof(Account.AccountId));

            if (!ModelState.IsValid)
                return View(user);

            await _userService.CreateAsync(user);

            TempData["Success"] = "Tạo tài khoản thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Sửa tài khoản
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Sửa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Account user)
        {
            if (string.IsNullOrWhiteSpace(id) || id != user.AccountId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(user);

            var existingUser = await _userService.GetByIdAsync(id);

            if (existingUser == null)
                return NotFound();

            await _userService.UpdateAsync(id, user);

            TempData["Success"] = "Cập nhật tài khoản thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Xác nhận xóa tài khoản
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Xóa tài khoản
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            try
            {
                var deleted = await _userService.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                TempData["Success"] = "Xóa tài khoản thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Không thể xóa tài khoản vì dữ liệu đang được sử dụng "
                    + "hoặc có ràng buộc liên quan.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
