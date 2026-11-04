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

        // [HttpGet] Lấy thông tin tài khoản và chi tiết tài khoản
        [Authorize]
        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("login", "account");
            }

            // Lấy account kèm theo DetailAccount
            var account = await _accountService.FindByIdAsync(userId);
            if (account == null)
            {
                return RedirectToAction("login", "account");
            }

            // Ánh xạ dữ liệu sang ViewModel
            var model = new ProfileViewModel
            {
                AccountId = account.AccountId,
                Name = account.Name,
                Email = account.Email,
                AvataUrl = account.AvataUrl,
                SDT = account.DetailAccount?.SDT,
                Address = account.DetailAccount?.Address,
                Latitude = account.DetailAccount?.Latitude,
                Longitude = account.DetailAccount?.Longitude
            };

            return View(model);
        }

        // [HttpPost] Xử lý lưu chỉnh sửa Profile & DetailAccount
        [Authorize]
        [HttpPost]
        [Route("index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("login", "account");
            }

            // Gọi hàm riêng UpdateProfileAsync để xử lý cập nhật Account và DetailAccount
            bool isUpdated = await _accountService.UpdateProfileAsync(
                userId,
                model.Name ?? "",
                model.SDT,
                model.Address,
                model.Latitude,
                model.Longitude
            );

            if (isUpdated)
            {
                TempData["Msg"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                TempData["Msg"] = "Cập nhật thất bại!";
            }

            return RedirectToAction("index");
        }

        // Xử lý upload ảnh đại diện qua Ajax
        [Authorize]
        [HttpPost]
        [Route("upload-avatar")]
        public async Task<IActionResult> UploadAvataAjax(IFormFile avatarFile)
        {
            // Xử lý file ảnh đầu vào có tồn tại không
            if (avatarFile == null || avatarFile.Length == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng chọn file ảnh hợp lệ"
                });
            }

            // Chỉ cho chọn file ảnh
            var extension = Path.GetExtension(avatarFile.FileName).ToLower();
            var allowExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            if (Array.IndexOf(allowExtensions, extension) < 0)
            {
                return Json(new { success = false, message = "Định dạng file không được hỗ trợ!" });
            }

            // Kiểm tra và xóa file cũ
            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currenAccount = await _accountService.FindByIdAsync(userID);
            if (currenAccount == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin tài khoản!" });
            }

            // Xóa ảnh cũ
            if (!string.IsNullOrEmpty(currenAccount.AvataUrl))
            {
                string oldRelativePath = currenAccount.AvataUrl.TrimStart('/');
                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, oldRelativePath);
                if (!oldRelativePath.Contains("avatadefault.jpg") && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Tạo tên file ngẫu nhiên bằng Guid để tránh bị trùng tên file trên Server
            string fileName = Guid.NewGuid().ToString() + extension;
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Image", "Upload");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string filePath = Path.Combine(uploadFolder, fileName);

            // Ghi file vào ổ cứng
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            // Lưu đường dẫn tương đối vào database thông qua UpdateAsync
            string newAvataUrl = $"/Image/Upload/{fileName}";
            currenAccount.AvataUrl = newAvataUrl;
            await _accountService.UpdateAsync(currenAccount);

            return Json(new { success = true, avatarUrl = newAvataUrl });
        }

        // Thay đổi mật khẩu
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
