using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Core.Models;

namespace TourisManager.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Location?> GetSellerLocationAsync(string userId);
        Task<OrderFilterResult> GetSellerOrdersAsync(string locationId, string? search, OrderStatus? status);
        Task<(bool Success, string Message, string? ShortOrderId)> UpdateOrderStatusAsync(string orderId, string locationId, OrderStatus newStatus);
        Task<Order?> GetOrderDetailAsync(string orderId, string locationId);
        string GetStatusDisplayName(OrderStatus status);
    }
}

