using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Review")]
    public class Review
    {
        public string ReviewId { get; set; }
        public string TargetType { get; set; } = string.Empty; // "Location" hoặc "Restaurant"
        public string TargetId { get; set; } // LocationId hoặc RestaurantId tương ứng
        public string UserId { get; set; }
        public int Rating { get; set; } // Điểm đánh giá (1-5 sao)
        public string? Content { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public User? User { get; set; }
        public ICollection<ReviewContent> ReviewContents { get; set; } = new List<ReviewContent>();
    }
}