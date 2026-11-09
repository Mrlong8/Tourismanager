using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Index( string? searchString, int? pageIndex)
        {
            ViewBag.CurrentSearch = searchString;

            const int pageSize = 8;
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

        // GET: Hiển thị trang xác nhận tạo Seller hàng loạt
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateSellers()
        {
            return View();
        }

        // POST: Tạo Seller hàng loạt và phân công Location
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSellersConfirmed()
        {
            const string defaultPassword = "Seller@123";

            var result = await _userService.CreateSellersAndAssignLocationsAsync(
                defaultPassword);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
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
            ModelState.Remove(nameof(Account.BankAccounts));



            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e =>
                        $"{x.Key}: {e.ErrorMessage}"));

                ViewBag.CreateError = string.Join(" | ", errors);

                return View(user);
            }

            try
            {
                bool result = await _userService.CreateAsync(user);

                if (!result)
                {
                    ModelState.AddModelError("", "Không thể lưu tài khoản vào cơ sở dữ liệu.");
                    return View(user);
                }

                TempData["Success"] = "Tạo tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                string error = ex.InnerException?.Message ?? ex.Message;

                ModelState.AddModelError("", "Lỗi cơ sở dữ liệu: " + error);
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return View(user);
            }
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

            // Bỏ qua các thuộc tính không được nhập từ form
            ModelState.Remove(nameof(Account.BankAccounts));
            ModelState.Remove(nameof(Account.Locations));
            ModelState.Remove(nameof(Account.Reviews));
            ModelState.Remove(nameof(Account.Favorites));
            ModelState.Remove(nameof(Account.SavedFoods));
            ModelState.Remove(nameof(Account.ReservationServices));
            ModelState.Remove(nameof(Account.RoomChats));
            ModelState.Remove(nameof(Account.Orders));
            ModelState.Remove(nameof(Account.Wallet));
            ModelState.Remove(nameof(Account.DetailAccount));

            if (!ModelState.IsValid)
                return View(user);

            var existingUser = await _userService.GetByIdAsync(id);

            if (existingUser == null)
                return NotFound();

            bool result = await _userService.UpdateAsync(id, user);

            if (!result)
            {
                ModelState.AddModelError("", "Không thể cập nhật tài khoản.");
                return View(user);
            }

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
            try{
                var deleted = await _userService.DeleteAsync(id);
                 if (!deleted)
                    return NotFound();
                TempData["Success"] = "Xóa tài khoản thành công!";
            }
            catch (DbUpdateException){
                TempData["Error"] =
                    "Không thể xóa tài khoản vì dữ liệu đang được sử dụng "
                    + "hoặc có ràng buộc liên quan.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
