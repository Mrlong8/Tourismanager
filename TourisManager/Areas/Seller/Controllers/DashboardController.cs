using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using TourisManager.Core.Entities;
using TourisManager.Data; // Kéo thư viện Data để sử dụng AppDbContext

namespace TourisManager.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize] // Bắt buộc tài khoản phải đăng nhập mới được truy cập
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // --- TRẠM KIỂM SOÁT KHI VÀO DASHBOARD ---
        [HttpGet]
        public IActionResult Index()
        {
            // 1. Lấy AccountId của người đang đăng nhập (từ Cookie)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Chọc vào Database kiểm tra xem Seller này đã có Location (gian hàng) nào chưa
            var location = _context.Locations.FirstOrDefault(l => l.AccountId == userId);

            // 3. Nếu chưa có -> Ép buộc chuyển hướng sang trang "Thêm nhà hàng mới"
            if (location == null)
            {
                return RedirectToAction("CreateLocation");
            }

            // Nếu đã có -> Vào trang Dashboard bình thường
            return View(location);
        }

        // --- MỞ GIAO DIỆN FORM THÊM NHÀ HÀNG ---
        [HttpGet]
        public IActionResult CreateLocation()
        {
            // Khởi tạo một đối tượng Location rỗng để form HTML có thể binding (gắn kết) dữ liệu
            return View(new Location());
        }

        // --- XỬ LÝ LƯU NHÀ HÀNG VÀO DATABASE ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(Location model)
        {
            // Lấy ID tài khoản của Chủ quán hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // RẤT QUAN TRỌNG: Gắn ID của người đăng nhập vào làm chủ của gian hàng này
            model.AccountId = userId;

            // Vì bảng Location yêu cầu CategoryId (Danh mục) không được rỗng, ta tạm gán mã mặc định
            // (Sau này bạn có thể cập nhật thành Dropdown list để Chủ quán chọn danh mục thực tế)
            if (string.IsNullOrEmpty(model.CategoryId))
            {
                model.CategoryId = "CAT_DEFAULT";
            }

            // Thêm bản ghi vào DbContext và lưu xuống cơ sở dữ liệu
            _context.Locations.Add(model);
            await _context.SaveChangesAsync();

            // Tạo thành công thì tự động vòng lại trang Index. 
            // Lúc này trạm kiểm soát Index sẽ cho qua vì Database đã có dữ liệu gian hàng.
            return RedirectToAction("Index");
        }
    }
}