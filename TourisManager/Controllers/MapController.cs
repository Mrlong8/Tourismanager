using Microsoft.AspNetCore.Mvc;
using TourisManager.Services;

namespace TourisManager.Controllers
{
    public class MapController : Controller
    {
        private readonly ILocationService _locationService;

        public MapController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public IActionResult Index()
        {
            var listLocations = _locationService.GetDataAll();
            return View(listLocations);
        }
    }
}
