using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("Area")]
    public class Area
    {
        [Key]
        public string AreaId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;

        // 1 - N
        public ICollection<Location> Locations { get; set; }  = new List<Location>();
    }
}
