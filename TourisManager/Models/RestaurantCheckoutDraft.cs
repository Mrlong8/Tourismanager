using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TourisManager.Models
{
    public class RestaurantCheckoutRequest
    {
        [Required, StringLength(100)] public string LocationId { get; set; } = "";
        [Required, MinLength(1), MaxLength(80)] public List<RestaurantCheckoutLine> Items { get; set; } = new();
    }

    public class RestaurantCheckoutLine
    {
        [Required, StringLength(200)] public string MenuItemId { get; set; } = "";
        [Range(1, 99)] public int Quantity { get; set; }
    }

    // Contract for the invoice page. No Order or OrderDetail is inserted at this stage.
    public class RestaurantCheckoutDraft
    {
        public const string SessionKey = "FoodieMap.CheckoutDraft.v1";
        public int Version { get; set; } = 1;
        public string DraftId { get; set; } = Guid.NewGuid().ToString("N");
        public string LocationId { get; set; } = "";
        public string RestaurantName { get; set; } = "";
        public string RestaurantAddress { get; set; } = "";
        public string Currency { get; set; } = "VND";
        public string Status { get; set; } = "draft";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsIllustrative { get; set; }
        public List<RestaurantCheckoutDraftItem> Items { get; set; } = new();
        public int TotalQuantity => Items.Sum(i => i.Quantity);
        public decimal TotalAmount => Items.Sum(i => i.LineTotal);
    }

    public class RestaurantCheckoutDraftItem
    {
        public string MenuItemId { get; set; } = "";
        public string? FoodId { get; set; }
        public string? SizeId { get; set; }
        public string Name { get; set; } = "";
        public string Unit { get; set; } = "phần";
        public string ImageUrl { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
