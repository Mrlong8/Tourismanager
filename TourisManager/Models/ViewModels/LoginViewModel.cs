namespace TourisManager.Models.ViewModels
{
    public class LoginViewModel
    {
        public string UsernameOrEmail { get; set; } = string.Empty; // Nhập Username hoặc Email
        public string Password { get; set; } = string.Empty;
    }
}