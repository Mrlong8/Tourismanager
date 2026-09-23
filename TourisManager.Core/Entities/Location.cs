using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Location")]
    public class Location
    {
        public string LocationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string? Address { get; set; }
        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
        public string? ImageUrl { get; set; }
        public string? IconUrl { get; set; }

        [ForeignKey(nameof(Creator))]
        public string? CreateBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation Properties (Không dùng [ValidateNever] ở đây)
        public Category? Category { get; set; }
        public Account? Creator { get; set; }
        public ICollection<LocationImage> LocationImages { get; set; } = new List<LocationImage>();
    }
}