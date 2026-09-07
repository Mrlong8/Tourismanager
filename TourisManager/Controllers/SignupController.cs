using Microsoft.AspNetCore.Mvc;

namespace TourisManager.Controllers
{
    public class SignupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
