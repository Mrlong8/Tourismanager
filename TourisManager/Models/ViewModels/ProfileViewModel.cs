namespace TourisManager.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string AccountId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? AvataUrl { get; set; }

        // Các thông tin từ DetailAccount
        public string? SDT { get; set; }
        public string? Address { get; set; }

        // Tọa độ vị trí
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
