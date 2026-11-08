using System.Collections.Generic;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Models
{
    public class OrderFilterResult
    {
        public List<Order> Orders { get; set; } = new();
        public string? SearchTerm { get; set; }
        public OrderStatus? SelectedStatus { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

