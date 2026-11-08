using System.ComponentModel.DataAnnotations;

namespace TourisManager.Models.ViewModels
{
    public class UpdateBankInfoVm
    {
        public string? BankAccountId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngân hàng.")]
        [StringLength(50)]
        public string BankCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản.")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Số tài khoản không hợp lệ.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số tài khoản chỉ gồm chữ số.")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản.")]
        [StringLength(100)]
        public string AccountHolderName { get; set; } = string.Empty;
    }
}