using System.ComponentModel.DataAnnotations;

namespace TourisManager.Core.Entities
{
    public class BankAccount
    {
        [Key]
        public string BankAccountId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string AccountId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccountHolderName { get; set; }

        public bool IsDefault { get; set; } = false;

        // Navigation property
        public virtual Account Account { get; set; }
    }
}