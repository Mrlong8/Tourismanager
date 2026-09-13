using Microsoft.AspNetCore.Mvc;

namespace TourisManager.Controllers
{
    public class FavoriteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
