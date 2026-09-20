using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Helpers;
using TourisManager.Models.ViewModels;
using TourisManager.Services;

namespace TourisManager.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(AccountService accountService, IWebHostEnvironment webHostEnvironment)
        {
            _accountService = accountService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            // câu lệnh ì kiểm tra xem đã có cookie chưa
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("profile");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [Route("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var account = _accountService.Login(model.UsernameOrEmail, model.Password);
            if (account == null)
            {
                TempData["Msg"] = "Đăng Nhập thất bại";
                return RedirectToAction("login");
            }

            // nếu account != null thì đóng gói thông tin người dùng vào các thẻ dữ liệu trên
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,account.AccountId),
                new Claim(ClaimTypes.Name,account.Name),
                new Claim(ClaimTypes.Email,account.Email),
                new Claim(ClaimTypes.Role,account.Role)
            };

            // xác   thực danh tính, Tạo bộ hồ sơ danh tính chính thức cho User dựa trên Scheme Cookie.
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity); // sau khi đăng nhập

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true
                    //nghĩa là authentication cookie là persistent
                    //Tức là bạn đang nói
                    //"Hãy duy trì đăng nhập sau khi trình duyệt đóng/mở lại, theo thời hạn cookie."
                }
                );
            return RedirectToAction("profile");
        }
        [HttpPost]
        [Route("logout")]
        [ValidateAntiForgeryToken]// thủ tục bảo mật chống tấn công
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );// xóa toàn bộ cookie

            return RedirectToAction("login");
        }

        [HttpGet]
        [Route("signup")]
        public IActionResult Signup()
        {
            return View(new Account());
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult Signup(Account account)
        {
            if (!string.IsNullOrEmpty(account.Password))
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            }

            if (_accountService.Create(account))
            {
                return RedirectToAction("login");
            }

            TempData["Msg"] = "Đăng ký thất bại!";
            return RedirectToAction("signup");
        }
        [Authorize]
        [HttpGet]
        [Route("profile")]
        public IActionResult Profile()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);// lấy Id ra Hàm này sẽ tìm thẻ Claim đầu tiên có kiểu là X và trả về giá trị (dạng string) lưu bên trong thẻ đó.

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("login");
            }

            var account = _accountService.FindById(userId);

            if (account == null)
            {
                return RedirectToAction("login");
            }

            return View(account);
        }

        [Authorize]//Middleware bảo vệ Route. Nếu chưa đăng nhập, người dùng sẽ tự động bị chặn và đẩy về trang
        [HttpPost]
        [Route("profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Account account)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentAccount = _accountService.FindById(userId);

            if (currentAccount == null)
            {
                return RedirectToAction("login");
            }
            currentAccount.Name = account.Name;
            if (_accountService.Update(currentAccount))
            {
                TempData["Msg"] = "Success";
            }
            else
            {
                TempData["Msg"] = "Failed";
            }

            return RedirectToAction("profile");
        }

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
                    success = false,message = "Vui lòng chọn file ảnh hợp lệ"
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
            var currenAccound = _accountService.FindById(userID);
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
            _accountService.Update(currenAccound);

            return Json(new { success = true, avatarUrl = newAvataUrl });

        }

    }
}