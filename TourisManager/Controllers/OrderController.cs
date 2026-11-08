using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TourisManager.Core.Services;

namespace TourisManager.Controllers
{
    public class OrderController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public OrderController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("cancel-order")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            string accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(accountId))
            {
                return RedirectToAction("Login", "Account");
            }
            bool result = await _invoiceService.CancelOrderAsync(orderId, accountId);
            if (result)
            {
                TempData["Success"] = "Hủy Đơn Hàng Thành Công";
            }
            else
            {
                TempData["Error"] = "Hủy Đơn Hàng Thất Bại";
            }
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                return Json(new { success = result });
            }
            return RedirectToAction("MyOrder", "Profile");
        }
    }
}