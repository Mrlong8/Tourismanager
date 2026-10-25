using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    public class DetailAccount
    {
        [Key]
        public string DetailId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; } = string.Empty;
        public string? SDT { get; set; }
        public string? Address { get; set; }

        // Tọa độ vị trí (thay thế GoogleMapUrl)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Navigation
        public Account Account { get; set; } = null!;
    }
}
