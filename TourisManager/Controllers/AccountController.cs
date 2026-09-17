using Microsoft.AspNetCore.Mvc;
using TourisManager.Data;
using TourisManager.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using TourisManager.Services;
using TourisManager.Core.Entities;
using System.Timers;


namespace TourisManager.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private AccountService _accountService;

        public AccountController(AppDbContext context, AccountService accountService)
        {
 
            _accountService = accountService;
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
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (_accountService.Login(model.Email, model.Password))
            {
                HttpContext.Session.SetString("email", model.Email);
                return RedirectToAction("profile");
            }
            else
            {
                TempData["Msg"] = "failed";
                return RedirectToAction("Login");
            }
         
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("email");
            return RedirectToAction("login");
        }

        [HttpGet]
        [Route("signup")]

        public IActionResult Signup()
        {
            return View(new User());
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult Signup(User user)
        {
            // mã hóa passwort bằng thư viện BCrypt
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            if (_accountService.Create(user))
            {
                return RedirectToAction("Login");
            }
            else
            {
                TempData["Msg"] = "False";
                return RedirectToAction("Signup");
            }

           
        }

        [HttpGet]
        [Route("profile")]
        public IActionResult Profile()
        {
            string email = HttpContext.Session.GetString("email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var user = _accountService.FinebyEmail(email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            return View(user);
        }
        [HttpPost]
        [Route("profile")]
        public IActionResult Profile(User user)
        {
            var currentAccount = _accountService.FinebyEmail(user.Email);
            currentAccount.Name = user.Name;


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
    }
}
