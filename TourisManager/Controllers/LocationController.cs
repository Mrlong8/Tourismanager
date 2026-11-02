using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Data.Seed;
using TourisManager.Models;

namespace TourisManager.Controllers
{
    public class LocationController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;
        public LocationController(AppDbContext db, IWebHostEnvironment environment) { _db = db; _environment = environment; }
        public IActionResult Index() => RedirectToAction("Index", "Map");
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var location = await _db.Locations.AsNoTracking().FirstOrDefaultAsync(l => l.LocationId == id);
            if (location == null) return NotFound();
            var catalog = RestaurantSeed.Load(RestaurantSeed.FindCatalogPath(_environment.WebRootPath));
            var reference = catalog.Restaurants.FirstOrDefault(r => r.Id == id);
            var foods = await _db.Foods.AsNoTracking().Include(f => f.TypeFood).Include(f => f.FoodSizes).Where(f => f.LocationId == id).ToListAsync();
            var reviews = await _db.Reviews.AsNoTracking().Include(r => r.Account).Where(r => r.LocationId == id).OrderByDescending(r => r.CreateAt).ToListAsync();
            var category = await _db.Categories.AsNoTracking().Where(c => c.CategoryId == location.CategoryId).Select(c => c.Name).FirstOrDefaultAsync();
            var model = new LocationDetailViewModel
            {
                Id = id,
                Name = location.Name,
                Address = location.Address ?? "Đang cập nhật",
                CategoryName = category ?? "Nhà hàng",
                Latitude = (double)location.Latitude,
                Longitude = (double)location.Longitude,
                ImageUrl = location.ImageUrl,
                Phone = reference?.Phone ?? "",
                SourceUrl = reference?.SourceUrl ?? "",
                MenuSourceUrl = reference?.MenuSourceUrl ?? "",
                Extra = new LocationExtra
                {
                    Description = !string.IsNullOrWhiteSpace(location.Description) ? location.Description : "Thông tin nhà hàng đang được cập nhật.",
                    OpenHours = string.IsNullOrWhiteSpace(reference?.Hours) ? "Đang cập nhật" : reference.Hours,
                    Reviews = reviews.Select(r => new ReviewItem(string.IsNullOrWhiteSpace(r.Account?.Name) ? "Thực khách" : r.Account.Name,
                        r.Rating, r.CreateAt.ToString("dd/MM/yyyy"), r.Content ?? "")).ToList()
                }
            };
            model.MenuGroups = foods.GroupBy(f => string.IsNullOrWhiteSpace(f.TypeFood?.Name) ? "Món khác" : f.TypeFood.Name)
                .Select(g => new RestaurantMenuGroup
                {
                    Name = g.Key,
                    Items = g.OrderBy(f => f.Name).Select(f => new RestaurantMenuItem
                    {
                        Id = f.FoodId,
                        Name = f.Name,
                        Description = f.Description ?? "",
                        ImageUrl = f.ImageUrl ?? "",
                        Price = f.FoodSizes.Where(s => s.Price > 0).Select(s => (decimal?)s.Price).Min()
                    }).ToList()
                }).ToList();
            if (foods.Count == 0 && reference != null && catalog.BrandMenus.TryGetValue(reference.Brand, out var groups))
            {
                model.IsReferenceMenu = true;
                model.MenuGroups = groups.Select(g => new RestaurantMenuGroup
                {
                    Name = g.Name,
                    Items = g.Items.Select(n => new RestaurantMenuItem { Name = n }).ToList()
                }).ToList();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(RestaurantReviewRequest request)
        {
            var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(accountId) || !await _db.Accounts.AnyAsync(a => a.AccountId == accountId))
                return Unauthorized(new { message = "Vui lòng đăng nhập để viết đánh giá." });
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { message = "Vui lòng chọn từ 1 đến 5 sao và nhập nội dung tối đa 500 ký tự." });
            if (!await _db.Locations.AnyAsync(l => l.LocationId == request.LocationId)) return NotFound();
            _db.Reviews.Add(new Review
            {
                LocationId = request.LocationId,
                AccountId = accountId,
                Rating = request.Rating,
                Content = request.Content.Trim(),
                CreateAt = DateTime.Now
            });
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
    public class RestaurantReviewRequest
    {
        [Required] public string LocationId { get; set; } = "";
        [Range(1, 5)] public int Rating { get; set; }
        [Required, StringLength(500)] public string Content { get; set; } = "";
    }
}
