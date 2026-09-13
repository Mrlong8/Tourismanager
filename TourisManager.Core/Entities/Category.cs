using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Category")]
    public class Category
    {
        public string CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Địa điểm" hoặc "Quán ăn"

        // Navigation Properties
        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}