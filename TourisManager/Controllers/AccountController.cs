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
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IAccountService accountService, IWebHostEnvironment webHostEnvironment)
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
                string? userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Kiểm tra xem Id này có tồn tại trong CSDL không
                var account = await _accountService.FindByIdAsync(userId);
                if (account != null)
                {
                    return RedirectToAction("index","profile"); // Có trong CSDL thì vào Profile
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
            var account = await _accountService.LoginAsync(model.UsernameOrEmail, model.Password);
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
            return RedirectToAction("index", "profile");
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
            var account = await _accountService.FindByUsernameOrEmailAsync(email);
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
                bool isCreated = await _accountService.CreateAsync(account);
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
                    await _accountService.UpdateAsync(account);
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
            return RedirectToAction("index", "profile");
        }

        [HttpGet]
        [Route("signup")]
        public IActionResult Signup()
        {
            return View(new SignupViewModel());
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //  KIỂM TRA TRÙNG username  TRONG DATABASE
            if (await _accountService.IsUserNameExistsAsync(model.Username))
            {
                ModelState.AddModelError("Username", "Tên tài khoản này đã được sử dụng.");
            }

            // KIỂM TRA TRÙNG EMAIL TRONG DATABASE
            if (await _accountService.IsUserEmailExistsAsync(model.Email))
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


            if (await _accountService.CreateAsync(account))
            {
                return RedirectToAction("login");
            }

            TempData["Msg"] = "Đăng ký thất bại!";
            return RedirectToAction("login");
        }
     


    }
}