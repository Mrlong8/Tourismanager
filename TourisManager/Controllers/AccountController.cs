using Microsoft.AspNetCore.Mvc;
using TourisManager.Data;
using TourisManager.Models.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace TourisManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                ViewBag.Error = "Tài khoản không tòn tại";
                return View(model);
            }

            if (user.Password != model.Password)
            {
                ViewBag.Error = "Mật khẩu không chính xác!";
                return View(model);
            }
            ViewBag.Message = $"Đăng nhập thành công! Xin chào {user.Name}";
            return View("Index",user);
        }


        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
    }
}
