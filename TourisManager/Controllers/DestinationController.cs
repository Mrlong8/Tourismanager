using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Models;

namespace TourisManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
        private readonly MyDbContext _context;

        public DestinationController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Destination
        [HttpGet]
        public async Task<IActionResult> GetDestination()
        {
            var data = await _context.Destinations.ToListAsync();
            return Ok(data);
        }
    }
}