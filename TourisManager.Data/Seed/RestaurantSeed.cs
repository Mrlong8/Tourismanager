using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;

namespace TourisManager.Data.Seed
{
    public class RestaurantCatalog
    {
        public List<RestaurantEntry> Restaurants { get; set; } = new();
        public Dictionary<string, List<CatalogMenuGroup>> BrandMenus { get; set; } = new();
    }
    public class RestaurantEntry
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Province { get; set; } = "";
        public string SourceProvince { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Description { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Hours { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string SourceUrl { get; set; } = "";
        public string MenuSourceUrl { get; set; } = "";
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<string> Categories { get; set; } = new();
    }
    public class CatalogMenuGroup
    {
        public string Name { get; set; } = "";
        public List<string> Items { get; set; } = new();
    }

    // Adds reference businesses only; never fabricates prices, reviews or seller accounts.
    public static class RestaurantSeed
    {
        public static readonly string[] CategoryNames = { "Nhà hàng", "Lẩu", "Buffet", "Hải sản", "Lẩu & Nướng", "Quán Nhậu", "Món chay", "Đặt tiệc", "Hàn Quốc", "Nhật Bản", "Món Âu", "Món Việt", "Món Thái", "Món Trung Hoa", "Tiệc cưới" };
        public static readonly string[] LegacySampleIds = Enumerable.Range(1, 20).Select(i => $"loc-{i:00}").Concat(new[] { "loc-22" }).ToArray();

        public static string FindCatalogPath(string? webRoot = null)
        {
            var candidates = new[] {
                Path.Combine(webRoot ?? "", "data", "restaurants.v1.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "restaurants.v1.json"),
                Path.Combine(AppContext.BaseDirectory, "wwwroot", "data", "restaurants.v1.json")
            };
            return candidates.FirstOrDefault(File.Exists) ?? throw new FileNotFoundException("Thiếu wwwroot/data/restaurants.v1.json. Hãy chép đủ gói FoodieMap.");
        }

        public static RestaurantCatalog Load(string path)
        {
            var catalog = JsonSerializer.Deserialize<RestaurantCatalog>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("Dữ liệu nhà hàng không hợp lệ.");
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in catalog.Restaurants)
            {
                var missingCoordinates = r.Latitude == 0 && r.Longitude == 0;
                if (!r.Id.StartsWith("fm-") || r.Id.Length > 36 || !ids.Add(r.Id)
                    || string.IsNullOrWhiteSpace(r.Name) || string.IsNullOrWhiteSpace(r.Address) || string.IsNullOrWhiteSpace(r.Province)
                    || !Uri.TryCreate(r.SourceUrl, UriKind.Absolute, out var source) || (source.Scheme != "http" && source.Scheme != "https")
                    || r.Categories.Count == 0 || r.Categories.Any(c => !CategoryNames.Contains(c))
                    || (!missingCoordinates && (r.Latitude < 8 || r.Latitude > 24 || r.Longitude < 102 || r.Longitude > 110)))
                    throw new InvalidDataException("Bản ghi nhà hàng không hợp lệ: " + r.Id);
            }
            return catalog;
        }

        public static int Import(AppDbContext db, string path)
        {
            var catalog = Load(path);
            var existing = db.Locations.Select(x => x.LocationId).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var pending = catalog.Restaurants.Where(r => !existing.Contains(r.Id)).ToList();
            if (pending.Count == 0) return 0;
            using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction() : null;
            var categories = db.Categories.ToList();
            foreach (var name in CategoryNames)
            {
                if (categories.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))) continue;
                var c = new Category { CategoryId = $"fm-cat-{Array.IndexOf(CategoryNames, name):00}", Name = name };
                db.Categories.Add(c); categories.Add(c);
            }
            var areas = db.Areas.ToList();
            foreach (var name in pending.Select(r => r.Province).Distinct())
            {
                if (areas.Any(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase))) continue;
                var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(name))).Substring(0, 16).ToLowerInvariant();
                var a = new Area { AreaId = "fm-area-" + hash, Name = name };
                db.Areas.Add(a); areas.Add(a);
            }
            foreach (var r in pending)
            {
                string CategoryId(string name) => categories.First(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)).CategoryId;
                db.Locations.Add(new Location {
                    LocationId = r.Id, Name = r.Name, Address = r.Address, Description = r.Description,
                    Latitude = r.Latitude, Longitude = r.Longitude, ImageUrl = r.ImageUrl,
                    CategoryId = CategoryId("Nhà hàng"), AccountId = null,
                    AreaId = areas.First(a => string.Equals(a.Name, r.Province, StringComparison.OrdinalIgnoreCase)).AreaId
                });
                db.LocationCategories.AddRange(r.Categories.Distinct().Select(name => new LocationCategory { LocationId = r.Id, CategoryId = CategoryId(name) }));
            }
            db.SaveChanges(); transaction?.Commit();
            return pending.Count;
        }
    }
}
