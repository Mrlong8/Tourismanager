using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    [Table("User")]
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } // Cho phép Null nếu đăng nhập bằng Google
        public string Role { get; set; } = "User"; // User, Seller, Admin
        public string? AvataUrl { get; set; }
        public string AuthProvider { get; set; } = "Local"; // Local hoặc Google
        public string? ProviderKey { get; set; } // Google Sub ID
        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation Properties (Quan hệ)
        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}