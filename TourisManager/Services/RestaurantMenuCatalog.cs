using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Data.Seed;
using TourisManager.Models;

namespace TourisManager.Services
{
    public sealed class RestaurantMenuResult
    {
        public List<RestaurantMenuGroup> Groups { get; set; } = new();
        public string Notice { get; set; } = "";
        public string SourceUrl { get; set; } = "";
        public string SourceLabel { get; set; } = "";
        public string CheckedAt { get; set; } = "";
        public bool IsReference { get; set; }
    }

    public static class RestaurantMenuCatalog
    {
        private const string SeedFoodPrefix = "fm-menu-";
        private const string ImageFallback = "/Image/menu/image-unavailable.svg";
        private const string SampleDescriptionPrefix = "Món và giá minh họa. ";
        private const string ReferenceDescriptionPrefix = "Món và giá tham khảo theo chi nhánh đã đối chiếu. ";
        private static readonly ConcurrentDictionary<string, Lazy<MenuCatalogFile>> MenuCache = new();
        private static readonly ConcurrentDictionary<string, Lazy<Dictionary<string, RestaurantEntry>>> RestaurantCache = new();

        public static RestaurantEntry? FindRestaurant(IWebHostEnvironment environment, string locationId)
        {
            var path = RestaurantSeed.FindCatalogPath(environment.WebRootPath);
            var restaurants = RestaurantCache.GetOrAdd(path, file => new Lazy<Dictionary<string, RestaurantEntry>>(
                () => RestaurantSeed.Load(file).Restaurants.ToDictionary(r => r.Id, StringComparer.OrdinalIgnoreCase))).Value;
            return restaurants.TryGetValue(locationId, out var restaurant) ? restaurant : null;
        }

        private static string NormalizeImage(string? url)
        {
            const string oldFolder = "/images/menu/";
            if (string.IsNullOrWhiteSpace(url)) return ImageFallback;
            if (url.StartsWith(oldFolder, StringComparison.OrdinalIgnoreCase))
                return "/Image/menu/" + url.Substring(oldFolder.Length);
            if (url.StartsWith("~" + oldFolder, StringComparison.OrdinalIgnoreCase))
                return "~/Image/menu/" + url.Substring(oldFolder.Length + 1);
            return url;
        }

        private static string MakeSeedFoodId(string locationId, string key, string itemId)
        {
            var value = Encoding.UTF8.GetBytes(locationId + "|" + key + "|" + itemId);
            return SeedFoodPrefix + Convert.ToHexString(SHA256.HashData(value)).Substring(0, 28).ToLowerInvariant();
        }

        public static async Task<RestaurantMenuResult> GetAsync(AppDbContext db, IWebHostEnvironment environment, Location location)
        {
            var foods = await db.Foods.AsNoTracking().Include(f => f.TypeFood)
                .Include(f => f.FoodSizes).ThenInclude(s => s.Size)
                .Where(f => f.LocationId == location.LocationId).OrderBy(f => f.Name).ToListAsync();

            var file = Path.Combine(environment.ContentRootPath, "App_Data", "restaurant-menus.v2.json");
            var catalog = File.Exists(file) ? MenuCache.GetOrAdd(file, path => new Lazy<MenuCatalogFile>(
                () => JsonSerializer.Deserialize<MenuCatalogFile>(File.ReadAllText(path),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new MenuCatalogFile())).Value : null;

            var reference = FindRestaurant(environment, location.LocationId);
            string key = catalog?.DefaultMenu ?? "";
            if (catalog != null && reference != null && catalog.BrandMenus.TryGetValue(reference.Brand, out var brandKey))
                key = brandKey;
            if (catalog != null && catalog.RestaurantMenus.TryGetValue(location.LocationId, out var restaurantKey))
                key = restaurantKey;
            var menu = catalog != null && catalog.Menus.TryGetValue(key, out var found) ? found : null;

            if (foods.Count > 0)
            {
                bool includesSeedFood = foods.Any(f => f.FoodId.StartsWith(SeedFoodPrefix, StringComparison.Ordinal));
                var originalIds = new Dictionary<string, string>(StringComparer.Ordinal);
                if (includesSeedFood && menu != null && reference != null)
                {
                    foreach (var item in menu.Groups.SelectMany(g => g.Items))
                        originalIds[MakeSeedFoodId(reference.Id, key, item.Id)] = "catalog:" + key + ":" + item.Id;
                }
                return new RestaurantMenuResult
                {
                    IsReference = includesSeedFood,
                    Notice = includesSeedFood
                        ? menu?.Notice ?? "Giá và món nhập từ dữ liệu tham khảo. Vui lòng xác nhận với nhà hàng trước khi đặt."
                        : "Giá theo thực đơn được lưu trong hệ thống. Tổng tiền món chưa gồm phí giao hàng hoặc phụ phí tại bước hóa đơn.",
                    SourceUrl = includesSeedFood ? menu?.SourceUrl ?? "" : "",
                    SourceLabel = includesSeedFood ? menu?.SourceLabel ?? "Thực đơn tham khảo" : "Thực đơn của nhà hàng",
                    CheckedAt = includesSeedFood ? catalog?.CheckedAt ?? "" : "",
                    Groups = foods.GroupBy(f => string.IsNullOrWhiteSpace(f.TypeFood?.Name) ? "Món khác" : f.TypeFood.Name)
                        .Select(g => new RestaurantMenuGroup
                        {
                            Name = g.Key,
                            Items = g.Select(f =>
                            {
                                var size = f.FoodSizes.Where(s => s.Price > 0).OrderBy(s => s.Price).ThenBy(s => s.SizeId).FirstOrDefault();
                                var description = f.Description ?? "";
                                var isSeed = f.FoodId.StartsWith(SeedFoodPrefix, StringComparison.Ordinal);
                                var hasReferencePrice = description.StartsWith(ReferenceDescriptionPrefix, StringComparison.Ordinal);
                                if (isSeed && hasReferencePrice)
                                    description = description.Substring(ReferenceDescriptionPrefix.Length);
                                else if (isSeed && description.StartsWith(SampleDescriptionPrefix, StringComparison.Ordinal))
                                    description = description.Substring(SampleDescriptionPrefix.Length);
                                return new RestaurantMenuItem
                                {
                                    Id = originalIds.TryGetValue(f.FoodId, out var originalId) ? originalId : "db:" + f.FoodId,
                                    FoodId = f.FoodId,
                                    SizeId = size?.SizeId,
                                    Name = f.Name,
                                    Description = description,
                                    ImageUrl = NormalizeImage(f.ImageUrl),
                                    Unit = size?.Size?.Name ?? "phần",
                                    Price = size?.Price,
                                    IsIllustrative = isSeed && !hasReferencePrice
                                };
                            }).ToList()
                        }).ToList()
                };
            }

            if (catalog == null)
                return new RestaurantMenuResult { Notice = "Chưa tìm thấy dữ liệu thực đơn. Vui lòng chép đủ thư mục App_Data." };
            if (menu == null)
                return new RestaurantMenuResult { Notice = "Thực đơn đang được cập nhật." };
            var result = new RestaurantMenuResult
            {
                IsReference = true,
                Notice = menu.Notice,
                SourceUrl = menu.SourceUrl,
                SourceLabel = menu.SourceLabel,
                CheckedAt = catalog.CheckedAt
            };
            foreach (var group in menu.Groups)
            {
                var items = group.Items.Where(i => !string.IsNullOrWhiteSpace(i.Id) && !string.IsNullOrWhiteSpace(i.Name))
                    .Select(i => new RestaurantMenuItem
                    {
                        Id = "catalog:" + key + ":" + i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        ImageUrl = NormalizeImage(i.ImageUrl),
                        Price = i.Price > 0 ? i.Price : null,
                        Unit = i.Unit,
                        IsIllustrative = menu.IsIllustrative
                    }).ToList();
                if (items.Count > 0) result.Groups.Add(new RestaurantMenuGroup { Name = group.Name, Items = items });
            }
            return result;
        }

        private sealed class MenuCatalogFile
        {
            public string CheckedAt { get; set; } = "";
            public string DefaultMenu { get; set; } = "vietnamese";
            public Dictionary<string, string> BrandMenus { get; set; } = new();
            public Dictionary<string, string> RestaurantMenus { get; set; } = new();
            public Dictionary<string, CatalogMenu> Menus { get; set; } = new();
        }
        private sealed class CatalogMenu
        {
            public string Notice { get; set; } = "";
            public string SourceUrl { get; set; } = "";
            public string SourceLabel { get; set; } = "";
            public bool IsIllustrative { get; set; }
            public List<CatalogGroup> Groups { get; set; } = new();
        }
        private sealed class CatalogGroup
        {
            public string Name { get; set; } = "";
            public List<CatalogItem> Items { get; set; } = new();
        }
        private sealed class CatalogItem
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public string ImageUrl { get; set; } = "";
            public decimal Price { get; set; }
            public string Unit { get; set; } = "phần";
        }
    }
}
