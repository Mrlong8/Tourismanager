using System;
using System.Collections.Generic;
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
        public static async Task<RestaurantMenuResult> GetAsync(AppDbContext db, IWebHostEnvironment environment, Location location)
        {
            var foods = await db.Foods.AsNoTracking().Include(f => f.TypeFood)
                .Include(f => f.FoodSizes).ThenInclude(s => s.Size)
                .Where(f => f.LocationId == location.LocationId).OrderBy(f => f.Name).ToListAsync();

            // Existing restaurant data always takes precedence over the reference catalog.
            if (foods.Count > 0)
            {
                return new RestaurantMenuResult
                {
                    Notice = "Giá theo thực đơn được lưu trong hệ thống. Tổng tiền món chưa gồm phí giao hàng hoặc phụ phí tại bước hóa đơn.",
                    SourceLabel = "Thực đơn của nhà hàng",
                    Groups = foods.GroupBy(f => string.IsNullOrWhiteSpace(f.TypeFood?.Name) ? "Món khác" : f.TypeFood.Name)
                        .Select(g => new RestaurantMenuGroup
                        {
                            Name = g.Key,
                            Items = g.Select(f =>
                            {
                                var size = f.FoodSizes.Where(s => s.Price > 0).OrderBy(s => s.Price).ThenBy(s => s.SizeId).FirstOrDefault();
                                return new RestaurantMenuItem
                                {
                                    Id = "db:" + f.FoodId, FoodId = f.FoodId, SizeId = size?.SizeId,
                                    Name = f.Name, Description = f.Description ?? "",
                                    ImageUrl = string.IsNullOrWhiteSpace(f.ImageUrl) ? "/Image/menu/image-unavailable.svg" : f.ImageUrl,
                                    Unit = size?.Size?.Name ?? "phần", Price = size?.Price
                                };
                            }).ToList()
                        }).ToList()
                };
            }

            var file = Path.Combine(environment.ContentRootPath, "App_Data", "restaurant-menus.v2.json");
            if (!File.Exists(file))
                return new RestaurantMenuResult { Notice = "Chưa tìm thấy dữ liệu thực đơn. Vui lòng chép thư mục App_Data của bản cập nhật vào project TourisManager." };
            var catalog = JsonSerializer.Deserialize<MenuCatalogFile>(await File.ReadAllTextAsync(file),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new MenuCatalogFile();
            var restaurantCatalog = RestaurantSeed.Load(RestaurantSeed.FindCatalogPath(environment.WebRootPath));
            var reference = restaurantCatalog.Restaurants.FirstOrDefault(r => r.Id == location.LocationId);
            string key = catalog.DefaultMenu;
            if (reference != null && catalog.BrandMenus.TryGetValue(reference.Brand, out var brandKey)) key = brandKey;
            if (catalog.RestaurantMenus.TryGetValue(location.LocationId, out var restaurantKey)) key = restaurantKey;
            if (!catalog.Menus.TryGetValue(key, out var menu)) return new RestaurantMenuResult { Notice = "Thực đơn đang được cập nhật." };
            var result = new RestaurantMenuResult
            {
                IsReference = true, Notice = menu.Notice, SourceUrl = menu.SourceUrl,
                SourceLabel = menu.SourceLabel, CheckedAt = catalog.CheckedAt
            };
            foreach (var group in menu.Groups)
            {
                var items = group.Items.Where(i => !string.IsNullOrWhiteSpace(i.Id) && !string.IsNullOrWhiteSpace(i.Name))
                    .Select(i => new RestaurantMenuItem
                    {
                        Id = "catalog:" + key + ":" + i.Id, Name = i.Name, Description = i.Description,
                        ImageUrl = i.ImageUrl, Price = i.Price > 0 ? i.Price : null,
                        Unit = i.Unit, IsIllustrative = menu.IsIllustrative
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
