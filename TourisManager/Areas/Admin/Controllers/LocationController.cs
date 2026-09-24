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
        public async Task<IActionResult> Create()
        {
            // Đảm bảo value field là "CategoryId" (kiểu string) và display field là "Name"
            ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location location, IFormFile? imageFile)
        {
            ModelState.Remove("CreateBy");
            ModelState.Remove("Category");
            ModelState.Remove("Creator");
            ModelState.Remove("LocationImages");

            if (ModelState.IsValid)
            {
                // 1. Tự động lấy AccountId đầu tiên tồn tại trong CSDL để tránh lỗi Foreign Key
                var defaultAccount = await _context.Accounts.FirstOrDefaultAsync();
                if (defaultAccount != null)
                {
                    location.CreateBy = defaultAccount.AccountId; // Hoặc Username tùy tên khóa chính bảng Account
                }
                else
                {
                    // Trường hợp bảng Account chưa có tài khoản nào
                    ModelState.AddModelError("", "Hệ thống chưa có tài khoản người dùng trong bảng Account!");
                    ViewBag.CategoryId = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name", location.CategoryId);
                    return View(location);
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
                await _context.SaveChangesAsync(); // Sẽ lưu thành công[cite: 16]
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

            // Nạp danh sách Category vào ViewBag và chọn sẵn CategoryId hiện tại của Location
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

            // 1. Lấy dữ liệu cũ trong CSDL để giữ lại các thông tin không sửa
            var oldLocation = await _context.Locations.AsNoTracking().FirstOrDefaultAsync(x => x.LocationId == id);
            if (oldLocation == null)
            {
                return NotFound();
            }

            // 2. Xử lý lưu ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                // TH1: Người dùng chọn tệp ảnh mới -> Lưu file vào máy chủ & gán đường dẫn mới
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
                // TH2: Người dùng không chọn ảnh mới -> Giữ nguyên đường dẫn ảnh cũ
                location.ImageUrl = oldLocation.ImageUrl;
            }

            // 3. Gán lại các thuộc tính bắt buộc nếu form không truyền về
            if (string.IsNullOrEmpty(location.CreateBy))
            {
                location.CreateBy = oldLocation.CreateBy;
            }
            if (string.IsNullOrEmpty(location.CategoryId))
            {
                location.CategoryId = oldLocation.CategoryId;
            }

            // 4. Bỏ qua kiểm tra validate cho các thuộc tính Navigation/Bắt buộc
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
                    await _context.SaveChangesAsync(); // Lưu dữ liệu mới (bao gồm ImageUrl) vào Database
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

            // Nếu validation không hợp lệ, nạp lại ViewBag Danh mục để View hiển thị đúng Dropdown
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