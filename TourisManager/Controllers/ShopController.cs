using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TourisManager.Core.Services;

namespace TourisManager.Controllers
{
    public class ShopController : Controller
    {
        private IInvoiceService _shopService;
        public ShopController(IInvoiceService shopService)
        {
            _shopService = shopService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Invoice()
        {
            string accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string Role = User.FindFirstValue(ClaimTypes.Role);


            if (string.IsNullOrWhiteSpace(accountId))
            {
                return RedirectToAction("Login","Account");
            }

            //if (Role != "Seller")
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            var invoidList = await _shopService.GetAllInvoiceBySellerAsync(accountId);
            return View(invoidList);
        }
    }
}
