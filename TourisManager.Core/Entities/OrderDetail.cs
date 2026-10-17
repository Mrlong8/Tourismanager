using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TourisManager.Core.Entities
{
    public class OrderDetail
    {
        [ForeignKey(nameof(Order))]
        public string OrderId { get; set; } = string.Empty;
        [ForeignKey(nameof(Food))]
        public string FoodId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public Food Food { get; set; } = null!;
    }
}
