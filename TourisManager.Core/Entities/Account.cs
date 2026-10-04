using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("Account")] // Đổi tên bảng CSDL thành Account
    public class Account
    {
        public string AccountId { get; set; } // Đổi tên thuộc tính từ UserId thành AccountId
        public string? Username { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string Role { get; set; } = "User"; // User, Seller, Admin
        public string? AvataUrl { get; set; }
        public string AuthProvider { get; set; } = "Local";
        public string? ProviderKey { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public DetailAccount? DetailAccount { get; set; }
        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<SavedFood> SavedFoods { get; set; } = new List<SavedFood>();
        public ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();
        public ICollection<RoomChat> RoomChats { get; set; } = new List<RoomChat>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();



    }
}