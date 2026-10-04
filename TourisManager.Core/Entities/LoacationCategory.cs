using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("LocationCategory")]
    public class LocationCategory
    {
        [ForeignKey(nameof(Category))]
        public string CategoryId { get; set; } = string.Empty;
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;

        // Navigation
        public Category Category { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}
