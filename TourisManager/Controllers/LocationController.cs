using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Data.Seed;
using TourisManager.Models;
using TourisManager.Services;

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
            var menu = await RestaurantMenuCatalog.GetAsync(_db, _environment, location);
            model.MenuGroups = menu.Groups;
            model.IsReferenceMenu = menu.IsReference;
            model.MenuNotice = menu.Notice;
            model.MenuSourceUrl = menu.SourceUrl;
            model.MenuSourceLabel = menu.SourceLabel;
            model.MenuCheckedAt = menu.CheckedAt;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrepareCheckout([FromBody] RestaurantCheckoutRequest request)
        {
            if (request == null || !ModelState.IsValid || request.Items == null || request.Items.Count < 1 || request.Items.Count > 80
                || request.Items.Any(i => i == null || string.IsNullOrWhiteSpace(i.MenuItemId) || i.Quantity < 1 || i.Quantity > 99)
                || request.Items.Select(i => i.MenuItemId).Distinct(StringComparer.Ordinal).Count() != request.Items.Count)
                return BadRequest(new { success = false, message = "Giỏ món không hợp lệ. Mỗi món được chọn từ 1 đến 99 phần." });
            var location = await _db.Locations.AsNoTracking().FirstOrDefaultAsync(l => l.LocationId == request.LocationId);
            if (location == null) return NotFound(new { success = false, message = "Không tìm thấy nhà hàng." });
            var menu = await RestaurantMenuCatalog.GetAsync(_db, _environment, location);
            var available = menu.Groups.SelectMany(g => g.Items).Where(i => i.CanOrder).ToDictionary(i => i.Id, StringComparer.Ordinal);
            var draft = new RestaurantCheckoutDraft
            {
                LocationId = location.LocationId,
                RestaurantName = location.Name,
                RestaurantAddress = location.Address ?? ""
            };
            foreach (var line in request.Items)
            {
                if (!available.TryGetValue(line.MenuItemId, out var item))
                    return BadRequest(new { success = false, message = "Một món đã thay đổi hoặc không thuộc nhà hàng này. Hãy tải lại thực đơn." });
                draft.Items.Add(new RestaurantCheckoutDraftItem
                {
                    MenuItemId = item.Id,
                    FoodId = item.FoodId,
                    SizeId = item.SizeId,
                    Name = item.Name,
                    Unit = item.Unit,
                    ImageUrl = item.ImageUrl,
                    UnitPrice = item.Price!.Value,
                    Quantity = line.Quantity
                });
                draft.IsIllustrative |= item.IsIllustrative;
            }
            // Prices and totals come exclusively from server data, never the posted browser values.
            HttpContext.Session.SetString(RestaurantCheckoutDraft.SessionKey, JsonSerializer.Serialize(draft));
            return Json(new
            {
                success = true,
                draftId = draft.DraftId,
                redirectUrl = Url.Action("Invoice", "Shop", new { area = "", draftId = draft.DraftId }),
                draft
            });
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CheckoutDraft(string? draftId)
        {
            var json = HttpContext.Session.GetString(RestaurantCheckoutDraft.SessionKey);
            var draft = string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<RestaurantCheckoutDraft>(json);
            if (draft == null || (!string.IsNullOrWhiteSpace(draftId) && !string.Equals(draftId, draft.DraftId, StringComparison.Ordinal))
                || draft.CreatedAtUtc < DateTime.UtcNow.AddHours(-2))
                return NotFound(new { message = "Giỏ đặt món đã hết hạn. Vui lòng chọn lại món." });
            return Json(draft);
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
