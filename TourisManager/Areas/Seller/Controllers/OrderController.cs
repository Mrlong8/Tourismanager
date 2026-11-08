using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourisManager.Areas.Seller.Models;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;

namespace TourisManager.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller,Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private async Task<Location?> GetCurrentSellerLocationAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;
            return await _orderService.GetSellerLocationAsync(userId);
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

            var filterResult = await _orderService.GetSellerOrdersAsync(location.LocationId, search, status);

            var viewModel = new OrderListViewModel
            {
                Orders = filterResult.Orders,
                SearchTerm = filterResult.SearchTerm,
                SelectedStatus = filterResult.SelectedStatus,
                TotalOrders = filterResult.TotalOrders,
                PendingOrders = filterResult.PendingOrders,
                ShippingOrders = filterResult.ShippingOrders,
                CompletedOrders = filterResult.CompletedOrders,
                CancelledOrders = filterResult.CancelledOrders,
                TotalRevenue = filterResult.TotalRevenue
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

            var result = await _orderService.UpdateOrderStatusAsync(orderId, location.LocationId, newStatus);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }

            return RedirectToAction(nameof(Index), new { status = currentFilter });
        }

        // GET: /Seller/Order/GetOrderDetailJson?id=... (Dùng cho Modal xem chi tiết)
        [HttpGet]
        public async Task<IActionResult> GetOrderDetailJson(string id)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null) return Json(new { success = false, message = "Chưa đăng nhập gian hàng" });

            var order = await _orderService.GetOrderDetailAsync(id, location.LocationId);
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
                statusName = _orderService.GetStatusDisplayName(order.OrderStatus),
                totalAmount = order.TotalAmount,
                items = items
            });
        }
    }
}
