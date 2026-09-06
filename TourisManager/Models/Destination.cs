using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Models
{
    public class Destination
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Address { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal Longitude { get; set; }

        public string? ImageUrl { get; set; }
        public string? CustomIconUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}