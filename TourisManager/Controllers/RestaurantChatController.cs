using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data;

namespace TourisManager.Controllers
{
    [Route("RestaurantChat")]
    public sealed class RestaurantChatController : Controller
    {
        private readonly AppDbContext _db;

        public RestaurantChatController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var restaurant = await _db.Locations.AsNoTracking()
                .Where(x => x.LocationId == id)
                .Select(x => new { x.LocationId, x.Name })
                .FirstOrDefaultAsync(HttpContext.RequestAborted);

            if (restaurant == null)
                return NotFound();

            ViewData["LocationId"] = restaurant.LocationId;
            return View("Index", restaurant.Name);
        }
    }
}
