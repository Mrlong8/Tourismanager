using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data;
using TourisManager.Core.Entities;

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
        public async Task<IActionResult> Index()
        {
            var locations = await _context.Locations.ToListAsync();
            return View(locations);
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

        // GET: /Admin/Location/Create
        public IActionResult Create()
        {
            return View();
        }
        //Post: /Admin/Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location location)
        {
            // 1. Tự sinh LocationId nếu chưa có
            if (string.IsNullOrEmpty(location.LocationId))
            {
                location.LocationId = "loc-" + Guid.NewGuid().ToString().Substring(0, 5);
            }

            // 2. Gán CreateBy bằng AccountId hợp lệ ĐÃ CÓ sẵn trong bảng Account
            // Kiểm tra trong trang User của bạn, ví dụ "acc-01" hoặc id của tài khoản admin
            if (string.IsNullOrEmpty(location.CreateBy))
            {
                location.CreateBy = "acc-01"; // Thay "acc-01" bằng một AccountId có trong CSDL của bạn
            }

            // 3. Gán CategoryId bằng mã Category hợp lệ đã có trong bảng Category (nếu có khóa ngoại)
            if (string.IsNullOrEmpty(location.CategoryId))
            {
                // Nhớ thay bằng mã CategoryId thực tế đã tồn tại trong DB của bạn (ví dụ: "cat-01" hoặc "1")
                location.CategoryId = "cat-01";
            }

            // 4. Bỏ qua Validate cho các trường đã tự gán
            ModelState.Remove(nameof(Location.LocationId));
            ModelState.Remove(nameof(Location.CreateBy));
            ModelState.Remove(nameof(Location.CategoryId));

            if (ModelState.IsValid)
            {
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(location);
        }

        // GET: /Admin/Location/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: /Admin/Location/Edit/id
        // POST: /Admin/Location/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Location location)
        {
            if (id != location.LocationId)
            {
                return NotFound();
            }

            // 1. Lấy dữ liệu cũ trong CSDL để giữ nguyên các thông tin không sửa (như CreateBy, CategoryId, Ngày tạo)
            var oldLocation = await _context.Locations.AsNoTracking().FirstOrDefaultAsync(x => x.LocationId == id);
            if (oldLocation == null)
            {
                return NotFound();
            }

            // 2. Gán lại các thuộc tính bắt buộc nếu form không truyền về
            if (string.IsNullOrEmpty(location.CreateBy))
            {
                location.CreateBy = oldLocation.CreateBy;
            }
            if (string.IsNullOrEmpty(location.CategoryId))
            {
                location.CategoryId = oldLocation.CategoryId;
            }

            // 3. Bỏ qua kiểm tra validate cho các trường này
            ModelState.Remove(nameof(Location.CreateBy));
            ModelState.Remove(nameof(Location.CategoryId));

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