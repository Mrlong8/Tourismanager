using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Core.Models;
using TourisManager.Data;

namespace TourisManager.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Location?> GetSellerLocationAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;
            return await _context.Locations.FirstOrDefaultAsync(l => l.AccountId == userId);
        }

        public async Task<OrderFilterResult> GetSellerOrdersAsync(string locationId, string? search, OrderStatus? status)
        {
            var allOrders = await _context.Orders
                .Where(o => o.LocationId == locationId)
                .Include(o => o.Account)
                    .ThenInclude(a => a.DetailAccount)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                        .ThenInclude(f => f.TypeFood)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food)
                        .ThenInclude(f => f.FoodSizes)
                            .ThenInclude(fs => fs.Size)
                .OrderByDescending(o => o.CreateAt)
                .ToListAsync();

            int totalOrders = allOrders.Count;
            int pendingOrders = allOrders.Count(o => o.OrderStatus == OrderStatus.Pending);
            int shippingOrders = allOrders.Count(o => o.OrderStatus == OrderStatus.Shipping);
            int completedOrders = allOrders.Count(o => o.OrderStatus == OrderStatus.Completed);
            int cancelledOrders = allOrders.Count(o => o.OrderStatus == OrderStatus.Cancelled);
            decimal totalRevenue = allOrders.Where(o => o.OrderStatus == OrderStatus.Completed).Sum(o => o.TotalAmount);

            var filteredOrders = allOrders.AsEnumerable();

            if (status.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.OrderStatus == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                filteredOrders = filteredOrders.Where(o =>
                    o.OrderId.ToLower().Contains(term) ||
                    (o.Account != null && o.Account.Name != null && o.Account.Name.ToLower().Contains(term)) ||
                    (o.Account?.DetailAccount?.SDT != null && o.Account.DetailAccount.SDT.ToLower().Contains(term)) ||
                    (o.Account?.DetailAccount?.Address != null && o.Account.DetailAccount.Address.ToLower().Contains(term))
                );
            }

            return new OrderFilterResult
            {
                Orders = filteredOrders.ToList(),
                SearchTerm = search,
                SelectedStatus = status,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                ShippingOrders = shippingOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<(bool Success, string Message, string? ShortOrderId)> UpdateOrderStatusAsync(string orderId, string locationId, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.LocationId == locationId);

            if (order == null)
            {
                return (false, "Không tìm thấy đơn hàng cần cập nhật!", null);
            }

            order.OrderStatus = newStatus;

            if (newStatus == OrderStatus.Completed)
            {
                order.PaymentStatus = true;
            }

            await _context.SaveChangesAsync();
            string shortId = order.OrderId.Length > 8 ? order.OrderId.Substring(0, 8) : order.OrderId;
            string statusName = GetStatusDisplayName(newStatus);

            return (true, $"Đã cập nhật đơn hàng #{shortId} sang trạng thái: {statusName}!", shortId);
        }

        public async Task<Order?> GetOrderDetailAsync(string orderId, string locationId)
        {
            return await _context.Orders
                .Where(o => o.OrderId == orderId && o.LocationId == locationId)
                .Include(o => o.Account).ThenInclude(a => a.DetailAccount)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Food).ThenInclude(f => f.TypeFood)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Food).ThenInclude(f => f.FoodSizes).ThenInclude(fs => fs.Size)
                .FirstOrDefaultAsync();
        }

        public string GetStatusDisplayName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Chờ xác nhận",
                OrderStatus.Shipping => "Đang giao hàng",
                OrderStatus.Completed => "Hoàn thành",
                OrderStatus.Cancelled => "Đã hủy",
                _ => status.ToString()
            };
        }
    }
}

