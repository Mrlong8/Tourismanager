using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourisManager.Core.Entities
{
    public class Wallet
    {
        [Key]
        public string WalletId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string AccountId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FrozenBalance { get; set; } = 0;

        [MaxLength(3)]
        public string Currency { get; set; } = "VND";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Account Account { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    }
}