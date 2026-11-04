using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Areas.Seller.Models;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<Location?> GetCurrentSellerLocationAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;
            return await _context.Locations.FirstOrDefaultAsync(l => l.AccountId == userId);
        }

        public static string GetStatusDisplayName(OrderStatus status)
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

        // GET: /Seller/Order
        [HttpGet]
        public async Task<IActionResult> Index(string? search, OrderStatus? status)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null)
            {
                return RedirectToAction("CreateLocation", "Dashboard");
            }

            // Lấy tất cả đơn hàng thuộc gian hàng này
            var allOrders = await _context.Orders
                .Where(o => o.LocationId == location.LocationId)
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

            // Thống kê tổng số lượng đơn theo từng trạng thái
            int totalOrders = allOrders.Count;
            int pendingOrders = allOrders.Count(o => o.OrderStatus == OrderStatus.Pending);
            int shippingOrders = allOrders.Count(o => o.OrderStatus == OrderStatus.Shipping);
            int completedOrders = allOrders.Count(o => o.OrderStatus == OrderStatus.Completed);
            int cancelledOrders = allOrders.Count(o => o.OrderStatus == OrderStatus.Cancelled);
            decimal totalRevenue = allOrders.Where(o => o.OrderStatus == OrderStatus.Completed).Sum(o => o.TotalAmount);

            // Tiến hành lọc đơn hàng
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

            var viewModel = new OrderListViewModel
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

            ViewBag.Location = location;
            return View(viewModel);
        }

        // POST: /Seller/Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string orderId, OrderStatus newStatus, OrderStatus? currentFilter)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null) return RedirectToAction("CreateLocation", "Dashboard");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.LocationId == location.LocationId);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng cần cập nhật!";
                return RedirectToAction(nameof(Index), new { status = currentFilter });
            }

            order.OrderStatus = newStatus;

            // Nếu đơn hàng chuyển sang Hoàn thành -> tự động cập nhật trạng thái thanh toán là True
            if (newStatus == OrderStatus.Completed)
            {
                order.PaymentStatus = true;
            }

            await _context.SaveChangesAsync();
            string shortId = order.OrderId.Length > 8 ? order.OrderId.Substring(0, 8) : order.OrderId;
            TempData["Success"] = $"Đã cập nhật đơn hàng #{shortId} sang trạng thái: {GetStatusDisplayName(newStatus)}!";

            return RedirectToAction(nameof(Index), new { status = currentFilter });
        }

        // GET: /Seller/Order/GetOrderDetailJson?id=... (Dùng cho Modal xem chi tiết)
        [HttpGet]
        public async Task<IActionResult> GetOrderDetailJson(string id)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null) return Json(new { success = false, message = "Chưa đăng nhập gian hàng" });

            var order = await _context.Orders
                .Where(o => o.OrderId == id && o.LocationId == location.LocationId)
                .Include(o => o.Account).ThenInclude(a => a.DetailAccount)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Food).ThenInclude(f => f.TypeFood)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Food).ThenInclude(f => f.FoodSizes).ThenInclude(fs => fs.Size)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin đơn hàng" });
            }

            var items = order.OrderDetails.Select(od => new
            {
                foodName = od.Food?.Name ?? "Món ăn",
                imageUrl = od.Food?.ImageUrl ?? "/Image/Food/default_food.jpg",
                typeName = od.Food?.TypeFood?.Name ?? "Đồ ăn",
                sizeName = od.Food?.FoodSizes?.FirstOrDefault()?.Size?.Name ?? "Tiêu chuẩn",
                quantity = od.Quantity,
                unitPrice = od.Quantity > 0 ? (od.TotalPrice / od.Quantity) : 0,
                totalPrice = od.TotalPrice
            }).ToList();

            return Json(new
            {
                success = true,
                orderId = order.OrderId,
                createdAt = order.CreateAt.ToString("dd/MM/yyyy HH:mm"),
                customerName = order.Account?.Name ?? "Khách vãng lai",
                phone = order.Account?.DetailAccount?.SDT ?? "Chưa cập nhật",
                address = order.Account?.DetailAccount?.Address ?? "Chưa cập nhật",
                paymentMethod = order.PaymentMethod?.Name ?? "Tiền mặt",
                paymentStatus = order.PaymentStatus ? "Đã thanh toán" : "Chưa thanh toán",
                orderStatus = (int)order.OrderStatus,
                statusName = GetStatusDisplayName(order.OrderStatus),
                totalAmount = order.TotalAmount,
                items = items
            });
        }
    }
}

