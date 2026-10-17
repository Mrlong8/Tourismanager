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
        public string? GoogleMapUrl { get; set; }
        // Navigation
        public Account Account { get; set; } = null!;
    }
}
