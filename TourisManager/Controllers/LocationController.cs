using Microsoft.AspNetCore.Mvc;

namespace TourisManager.Controllers
{
    public class LocationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
