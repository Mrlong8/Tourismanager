namespace TourisManager.Models.ViewModels
{
    public class SignupViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Passwort { get; set; } = string.Empty;
        public string AuthenticPasswort { get; set; } = string.Empty;
        public bool AcceptTerms { get; set; }
    }
}