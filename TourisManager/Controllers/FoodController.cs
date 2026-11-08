using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Data;
using TourisManager.Helpers;
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
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Mã món ăn không hợp lệ.");
            }

            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null)
            {
                return NotFound("Không tìm thấy thông tin món ăn.");
            }

            return PartialView("_QuickOrderModal", food);
        }


        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] CheckoutReques request)
        {
            string accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để đặt hàng!" });
            }

            if (request == null || string.IsNullOrEmpty(request.FoodId) || request.Quantity <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu đơn hàng không hợp lệ." });
            }

            bool isSuccess = await _foodService.InsertInvoidAsync(accountId,request);
            if (isSuccess)
            {
                return Json(new { success = true, message = "Đặt hàng thành công!" ,
                                  redirectUrl = Url.Action("MyOrder","Profile") });
            }

            return Json(new { success = false, message = "Có lỗi xảy ra trong quá trình đặt hàng." });

        }




    }
}
