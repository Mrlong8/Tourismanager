
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data;

namespace TourisManager.ViewComponents
{
    public class AdminSidebarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public AdminSidebarViewComponent(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync(string item)
        {
            int total = 0;

            if (item == "Users")
            {
                total = await _context.Accounts.CountAsync();
            }
            else if (item == "Locations")
            {
                total = await _context.Locations.CountAsync();
            }

            return View("Default", total);
        }


    }

    public class AdminSidebarViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalLocations { get; set; }
    }
}
