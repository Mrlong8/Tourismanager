using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Location")]
    public class Location
    {
        public string LocationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryId { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? ImageUrl { get; set; }
        public string? IconUrl { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Category? Category { get; set; }
        public User? Creator { get; set; }
        public ICollection<LocationImage> LocationImages { get; set; } = new List<LocationImage>();
    }
}