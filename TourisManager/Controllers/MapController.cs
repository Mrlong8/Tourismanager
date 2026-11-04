using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data;
using TourisManager.Data.Seed;
using TourisManager.Models.ViewModels;

namespace TourisManager.Controllers
{
    public class MapController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;
        public MapController(AppDbContext db, IWebHostEnvironment environment) { _db = db; _environment = environment; }

        public async Task<IActionResult> Index()
        {
            var catalog = RestaurantSeed.Load(RestaurantSeed.FindCatalogPath(_environment.WebRootPath));
            var references = catalog.Restaurants.ToDictionary(r => r.Id);
            var locations = await _db.Locations.AsNoTracking().Include(l => l.Area)
                .Where(l => !RestaurantSeed.LegacySampleIds.Contains(l.LocationId)).ToListAsync();
            var categories = await _db.Categories.AsNoTracking().ToDictionaryAsync(c => c.CategoryId, c => c.Name);
            var links = await _db.LocationCategories.AsNoTracking().ToListAsync();
            var foods = await _db.Foods.AsNoTracking().Select(f => new { f.LocationId, f.Name }).ToListAsync();
            var result = new List<MapLocationViewModel>();
            foreach (var location in locations)
            {
                var names = links.Where(l => l.LocationId == location.LocationId && categories.ContainsKey(l.CategoryId)).Select(l => categories[l.CategoryId]).ToList();
                if (categories.TryGetValue(location.CategoryId, out var primary)) names.Add(primary);
                names = names.Where(n => RestaurantSeed.CategoryNames.Contains(n, StringComparer.OrdinalIgnoreCase)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var menu = foods.Where(f => f.LocationId == location.LocationId).Select(f => f.Name).ToList();
                if (!location.LocationId.StartsWith("fm-") && names.Count == 0 && menu.Count == 0) continue;
                if (!names.Contains("Nhà hàng", StringComparer.OrdinalIgnoreCase)) names.Insert(0, "Nhà hàng");
                references.TryGetValue(location.LocationId, out var reference);
                if (reference != null && catalog.BrandMenus.TryGetValue(reference.Brand, out var groups)) menu.AddRange(groups.SelectMany(g => g.Items));
                bool valid = location.Latitude >= 8 && location.Latitude <= 24 && location.Longitude >= 102 && location.Longitude <= 110;
                result.Add(new MapLocationViewModel
                {
                    LocationId = location.LocationId,
                    Name = location.Name,
                    Address = location.Address ?? "",
                    ImageUrl = location.ImageUrl ?? "",
                    AreaId = location.AreaId ?? "",
                    AreaName = location.Area?.Name ?? reference?.Province ?? "",
                    CategoryId = location.CategoryId,
                    Latitude = valid ? location.Latitude : null,
                    Longitude = valid ? location.Longitude : null,
                    CategoryNames = names,
                    HasMenu = menu.Count > 0,
                    MenuNames = menu.Distinct().ToList()
                });
            }
            return View(result.OrderBy(r => r.Name).ToList());
        }
    }
}
