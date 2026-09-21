using System.ComponentModel.DataAnnotations;

namespace TourisManager.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập UserName hoặc Email")]
        [Display(Name = "UserName hoặc Email" )]
        public string UsernameOrEmail { get; set; } = string.Empty; // Nhập Username hoặc Email

        [Required(ErrorMessage = "Vui Lòng Nhập Mật Khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật Khẩu")]
        public string Password { get; set; } = string.Empty;
    }
}