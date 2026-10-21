using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Data;
using TourisManager.Helpers;

namespace TourisManager.Controllers
{
    public class FoodController : Controller
    {
        private IFoodService _foodService;
        private int pageSize = 8;
        public FoodController(IFoodService service)
        {
            _foodService = service;
        }

        // GET: /Food
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var allFood = await _foodService.GetListFoodAsync();
            int pageNum = (int)Math.Ceiling(allFood.Count / (float)pageSize);

            ViewBag.pageNum = pageNum;
            ViewBag.pageIndex = 1;
            ViewBag.typeId = null;

            var result = allFood.Take(pageSize).ToList();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> FilterFoodsByType(string typeId,int? pageIndex)
        {
            var allFood = await _foodService.FilterFoodsAsync(typeId);
            int page = (pageIndex == null || pageIndex <= 0) ? 1 : pageIndex.Value;
            int pageNum = (int)Math.Ceiling(allFood.Count() / (float)pageSize);

            ViewBag.pageNum = pageNum;
            ViewBag.pageIndex = page;
            ViewBag.typeId = typeId;
            var result = allFood.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            return PartialView("_FoodGridPartial", result);
        }
    }
}