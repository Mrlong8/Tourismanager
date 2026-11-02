using System;
using System.Collections.Generic;
using System.Linq;

namespace TourisManager.Models
{
    public record ReviewItem(string Author, int Rating, string Date, string Content);
    public class LocationExtra
    {
        public string Description { get; set; } = "Thông tin đang được cập nhật.";
        public string OpenHours { get; set; } = "Đang cập nhật";
        public List<ReviewItem> Reviews { get; set; } = new();
        public double AvgRating => Reviews.Count == 0 ? 0 : Math.Round(Reviews.Average(r => r.Rating), 1);
    }
    public class RestaurantMenuItem
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal? Price { get; set; }
        public bool CanOrder => !string.IsNullOrEmpty(Id) && Price > 0;
    }
    public class RestaurantMenuGroup
    {
        public string Name { get; set; } = "";
        public List<RestaurantMenuItem> Items { get; set; } = new();
    }
    public class LocationDetailViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string CategoryName { get; set; } = "Nhà hàng";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool HasCoordinates => Latitude >= 8 && Latitude <= 24 && Longitude >= 102 && Longitude <= 110;
        public string? ImageUrl { get; set; }
        public string Phone { get; set; } = "";
        public string SourceUrl { get; set; } = "";
        public string MenuSourceUrl { get; set; } = "";
        public bool IsReferenceMenu { get; set; }
        public List<RestaurantMenuGroup> MenuGroups { get; set; } = new();
        public LocationExtra Extra { get; set; } = new();
    }
}
