using System.ComponentModel.DataAnnotations;

namespace TourisManager.Models.ViewModels
{
    public class SellerSignupViewModel
    {
        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên chủ quán không được để trống")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên quán ăn/gian hàng không được để trống")]
        public string ShopName { get; set; } = string.Empty;
    }
}
