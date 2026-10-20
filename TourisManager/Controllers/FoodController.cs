using Microsoft.AspNetCore.Mvc;
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

        public FoodController(IFoodService service)
        {
            _foodService = service;
        }

        // GET: /Food
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listFood = await _foodService.GetListFoodAsync();
            return View(listFood);
        }

        [HttpGet]
        public async Task<IActionResult> FilterFoodsByType(string typeId)
        {
            var foods = await _foodService.FilterFoodsAsync(typeId);
            return PartialView("_FoodGridPartial", foods);
        }
    }
}