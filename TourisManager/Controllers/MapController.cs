using Microsoft.AspNetCore.Mvc;

namespace TourisManager.Controllers
{
    public class MapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
