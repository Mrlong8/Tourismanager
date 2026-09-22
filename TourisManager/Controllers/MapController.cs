using Microsoft.AspNetCore.Mvc;
using TourisManager.Services;

namespace TourisManager.Controllers
{
    public class MapController : Controller
    {
        private readonly LocationService _locationService;

        public MapController(LocationService locationService)
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
