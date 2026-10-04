using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace TourisManager.Core.Entities
{
    [Table("Location")]
    public class Location
    {
        public string LocationId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string? Address { get; set; }
        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
        public string? ImageUrl { get; set; }
        public string? IconUrl { get; set; }

        [ForeignKey(nameof(Account))]
        public string? AccountId { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(Area))]
        public string? AreaId { get; set; }



        // Navigation

        public Account? Account { get; set; }
        public Area? Area { get; set; }


        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Food> Foods { get; set; } = new List<Food>();
        public ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();
        public ICollection<RoomChat> RoomChats { get; set; } = new List<RoomChat>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<ActivityStatus> ActivityStatuses { get; set; } = new List<ActivityStatus>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<LocationCategory> LocationCategories { get; set; } = new List<LocationCategory>();
    }
}