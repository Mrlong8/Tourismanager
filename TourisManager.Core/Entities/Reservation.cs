using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("Reservation")]
    public class Reservation
    {
        [Key]
        public string ReservationId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }

        // Navigation
        public Location Location { get; set; } = null!;
    }
}
