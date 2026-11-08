using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Data;
using TourisManager.Core.Entities;
using TourisManager.Data.Seed;

namespace TourisManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? keyword, string? areaId, string? category)
        {
            keyword = keyword?.Trim();
            areaId = areaId?.Trim();
            category = category?.Trim();

            // Lấy danh sách các khu vực thực tế từ Database
            var areas = await _context.Areas.AsNoTracking().OrderBy(a => a.Name).ToListAsync();
            ViewBag.Areas = areas;
            ViewBag.Keyword = keyword;
            ViewBag.SelectedAreaId = areaId;
            ViewBag.SelectedCategory = category;

            // Xây dựng câu truy vấn cơ sở (loại trừ sample cũ)
            var baseQuery = _context.Locations
                .AsNoTracking()
                .Include(l => l.Area)
                .Where(l => !RestaurantSeed.LegacySampleIds.Contains(l.LocationId));

            // 1. Lọc theo Khu vực nếu có chọn
            if (!string.IsNullOrEmpty(areaId))
            {
                var selectedArea = areas.FirstOrDefault(a => a.AreaId == areaId);
                var areaName = selectedArea?.Name ?? "";

                baseQuery = baseQuery.Where(l => l.AreaId == areaId ||
                    (!string.IsNullOrEmpty(areaName) && l.Address != null && l.Address.Contains(areaName)));
            }

            // 2. Lọc theo Từ khóa nếu có nhập (tìm tên quán, địa chỉ, mô tả hoặc món ăn)
            if (!string.IsNullOrEmpty(keyword))
            {
                baseQuery = baseQuery.Where(l =>
                    l.Name.Contains(keyword) ||
                    (l.Address != null && l.Address.Contains(keyword)) ||
                    (l.Description != null && l.Description.Contains(keyword)) ||
                    l.Foods.Any(f => f.Name.Contains(keyword)));
            }

            // Lấy danh sách ID các quán trong phạm vi sau khi lọc khu vực và từ khóa (Scope)
            var scopedLocationIds = await baseQuery.Select(l => l.LocationId).ToListAsync();

            // Load danh mục và bảng liên kết để tính số lượng và lọc đồng bộ với trang Map
            var categories = await _context.Categories.AsNoTracking().ToDictionaryAsync(c => c.CategoryId, c => c.Name);
            var locationCategoryLinks = await _context.LocationCategories.AsNoTracking().ToListAsync();
            var primaryCategories = await _context.Locations.AsNoTracking()
                .Where(l => !RestaurantSeed.LegacySampleIds.Contains(l.LocationId))
                .Select(l => new { l.LocationId, l.CategoryId })
                .ToListAsync();

            var locationCategoriesMap = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var prim in primaryCategories)
            {
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Nhà hàng" };
                if (!string.IsNullOrEmpty(prim.CategoryId) && categories.TryGetValue(prim.CategoryId, out var name))
                {
                    set.Add(name);
                }
                locationCategoriesMap[prim.LocationId] = set;
            }

            foreach (var link in locationCategoryLinks)
            {
                if (categories.TryGetValue(link.CategoryId, out var name))
                {
                    if (locationCategoriesMap.TryGetValue(link.LocationId, out var set))
                    {
                        set.Add(name);
                    }
                    else
                    {
                        locationCategoriesMap[link.LocationId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Nhà hàng", name };
                    }
                }
            }

            // Tính số lượng quán cho từng danh mục chuẩn
            var categoryCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var catName in RestaurantSeed.CategoryNames)
            {
                categoryCounts[catName] = scopedLocationIds.Count(id => locationCategoriesMap.TryGetValue(id, out var set) && set.Contains(catName));
            }
            ViewBag.CategoryCounts = categoryCounts;
            ViewBag.CategoryNames = RestaurantSeed.CategoryNames;

            // 3. Lọc theo Danh mục nếu có chọn
            if (!string.IsNullOrEmpty(category))
            {
                var matchingIds = locationCategoriesMap
                    .Where(kvp => kvp.Value.Contains(category))
                    .Select(kvp => kvp.Key)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                baseQuery = baseQuery.Where(l => matchingIds.Contains(l.LocationId));
            }

            // Lấy các quán ăn phù hợp có tọa độ để hiển thị trên Bản đồ tương tác
            var mapLocations = await baseQuery
                .Where(l => l.Latitude >= 8 && l.Latitude <= 24 && l.Longitude >= 102 && l.Longitude <= 110)
                .Select(l => new
                {
                    name = l.Name,
                    address = l.Address ?? "Chưa cập nhật địa chỉ",
                    latitude = l.Latitude,
                    longitude = l.Longitude,
                    imageUrl = !string.IsNullOrEmpty(l.ImageUrl) ? l.ImageUrl : "/Image/default.jpg",
                    locationId = l.LocationId
                })
                .ToListAsync();

            ViewBag.MapLocations = mapLocations;

            // Lấy danh sách hiển thị cột bên phải (kết quả tìm kiếm hoặc quán mới nhất)
            bool isFiltering = !string.IsNullOrEmpty(keyword) || !string.IsNullOrEmpty(areaId) || !string.IsNullOrEmpty(category);
            ViewBag.IsFiltering = isFiltering;

            var featuredLocations = await baseQuery
                .OrderByDescending(x => x.CreateAt)
                .Take(isFiltering ? 30 : 6)
                .ToListAsync();

            return View(featuredLocations);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(); // Trả về trang lỗi mặc định
        }
    }
}