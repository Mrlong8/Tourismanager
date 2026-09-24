using System.ComponentModel.DataAnnotations;

namespace TourisManager.Models.ViewModels
{
    public class SignupViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "Tên tài khoản không được chứa khoảng trắng, ký tự đặc biệt và phải từ 3 - 20 ký tự")]
        [Display(Name = "Tên tài khoản")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;

        //[Range(typeof(bool), "true", "true", ErrorMessage = "Bạn phải đồng ý với điều khoản")]
        //public bool AcceptTerms { get; set; }
    }
}