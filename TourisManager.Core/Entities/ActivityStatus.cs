using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("ActivityStatus")]
    public class ActivityStatus
    {
        [Key]
        public string ActivityStatusId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }


        // Navigation
        public Location Location { get; set; } = null!;
    }
}
