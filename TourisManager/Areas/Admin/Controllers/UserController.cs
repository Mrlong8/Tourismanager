using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Helpers;   
namespace TourisManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/User


        public async Task<IActionResult> Index(string? searchString, int? pageIndex)
        {
            ViewBag.CurrentSearch = searchString;

            int pageSize = 3;
            int pageNumber = pageIndex ?? 1;

            var usersQuery = _context.Accounts.AsNoTracking();

            // Tìm kiếm theo Username, Name hoặc Email
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string keyword = searchString.Trim();

                usersQuery = usersQuery.Where(u =>
                    (u.Username != null && u.Username.Contains(keyword)) ||
                    u.Name.Contains(keyword) ||
                    u.Email.Contains(keyword));
            }

            // Sắp xếp tài khoản mới nhất lên đầu
            usersQuery = usersQuery.OrderByDescending(u => u.CreateAt);

            // Phân trang sau khi tìm kiếm
            var pagedData = await PaginatedList<Account>.CreateAsync(
                usersQuery,
                pageNumber,
                pageSize);

            return View(pagedData);
        }


        // GET: /Admin/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account user)
        {
            // Tạo AccountId trước
            user.AccountId = Guid.NewGuid().ToString();

            // Bỏ lỗi validation của AccountId
            ModelState.Remove(nameof(Account.AccountId));

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(user.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }
                _context.Accounts.Add(user);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }


        // GET: /Admin/User/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _context.Accounts.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /Admin/User/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Account user)
        {
            if (string.IsNullOrWhiteSpace(id) || id != user.AccountId)
            {
                return NotFound();
            }

            // Lấy tài khoản hiện có trong database
            var existingUser = await _context.Accounts.FindAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Chỉ cập nhật các trường được phép sửa
            if (ModelState.IsValid)
            {
                existingUser.Username = user.Username;
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role;
                existingUser.AvataUrl = user.AvataUrl;
                existingUser.AuthProvider = user.AuthProvider;

                // Chỉ đổi mật khẩu nếu người quản trị nhập mật khẩu mới
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    existingUser.Password =
                        BCrypt.Net.BCrypt.HashPassword(user.Password);
                }

                // Giữ nguyên Password, CreateAt và thông tin đăng nhập ngoài form
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }


        // GET: /Admin/User/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Accounts
                .FirstOrDefaultAsync(x => x.AccountId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // POST: /Admin/User/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _context.Accounts.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                _context.Accounts.Remove(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa tài khoản thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Không thể xóa tài khoản vì dữ liệu đang được sử dụng hoặc có ràng buộc liên quan.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}