using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;
using TourisManager.Core.Models;
using TourisManager.Models;
using TourisManager.Services;

namespace TourisManager.Core.Interfaces
{
    public interface IFoodService
    {
        Task<List<Food>> GetListFoodAsync();
        Task<IEnumerable<Food>> FilterFoodsAsync(string typeID);
        Task<IEnumerable<Food>> FilterFoodsByMultiTypeAsync(List<string> typeIds);
        Task<Food?> GetFoodByIdAsync(string id);
        Task<Order?> InsertInvoidAsync(string accountId, CheckoutReques request);

        Task<bool> ProcessQRPaymentSuccessAsync(string orderId);
        // Khai báo phương thức kiểm tra xem đơn hàng đã được thanh toán hay chưa
        Task<bool> IsOrderPaidAsync(string orderId);
        // Khai báo phương thức tìm kiếm đơn hàng dựa trên phần đầu của mã đơn hàng
        Task<Order?> GetOrderStartsWithAsync(string shortOrderId);
        // Thêm vào IFoodService.cs
        Task<Order?> CreateOrderFromCartAsync(string accountId, CartCheckoutRequest request);

    }
}
