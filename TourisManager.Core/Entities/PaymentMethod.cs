using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    [Table("PaymentMethod")]
    public class PaymentMethod
    {
        [Key]
        public string PaymentMethodId { get; set; }
            = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;


        // 1 - N
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
