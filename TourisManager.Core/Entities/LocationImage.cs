using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("LocationImage")]
    public class LocationImage
    {
        public string LocationImageId { get; set; }
        public string LocationId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation Property
        public Location? Location { get; set; }
    }
}