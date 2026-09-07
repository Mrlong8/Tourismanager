using System.ComponentModel.DataAnnotations;

namespace TourisManager.Models.Entity
{
    public class User
    {
        [Key]
        public string UserId  { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
