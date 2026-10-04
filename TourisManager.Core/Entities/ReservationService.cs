using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("ReservationService")]
    public class ReservationService
    {
        [Key]
        public string ReservationServiceId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; } = string.Empty;
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;
        public int PeopleQuantity { get; set; }
        public DateTime Time { get; set; }
        [ForeignKey(nameof(Order))]
        public string? OrderId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;


        // Navigation
        public Account Account { get; set; } = null!;
        public Location Location { get; set; } = null!;
        public Order? Order { get; set; }
    }
}
