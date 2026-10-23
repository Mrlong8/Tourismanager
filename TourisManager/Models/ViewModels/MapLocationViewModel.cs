using System.Collections.Generic;

namespace TourisManager.Models.ViewModels
{
    public class MapLocationViewModel
    {
        public string LocationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string AreaId { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public List<string> CategoryNames { get; set; } = new List<string>();
        public bool HasMenu { get; set; }
    }
}
