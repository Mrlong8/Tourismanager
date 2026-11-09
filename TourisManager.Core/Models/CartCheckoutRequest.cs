// Models/CartCheckoutRequest.cs
using System.Collections.Generic;
using TourisManager.Core.Models;


namespace TourisManager.Models
{
    public class CartCheckoutRequest
    {
        // Có thể lấy từ ID của cái Draft trong Session, hoặc truyền trực tiếp mảng
        public string DraftId { get; set; }

        public string LocationId { get; set; }

        // ID phương thức thanh toán (1: COD, 2: Chuyển khoản QR)
        public int PaymentMethodID { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal TotalAmount { get; set; }

        public List<CartItemRequest> Items { get; set; }
    }

    public class CartItemRequest
    {
        public string FoodId { get; set; }
        public string SizeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}