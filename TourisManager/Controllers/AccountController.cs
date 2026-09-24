using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public async Task<IActionResult> Login() // Cần đổi thành async Task vì gọi SignOutAsync
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                // Lấy Id từ Cookie
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Kiểm tra xem Id này có tồn tại trong CSDL không
                var account = _accountService.FindById(userId);
                if (account != null)
                {
                    return RedirectToAction("profile"); // Có trong CSDL thì vào Profile
                }
                else
                {
                    // Có Cookie nhưng không có trong CSDL (lỗi đồng bộ) -> Xóa Cookie
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
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

        // gọi tới đănh nhập băng google
        [HttpGet]
        [Route("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", "Account")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        // hàm xử lý
        public async Task<IActionResult> GoogleResponse()
        {
            // kiểm tra xem xác thực thành công hay chưa
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return RedirectToAction("Login", new { error = "Lỗi đăng nhập"});
            }
            // lấy danh sách thông tin mà google gửi về
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

            // Trích xuất từng trường dữ liệu cụ thể từ Claims (Nếu không có sẽ nhận giá trị rỗng "")
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "";
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
            var providerKey = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? ""; // ID tài khoản phía Google


            // Nếu không lấy được Email (trường quan trọng nhất), hủy đăng nhập và về lại trang Login
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", new { error = "Không lấy được Email từ Google!" });
            }
            //  Kiểm tra trong CSDL xem Email này đã tồn tại trong hệ thống chưa
            var account = _accountService.FindByUsernameOrEmail(email);
            if (account == null)
            {
                // TRƯỜNG HỢP 1: Tài khoản chưa từng tồn tại -> Khởi tạo một Account mới
                account = new Account
                {
                    Email = email,
                    Username = email.Split('@')[0], // Lấy phần chữ trước ký tự '@' làm Username (VD: abc@gmail.com -> username: abc)
                    Name = string.IsNullOrEmpty(name) ? email : name,
                    AuthProvider = "Google",
                    ProviderKey = providerKey,
                    Role = "User",
                    Password = "" // Đăng nhập Google nên để Password trống (null)
                };

                // Gọi Service lưu Account mới vào Database (AccountServiceImpl sẽ tự gán AccountId = acc-xx)
                bool isCreated = _accountService.Create(account);
                Console.Write(isCreated);
                if (!isCreated)
                {
                    return RedirectToAction("Login", new { error = "Lỗi hệ thống: Không thể lưu tài khoản vào CSDL!" });
                }
            }
            else
            {
                // TRƯỜNG HỢP 2: Tài khoản đã tồn tại trong CSDL -> Cập nhật bổ sung thông tin Google (nếu thiếu)
                bool needUpdate = false;

                // Nếu tài khoản cũ chưa gán ProviderKey thì bổ sung vào
                if (string.IsNullOrEmpty(account.ProviderKey))
                {
                    account.ProviderKey = providerKey;
                    account.AuthProvider = "Google";
                    needUpdate = true;
                }

                // Nếu có sự thay đổi thì gọi Service để lưu bản ghi cập nhật vào Database
                if (needUpdate)
                {
                    _accountService.Update(account);
                }
            }
            // BƯỚC 4: Tạo Cookie xác thực đăng nhập phiên làm việc cho ứng dụng của bạn
            // Tạo danh sách Claims nội bộ để ứng dụng nhận diện User đã đăng nhập thành công
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId ?? ""), // Lưu ID người dùng (VD: acc-01)
                new Claim(ClaimTypes.Name, account.Name ?? account.Username ?? ""), // Lưu tên hiển thị
                new Claim(ClaimTypes.Email, account.Email ?? ""), // Lưu Email
                new Claim(ClaimTypes.Role, account.Role ?? "User"), // Lưu Quyền (Role)
            };

            // Đóng gói danh sách Claims vào danh tính (ClaimsIdentity) với sơ đồ Cookie

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Ghi Cookie đăng nhập vào Trình duyệt của người dùng
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // BƯỚC 5: Đăng nhập hoàn tất, chuyển hướng người dùng về trang chủ (Home/Index)
            return RedirectToAction("Profile", "Account");
        }

        [HttpGet]
        [Route("signup")]
        public IActionResult Signup()
        {
            return View(new SignupViewModel());
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //  KIỂM TRA TRÙNG username  TRONG DATABASE
            if (_accountService.IsUserNameExists(model.Username))
            {
                ModelState.AddModelError("Username", "Tên tài khoản này đã được sử dụng.");
            }

            // KIỂM TRA TRÙNG EMAIL TRONG DATABASE
            if (_accountService.IsUserEmailExists(model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký.");
            }

         
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = new Account
            {
                Username = model.Username,
                Name = model.FullName,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };


            if (_accountService.Create(account))
            {
                return RedirectToAction("login");
            }

            TempData["Msg"] = "Đăng ký thất bại!";
            return RedirectToAction("login");
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