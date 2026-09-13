using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Favorite")]
    public class Favorite
    {
        public string FavoriteId { get; set; }
        public string UserId { get; set; }
        public string TargetType { get; set; } = string.Empty; // "Location" hoặc "Restaurant"
        public string TargetId { get; set; } // LocationId hoặc RestaurantId tương ứng

        // Navigation Property
        public User? User { get; set; }
    }
}