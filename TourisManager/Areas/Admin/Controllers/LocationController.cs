using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LocationController : Controller
    {
        private readonly AppDbContext _context;

        public LocationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Location
        public async Task<IActionResult> Index(string searchString, string categoryId)
        {
            // 1. Lấy danh sách địa điểm bao gồm thông tin Danh mục (Category)
            var query = _context.Locations.Include(l => l.Category).AsQueryable();

            // 2. Lọc theo từ khóa tìm kiếm (Tên địa điểm hoặc Địa chỉ)
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.Name.Contains(searchString) || l.Address.Contains(searchString));
            }

            // 3. Lọc theo Danh mục
            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(l => l.CategoryId == categoryId);
            }

            // 4. Truyền danh sách Categories ra View để hiển thị trong thẻ <select>
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryId;

            var result = await query.ToListAsync();
            return View(result);
        }

        // GET: /Admin/Location/Details/id
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(x => x.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // GET: Admin/Location/Create
        // POST: /Admin/Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location location, IFormFile? imageFile)
        {
            // Bỏ qua validate các navigation properties
            ModelState.Remove(nameof(Location.CreateBy));
            ModelState.Remove(nameof(Location.Category));
            ModelState.Remove(nameof(Location.Creator));
            ModelState.Remove(nameof(Location.LocationImages));

            if (ModelState.IsValid)
            {
                // 1. Ưu tiên lấy AccountId từ Session/User đang đăng nhập, nếu không có mới lấy tài khoản đầu tiên
                var currentUserId = HttpContext.Session.GetString("AccountId"); // Hoặc User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(currentUserId))
                {
                    location.CreateBy = currentUserId;
                }
                else
                {
                    var defaultAccount = await _context.Accounts.FirstOrDefaultAsync();
                    if (defaultAccount != null)
                    {
                        location.CreateBy = defaultAccount.AccountId;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hệ thống chưa có tài khoản người dùng trong bảng Account!");
                        ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name", location.CategoryId);
                        return View(location);
                    }
                }

                location.CreateAt = DateTime.Now;

                // 2. Xử lý upload ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Location");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    location.ImageUrl = "/Image/Location/" + fileName;
                }

                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name", location.CategoryId);
            return View(location);
        }

        // GET: /Admin/Location/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name", location.CategoryId);
            return View(location);
        }

        // POST: /Admin/Location/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Location location, IFormFile? imageFile)
        {
            if (id != location.LocationId)
            {
                return NotFound();
            }

            // 1. Lấy dữ liệu cũ thực tế từ CSDL
            var oldLocation = await _context.Locations.AsNoTracking().FirstOrDefaultAsync(x => x.LocationId == id);
            if (oldLocation == null)
            {
                return NotFound();
            }

            // 2. Xử lý lưu ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Location");

                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                location.ImageUrl = "/Image/Location/" + fileName;
            }
            else
            {
                location.ImageUrl = oldLocation.ImageUrl; // Giữ nguyên đường dẫn ảnh cũ
            }

            // 3. Bảo toàn các dữ liệu lịch sử và thông tin bắt buộc không sửa ở form Edit
            location.CreateBy = oldLocation.CreateBy;
            location.CreateAt = oldLocation.CreateAt; // Giữ nguyên ngày tạo ban đầu
            location.IconUrl = oldLocation.IconUrl;   // Giữ nguyên IconUrl nếu có

            if (string.IsNullOrEmpty(location.CategoryId))
            {
                location.CategoryId = oldLocation.CategoryId;
            }

            // 4. Bỏ qua kiểm tra validate cho các thuộc tính Navigation / Lịch sử
            ModelState.Remove(nameof(Location.CreateBy));
            ModelState.Remove(nameof(Location.CategoryId));
            ModelState.Remove(nameof(Location.Category));
            ModelState.Remove(nameof(Location.Creator));
            ModelState.Remove(nameof(Location.LocationImages));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Locations.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.LocationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name", location.CategoryId);
            return View(location);
        }

        private bool LocationExists(string id)
        {
            return _context.Locations.Any(e => e.LocationId == id);
        }
        // GET: /Admin/Location/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(x => x.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: /Admin/Location/Delete/id
        // POST: /Admin/Location/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var location = await _context.Locations.FindAsync(id);

            if (location != null)
            {
                try
                {
                    _context.Locations.Remove(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Tránh tràn ứng dụng nếu địa điểm này đang có dữ liệu con (như Restaurant) tham chiếu tới
                    TempData["Error"] = "Không thể xóa địa điểm này vì đã có dữ liệu nhà hàng/dịch vụ liên quan!";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}