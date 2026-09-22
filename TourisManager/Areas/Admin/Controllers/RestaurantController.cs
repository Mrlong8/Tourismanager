using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Data;
using TourisManager.Core.Entities;

namespace TourisManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RestaurantController : Controller
    {
        private readonly AppDbContext _context;

        public RestaurantController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Restaurant
        public async Task<IActionResult> Index()
        {
            var restaurants = await _context.Restaurants.ToListAsync();
            return View(restaurants);
        }

        // GET: /Admin/Restaurant/Details/id
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(x => x.RestaurantId == id);

            if (restaurant == null) return NotFound();

            return View(restaurant);
        }

        // GET: /Admin/Restaurant/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Restaurant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Restaurant restaurant)
        {
            if (string.IsNullOrEmpty(restaurant.RestaurantId))
            {
                restaurant.RestaurantId = "res-" + Guid.NewGuid().ToString().Substring(0, 5);
            }

            // Gán giá trị mặc định cho CreateBy và CategoryId từ dữ liệu thực tế trong DB
            if (string.IsNullOrEmpty(restaurant.CreateBy))
            {
                restaurant.CreateBy = "acc-01"; // AccountId có sẵn trong DB
            }

            if (string.IsNullOrEmpty(restaurant.CategoryId))
            {
                restaurant.CategoryId = "cat-01"; // CategoryId có sẵn trong DB
            }

            // Gán ngày tạo
            restaurant.CreateAt = DateTime.Now;

            ModelState.Remove(nameof(Restaurant.RestaurantId));
            ModelState.Remove(nameof(Restaurant.CreateBy));
            ModelState.Remove(nameof(Restaurant.CategoryId));

            if (ModelState.IsValid)
            {
                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(restaurant);
        }

        // GET: /Admin/Restaurant/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null) return NotFound();

            return View(restaurant);
        }

        // POST: /Admin/Restaurant/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Restaurant restaurant)
        {
            if (id != restaurant.RestaurantId) return NotFound();

            var oldRestaurant = await _context.Restaurants.AsNoTracking().FirstOrDefaultAsync(x => x.RestaurantId == id);
            if (oldRestaurant == null) return NotFound();

            if (string.IsNullOrEmpty(restaurant.CreateBy))
            {
                restaurant.CreateBy = oldRestaurant.CreateBy;
            }
            if (string.IsNullOrEmpty(restaurant.CategoryId))
            {
                restaurant.CategoryId = oldRestaurant.CategoryId;
            }

            restaurant.CreateAt = oldRestaurant.CreateAt;

            ModelState.Remove(nameof(Restaurant.CreateBy));
            ModelState.Remove(nameof(Restaurant.CategoryId));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Restaurants.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.RestaurantId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(restaurant);
        }

        // GET: /Admin/Restaurant/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(x => x.RestaurantId == id);

            if (restaurant == null) return NotFound();

            return View(restaurant);
        }

        // POST: /Admin/Restaurant/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(string id)
        {
            return _context.Restaurants.Any(e => e.RestaurantId == id);
        }
    }
}