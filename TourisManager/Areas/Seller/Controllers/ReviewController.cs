using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Areas.Seller.Models;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller,Admin")]
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<Location?> GetCurrentSellerLocationAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;
            return await _context.Locations.FirstOrDefaultAsync(l => l.AccountId == userId);
        }

        // GET: /Seller/Review
        [HttpGet]
        public async Task<IActionResult> Index(int? rating, string? search, string? sort = "newest", bool? hasComment = null)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null)
            {
                return RedirectToAction("CreateLocation", "Dashboard");
            }

            // Lấy tất cả đánh giá của quán này
            var allReviews = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Account)
                .Where(r => r.LocationId == location.LocationId)
                .ToListAsync();

            var totalReviews = allReviews.Count;
            var averageRating = totalReviews > 0 ? Math.Round(allReviews.Average(r => (double)r.Rating), 1) : 0;
            var count5 = allReviews.Count(r => r.Rating == 5);
            var count4 = allReviews.Count(r => r.Rating == 4);
            var count3 = allReviews.Count(r => r.Rating == 3);
            var count2 = allReviews.Count(r => r.Rating == 2);
            var count1 = allReviews.Count(r => r.Rating == 1);
            var withCommentsCount = allReviews.Count(r => !string.IsNullOrWhiteSpace(r.Content));

            // Lọc dữ liệu hiển thị
            var query = allReviews.AsEnumerable();

            if (rating.HasValue && rating.Value >= 1 && rating.Value <= 5)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            if (hasComment == true)
            {
                query = query.Where(r => !string.IsNullOrWhiteSpace(r.Content));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(r =>
                    (!string.IsNullOrEmpty(r.Content) && r.Content.ToLower().Contains(term)) ||
                    (r.Account != null && !string.IsNullOrEmpty(r.Account.Name) && r.Account.Name.ToLower().Contains(term)));
            }

            // Sắp xếp
            query = sort switch
            {
                "oldest" => query.OrderBy(r => r.CreateAt),
                "highest" => query.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreateAt),
                "lowest" => query.OrderBy(r => r.Rating).ThenByDescending(r => r.CreateAt),
                _ => query.OrderByDescending(r => r.CreateAt)
            };

            var viewModel = new ReviewListViewModel
            {
                Location = location,
                Reviews = query.ToList(),
                TotalReviews = totalReviews,
                AverageRating = averageRating,
                Count5Star = count5,
                Count4Star = count4,
                Count3Star = count3,
                Count2Star = count2,
                Count1Star = count1,
                WithCommentsCount = withCommentsCount,
                SelectedRating = rating,
                SearchTerm = search,
                SortBy = sort
            };

            ViewBag.Location = location;
            return View(viewModel);
        }

        // POST: /Seller/Review/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null)
            {
                return RedirectToAction("CreateLocation", "Dashboard");
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewId == id && r.LocationId == location.LocationId);

            if (review == null)
            {
                TempData["Error"] = "Không tìm thấy đánh giá cần xóa hoặc bạn không có quyền thao tác.";
                return RedirectToAction(nameof(Index));
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa đánh giá của khách hàng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
