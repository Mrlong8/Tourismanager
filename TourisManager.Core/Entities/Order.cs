using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Account))]
        public string AccountId { get; set; } = string.Empty;
        [ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        [ForeignKey(nameof(PaymentMethod))]
        public string PaymentMethodId { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string OrderStatus { get; set; } = string.Empty;


        // Navigation

        public Account Account { get; set; } = null!;
        public Location Location { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; } = null!;

        // 1 - N
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
