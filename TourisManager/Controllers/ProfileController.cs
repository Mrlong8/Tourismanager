using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TourisManager.Core.Entities;
using TourisManager.Models.ViewModels;
using TourisManager.Services;

namespace TourisManager.Controllers
{
    [Route("profile")]
    public class ProfileController : Controller
    {
        
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(IAccountService accountService, IWebHostEnvironment webHostEnvironment)
        {
            _accountService = accountService;
            _webHostEnvironment = webHostEnvironment;
        }

        // xem tài khoản
        [Authorize]
        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("login","account");
            }
            var account = await _accountService.FindByIdAsync(userId);
            if (account == null)
            {
                return RedirectToAction("login","account");
            }
            return View(account);
        }

        // xử lý edit profile
        [Authorize]//Middleware bảo vệ Route. Nếu chưa đăng nhập, người dùng sẽ tự động bị chặn và đẩy về trang
        [HttpPost]
        [Route("index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Account account)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentAccount = await _accountService.FindByIdAsync(userId);

            if (currentAccount == null)
            {
                return RedirectToAction("login","account");
            }
            currentAccount.Name = account.Name;
            if (await _accountService.UpdateAsync(currentAccount))
            {
                TempData["Msg"] = "Success";
            }
            else
            {
                TempData["Msg"] = "Failed";
            }

            return RedirectToAction("index");
        }

        // xử lý ap load ảnh
        [Authorize]
        [HttpPost]
        [Route("upload-avatar")]
        public async Task<IActionResult> UploadAvataAjax(IFormFile avatarFile)
        {
            // xử lý file ảnh đầu vào có tồn tại không
            if (avatarFile == null || avatarFile.Length == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng chọn file ảnh hợp lệ"
                });
            }

            // chỉ cho chọn file ảnh
            var extension = Path.GetExtension(avatarFile.FileName).ToLower();   // lấy đuôi file 
            var allowExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };// chỉ cho phép file ảnh

            if (Array.IndexOf(allowExtensions, extension) < 0) // so sánh trả về số đuôi trùng hợp
            {
                return Json(new { success = false, message = "Định dạng file không được hỗ trợ!" });
            }

            // kiểm tra và xóa file cũ
            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currenAccound = await _accountService.FindByIdAsync(userID);
            if (currenAccound == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin tài khoản!" });
            }

            // xóa ảnh cũ 
            if (!string.IsNullOrEmpty(currenAccound.AvataUrl))
            {
                // Loại bỏ dấu '/' ở đầu đường dẫn tương đối (vd: /Image/Upload/abc.jpg -> Image/Upload/abc.jpg)
                string oldRelativePath = currenAccound.AvataUrl.TrimStart('/');
                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, oldRelativePath);
                // Kiểm tra đường dẫn và đảm bảo không xóa nhầm ảnh đại diện mặc định (avatadefault.jpg)
                if (!oldRelativePath.Contains("avatadefault.jpg") && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // taojk anhr moiws
            // Tạo tên file ngẫu nhiên bằng Guid để tránh bị trùng tên file trên Server
            string fileName = Guid.NewGuid().ToString() + extension;
            // Đường dẫn vật lý đến thư mục wwwroot/Image/Upload
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Image", "Upload");
            // nối đường dẫn
            string filePath = Path.Combine(uploadFolder, fileName); // noois file

            // ghi file vào ổ cứng
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            // Lưu đường đẫn tường đối vào database
            string newAvataUrl = $"/Image/Upload/{fileName}";
            currenAccound.AvataUrl = newAvataUrl;
            await _accountService.UpdateAsync(currenAccound);

            return Json(new { success = true, avatarUrl = newAvataUrl });

        }

        [HttpGet]
        [Route("change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("change-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (success, message) = await _accountService.ChangePasswordAsync(userId, model.NewPassword, model.CurrentPassword);

            if (success == false)
            {
                TempData["Msg"] = message;
                return View(model);
            }
            TempData["Msg"] = message;
            return RedirectToAction("ChangePassword");
        }

    }
}
