using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("Image")]
    public class Image
    {
        [Key]
        public string ImageId { get; set; }  = Guid.NewGuid().ToString();
        public string ImageUrl { get; set; } = string.Empty;
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;

        // Navigation
        public Location Location { get; set; } = null!;
    }
}
