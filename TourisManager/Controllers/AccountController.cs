using Microsoft.AspNetCore.Mvc;

namespace TourisManager.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
