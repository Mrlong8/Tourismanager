using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Review")]
    public class Review
    {
        [Key]
        public string ReviewId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; } = string.Empty;
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation
        public Account Account { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}