using Microsoft.AspNetCore.Mvc;

namespace TourisManager.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
