using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data;
using TourisManager.Models.ViewModels;

namespace TourisManager.Controllers
{
    public class MapController : Controller
    {
        private readonly AppDbContext _db;

        public MapController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var locations = await _db.Locations
                .AsNoTracking()
                .OrderBy(location => location.Name)
                .Select(location => new MapLocationViewModel
                {
                    LocationId = location.LocationId,
                    Name = location.Name ?? string.Empty,
                    Address = location.Address ?? string.Empty,
                    ImageUrl = location.ImageUrl ?? string.Empty,
                    AreaId = location.AreaId ?? string.Empty,
                    AreaName = location.Area != null ? location.Area.Name : string.Empty,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    CategoryNames = _db.LocationCategories
                        .Where(link => link.LocationId == location.LocationId)
                        .Select(link => link.Category.Name)
                        .ToList(),
                    HasMenu = _db.Foods.Any(food => food.LocationId == location.LocationId)
                })
                .ToListAsync();

            return View(locations);
        }
    }
}
