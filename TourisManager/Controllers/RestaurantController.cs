using Microsoft.AspNetCore.Mvc;

namespace TourisManager.Controllers
{
    public class RestaurantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
