using Microsoft.AspNetCore.Mvc;

namespace TourisManager.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
