using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Models.Data;

namespace TourisManager.Controllers
{
    [Route("api/[controller]")] // định nghĩa lại đườn dãn
    [ApiController] //Là một Annotation thông báo cho ASP.NET Core biết:
                    //"Class này là một Web API chính hiệu,
                    //hãy bật các tính năng tự động cho nó".
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