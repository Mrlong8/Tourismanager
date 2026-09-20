using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Restaurant")]
    public class Restaurant
    {
        public string RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryId { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Category? Category { get; set; }
        public Account? Creator { get; set; }
    }
}