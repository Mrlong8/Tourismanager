using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Text.Json;
using TourisManager.Core.Entities;
using TourisManager.Core.Extensions;
using TourisManager.Core.Interfaces;
using TourisManager.Data;
using TourisManager.Helpers;
using TourisManager.Models;
using TourisManager.Services;

namespace TourisManager.Controllers
{
    public class FoodController : Controller
    {
        private IFoodService _foodService;
        private IAccountService _accountService;
        private int pageSize = 12;
        public FoodController(IFoodService service, IAccountService accountService)
        {
            _foodService = service;
            _accountService = accountService;
        }

        private async Task SetUserLocationToViewBag()
        {
            string accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(accountId))
            {
                var account = await _accountService.DetailAccount(accountId);

                if (account != null && account.DetailAccount != null)
                {
                    if (account.DetailAccount.Latitude != null && account.DetailAccount.Longitude != null)
                    {
                        ViewBag.UserLat = account.DetailAccount.Latitude;
                        ViewBag.UserLng = account.DetailAccount.Longitude;
                    }
                }
            }
        }

        // GET: /Food
        [HttpGet]
        public async Task<IActionResult> Index()
        {
             await SetUserLocationToViewBag();
            var allFood = await _foodService.GetListFoodAsync();
            int pageNum = (int)Math.Ceiling(allFood.Count / (float)pageSize);

            ViewBag.pageNum = pageNum;
            ViewBag.pageIndex = 1;
            ViewBag.typeId = null;

            var result = allFood.Take(pageSize).ToList();
            return View(result);
        }



        [HttpGet]
        public async Task<IActionResult> FilterFoodsByType(string? typeId, List<string>? typeIds, int? pageIndex)
        {
            await SetUserLocationToViewBag();

            List<string> selectedTypes = new List<string>();
            if (typeIds != null && typeIds.Any())
            {
                selectedTypes.AddRange(typeIds.Where(t => !string.IsNullOrWhiteSpace(t)));
            }
            else if (!string.IsNullOrWhiteSpace(typeId))
            {
                selectedTypes.Add(typeId);
            }

            var allFood = selectedTypes.Any()
                ? await _foodService.FilterFoodsByMultiTypeAsync(selectedTypes)
                : await _foodService.GetListFoodAsync();

            int page = (pageIndex == null || pageIndex <= 0) ? 1 : pageIndex.Value;
            int pageNum = (int)Math.Ceiling(allFood.Count() / (float)pageSize);

            ViewBag.pageNum = pageNum;
            ViewBag.pageIndex = page;
            ViewBag.typeId = selectedTypes.Count == 1 ? selectedTypes.First() : null;
            ViewBag.typeIds = selectedTypes;
            var result = allFood.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            return PartialView("_FoodGridPartial", result);
        }


        [HttpGet]
        public async Task<IActionResult> LoadQuickOrderModal(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Mã món ăn không hợp lệ.");

            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null) return NotFound("Không tìm thấy thông tin món ăn.");

            await SetUserLocationToViewBag();
            double distance = 0;

            if (ViewBag.UserLat != null && ViewBag.UserLng != null &&
                food.Location?.Latitude != null && food.Location?.Longitude != null &&
                (decimal)food.Location.Latitude != 0 && (decimal)food.Location.Longitude != 0)
            {
                distance = TinhKhoangCachExtension.CalculateDistance(
                    (double)ViewBag.UserLat, (double)ViewBag.UserLng,
                    (double)food.Location.Latitude, (double)food.Location.Longitude);
            }

            // Nếu chưa có vị trí hoặc tính ra 0 thì đặt mặc định 2km để tính phí giao hàng
            if (distance <= 0)
            {
                distance = 2.0;
            }

            ViewBag.Distance = Math.Round(distance, 1);
            ViewBag.DeliveryFee = Math.Round(distance * 5000); // 5.000đ mỗi km

            return PartialView("_QuickOrderModal", food);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] CheckoutReques request)
        {
            string accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(accountId)) return Json(new { success = false, message = "Vui lòng đăng nhập!" });

            var newOrder = await _foodService.InsertInvoidAsync(accountId, request);
            if (newOrder == null) return Json(new { success = false, message = "Lỗi tạo hóa đơn." });

            if (request.PaymentMethodID == 2)
            {
                // TRẢ VỀ ORDER ID ĐỂ TẠO MÃ QR, KHÔNG XỬ LÝ THANH TOÁN THÀNH CÔNG Ở ĐÂY NỮA
                return Json(new
                {
                    success = true,
                    isQR = true,
                    orderId = newOrder.OrderId,
                    totalAmount = request.TotalPrice,
                    message = "Đơn hàng đã tạo. Vui lòng quét mã để thanh toán!"
                });
            }

            return Json(new { success = true, isQR = false, redirectUrl = Url.Action("MyOrder", "Profile") });
        }

        // Xử lý dữ liệu giao dịch tự động đẩy về từ SePay và cập nhật trạng thái đơn hàng
        [HttpPost]
        [Route("api/sepay/webhook")]
        public async Task<IActionResult> SePayWebhook([FromBody] SePayWebhookRequest request)
        {
            if (request == null || request.transferType != "in")
            {
                return Ok(new { success = true });
            }

            // SỬA LỖI Ở ĐÂY: Thay \d+ thành [A-Z0-9]+ để khớp với cả chữ và số (ví dụ: DHFB7E14C2)
            var match = System.Text.RegularExpressions.Regex.Match(request.content, @"DH([A-Z0-9]+)");

            if (match.Success)
            {
                string shortOrderId = match.Groups[1].Value;

                // Gọi hàm tìm Order theo mã rút gọn đã viết ở bước trước
                var order = await _foodService.GetOrderStartsWithAsync(shortOrderId);

                if (order != null)
                {
                    await _foodService.ProcessQRPaymentSuccessAsync(order.OrderId);
                }
            }

            return Ok(new { success = true });
        }

        // Kiểm tra trạng thái thanh toán của đơn hàng để phản hồi về giao diện người dùng
        [HttpGet]
        public async Task<IActionResult> CheckPaymentStatus(string orderId)
        {
            bool isPaid = await _foodService.IsOrderPaidAsync(orderId);
            return Json(new { isPaid = isPaid });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCartOrder([FromBody] CartCheckoutRequest request)
        {
            string accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(accountId))
                return Json(new { success = false, message = "Vui lòng đăng nhập để tiếp tục!" });

            if (request == null || request.Items == null || !request.Items.Any())
                return Json(new { success = false, message = "Giỏ hàng trống." });

            // Tạo đơn hàng trực tiếp từ request
            var newOrder = await _foodService.CreateOrderFromCartAsync(accountId, request);

            if (newOrder == null)
                return Json(new { success = false, message = "Có lỗi xảy ra khi tạo hóa đơn. Vui lòng thử lại." });

            if (request.PaymentMethodID == 2)
            {
                return Json(new
                {
                    success = true,
                    isQR = true,
                    orderId = newOrder.OrderId,
                    totalAmount = newOrder.TotalAmount,
                    message = "Vui lòng quét mã QR để hoàn tất thanh toán."
                });
            }

            return Json(new
            {
                success = true,
                isQR = false,
                redirectUrl = Url.Action("MyOrder", "Profile")
            });
        }

    }
}
