using System;

namespace TourisManager.Models.ViewModels
{
    public class TopRatedRestaurantViewModel
    {
        public string LocationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public string? AreaName { get; set; }
        public string CategoryName { get; set; } = "Nhà hàng";
        public double Rating { get; set; } = 5.0;
        public int ReviewCount { get; set; }
        public decimal? MinPrice { get; set; }
        public string PriceRangeDisplay { get; set; } = string.Empty;
        public int FoodCount { get; set; }
        public string HighlightBadge { get; set; } = "Yêu thích";
        public string? OpenHours { get; set; }
    }
}
