using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourisManager.Areas.Seller.Models;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize]
    public class FoodController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public FoodController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private async Task<Location?> GetCurrentSellerLocationAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;
            return await _context.Locations.FirstOrDefaultAsync(l => l.AccountId == userId);
        }

        private async Task EnsureDefaultTypesAndSizesAsync()
        {
            if (!await _context.TypeFoods.AnyAsync())
            {
                _context.TypeFoods.AddRange(
                    new TypeFood { TypeFoodId = Guid.NewGuid().ToString(), Name = "Món chính" },
                    new TypeFood { TypeFoodId = Guid.NewGuid().ToString(), Name = "Món khai vị" },
                    new TypeFood { TypeFoodId = Guid.NewGuid().ToString(), Name = "Đồ uống" },
                    new TypeFood { TypeFoodId = Guid.NewGuid().ToString(), Name = "Món tráng miệng" },
                    new TypeFood { TypeFoodId = Guid.NewGuid().ToString(), Name = "Ăn vặt" },
                    new TypeFood { TypeFoodId = Guid.NewGuid().ToString(), Name = "Lẩu & Nướng" }
                );
                await _context.SaveChangesAsync();
            }

            if (!await _context.Sizes.AnyAsync())
            {
                _context.Sizes.AddRange(
                    new Size { SizeId = Guid.NewGuid().ToString(), Name = "Tiêu chuẩn" },
                    new Size { SizeId = Guid.NewGuid().ToString(), Name = "Size Nhỏ (S)" },
                    new Size { SizeId = Guid.NewGuid().ToString(), Name = "Size Vừa (M)" },
                    new Size { SizeId = Guid.NewGuid().ToString(), Name = "Size Lớn (L)" }
                );
                await _context.SaveChangesAsync();
            }
        }

        // GET: /Seller/Food
        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? typeId)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null)
            {
                return RedirectToAction("CreateLocation", "Dashboard");
            }

            await EnsureDefaultTypesAndSizesAsync();

            var query = _context.Foods
                .Where(f => f.LocationId == location.LocationId)
                .Include(f => f.TypeFood)
                .Include(f => f.FoodSizes).ThenInclude(fs => fs.Size)
                .OrderByDescending(f => f.CreateAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(f => f.Name.ToLower().Contains(term) || (f.Description != null && f.Description.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(typeId))
            {
                query = query.Where(f => f.TypeFoodId == typeId);
            }

            var foods = await query.ToListAsync();
            var allSellerFoods = await _context.Foods
                .Where(f => f.LocationId == location.LocationId)
                .Include(f => f.FoodSizes)
                .ToListAsync();

            var categories = await _context.TypeFoods.OrderBy(t => t.Name).ToListAsync();

            var allPrices = allSellerFoods.SelectMany(f => f.FoodSizes).Select(fs => fs.Price).ToList();
            decimal avgPrice = allPrices.Any() ? allPrices.Average() : 0m;

            var viewModel = new FoodListViewModel
            {
                Foods = foods,
                Categories = categories,
                SearchTerm = search,
                SelectedTypeId = typeId,
                TotalItems = allSellerFoods.Count,
                TotalCategories = allSellerFoods.Select(f => f.TypeFoodId).Distinct().Count(),
                AvgPrice = avgPrice
            };

            ViewBag.Location = location;
            return View(viewModel);
        }

        // POST: /Seller/Food/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodViewModel model, IFormFile? imageFile)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null)
            {
                return RedirectToAction("CreateLocation", "Dashboard");
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                TempData["Error"] = "Vui lòng nhập tên món ăn!";
                return RedirectToAction(nameof(Index));
            }

            // Xử lý tạo loại món mới nếu người bán tự nhập
            if (!string.IsNullOrWhiteSpace(model.NewTypeName))
            {
                var trimmedTypeName = model.NewTypeName.Trim();
                var existingType = await _context.TypeFoods.FirstOrDefaultAsync(t => t.Name.ToLower() == trimmedTypeName.ToLower());
                if (existingType == null)
                {
                    existingType = new TypeFood
                    {
                        TypeFoodId = Guid.NewGuid().ToString(),
                        Name = trimmedTypeName
                    };
                    _context.TypeFoods.Add(existingType);
                    await _context.SaveChangesAsync();
                }
                model.TypeFoodId = existingType.TypeFoodId;
            }

            if (string.IsNullOrWhiteSpace(model.TypeFoodId))
            {
                var defaultType = await _context.TypeFoods.FirstOrDefaultAsync();
                model.TypeFoodId = defaultType?.TypeFoodId ?? Guid.NewGuid().ToString();
            }

            // Xử lý upload ảnh
            string finalImageUrl = model.ImageUrl ?? "";
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadDir = Path.Combine(_environment.WebRootPath, "Image", "Food");
                Directory.CreateDirectory(uploadDir);
                var extension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadDir, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                finalImageUrl = $"/Image/Food/{fileName}";
            }

            if (string.IsNullOrWhiteSpace(finalImageUrl))
            {
                finalImageUrl = "/Image/Food/default_food.jpg";
            }

            // Tạo món ăn mới
            var food = new Food
            {
                FoodId = Guid.NewGuid().ToString(),
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                ImageUrl = finalImageUrl,
                TypeFoodId = model.TypeFoodId,
                LocationId = location.LocationId,
                CreateAt = DateTime.Now
            };

            _context.Foods.Add(food);

            // Gán Size và giá bán
            var sizeName = string.IsNullOrWhiteSpace(model.SizeName) ? "Tiêu chuẩn" : model.SizeName.Trim();
            var size = await _context.Sizes.FirstOrDefaultAsync(s => s.Name.ToLower() == sizeName.ToLower());
            if (size == null)
            {
                size = new Size
                {
                    SizeId = Guid.NewGuid().ToString(),
                    Name = sizeName
                };
                _context.Sizes.Add(size);
                await _context.SaveChangesAsync();
            }

            _context.FoodSizes.Add(new FoodSize
            {
                FoodId = food.FoodId,
                SizeId = size.SizeId,
                Price = model.Price
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã thêm món \"{food.Name}\" vào thực đơn thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/Food/GetFoodJson?id=... (Dùng cho Modal Edit)
        [HttpGet]
        public async Task<IActionResult> GetFoodJson(string id)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null) return Json(new { success = false, message = "Chưa đăng nhập gian hàng" });

            var food = await _context.Foods
                .Include(f => f.TypeFood)
                .Include(f => f.FoodSizes).ThenInclude(fs => fs.Size)
                .FirstOrDefaultAsync(f => f.FoodId == id && f.LocationId == location.LocationId);

            if (food == null)
            {
                return Json(new { success = false, message = "Không tìm thấy món ăn" });
            }

            var primarySize = food.FoodSizes.FirstOrDefault();

            return Json(new
            {
                success = true,
                foodId = food.FoodId,
                name = food.Name,
                description = food.Description ?? "",
                imageUrl = food.ImageUrl ?? "",
                typeFoodId = food.TypeFoodId,
                price = primarySize?.Price ?? 0,
                sizeName = primarySize?.Size?.Name ?? "Tiêu chuẩn"
            });
        }

        // POST: /Seller/Food/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FoodViewModel model, IFormFile? imageFile)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null) return RedirectToAction("CreateLocation", "Dashboard");

            if (string.IsNullOrWhiteSpace(model.FoodId))
            {
                TempData["Error"] = "Mã món ăn không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var food = await _context.Foods
                .Include(f => f.FoodSizes)
                .FirstOrDefaultAsync(f => f.FoodId == model.FoodId && f.LocationId == location.LocationId);

            if (food == null)
            {
                TempData["Error"] = "Không tìm thấy món ăn cần chỉnh sửa!";
                return RedirectToAction(nameof(Index));
            }

            // Xử lý loại món mới nếu có
            if (!string.IsNullOrWhiteSpace(model.NewTypeName))
            {
                var trimmedTypeName = model.NewTypeName.Trim();
                var existingType = await _context.TypeFoods.FirstOrDefaultAsync(t => t.Name.ToLower() == trimmedTypeName.ToLower());
                if (existingType == null)
                {
                    existingType = new TypeFood
                    {
                        TypeFoodId = Guid.NewGuid().ToString(),
                        Name = trimmedTypeName
                    };
                    _context.TypeFoods.Add(existingType);
                    await _context.SaveChangesAsync();
                }
                food.TypeFoodId = existingType.TypeFoodId;
            }
            else if (!string.IsNullOrWhiteSpace(model.TypeFoodId))
            {
                food.TypeFoodId = model.TypeFoodId;
            }

            food.Name = model.Name.Trim();
            food.Description = model.Description?.Trim();

            // Nếu người bán upload ảnh mới
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadDir = Path.Combine(_environment.WebRootPath, "Image", "Food");
                Directory.CreateDirectory(uploadDir);
                var extension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadDir, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                food.ImageUrl = $"/Image/Food/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                food.ImageUrl = model.ImageUrl.Trim();
            }

            // Cập nhật giá và kích cỡ
            var foodSize = food.FoodSizes.FirstOrDefault();
            var sizeName = string.IsNullOrWhiteSpace(model.SizeName) ? "Tiêu chuẩn" : model.SizeName.Trim();
            var size = await _context.Sizes.FirstOrDefaultAsync(s => s.Name.ToLower() == sizeName.ToLower());
            if (size == null)
            {
                size = new Size
                {
                    SizeId = Guid.NewGuid().ToString(),
                    Name = sizeName
                };
                _context.Sizes.Add(size);
                await _context.SaveChangesAsync();
            }

            if (foodSize != null)
            {
                foodSize.Price = model.Price;
                foodSize.SizeId = size.SizeId;
            }
            else
            {
                _context.FoodSizes.Add(new FoodSize
                {
                    FoodId = food.FoodId,
                    SizeId = size.SizeId,
                    Price = model.Price
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật món \"{food.Name}\" thành công!";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Seller/Food/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var location = await GetCurrentSellerLocationAsync();
            if (location == null) return RedirectToAction("CreateLocation", "Dashboard");

            var food = await _context.Foods
                .Include(f => f.FoodSizes)
                .Include(f => f.SavedFoods)
                .FirstOrDefaultAsync(f => f.FoodId == id && f.LocationId == location.LocationId);

            if (food == null)
            {
                TempData["Error"] = "Không tìm thấy món ăn cần xóa!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra xem món ăn đã có đơn hàng chưa
            var hasOrders = await _context.OrderDetails.AnyAsync(od => od.FoodId == id);
            if (hasOrders)
            {
                TempData["Error"] = $"Không thể xóa món \"{food.Name}\" vì đã có đơn đặt hàng từ khách hàng trong hệ thống!";
                return RedirectToAction(nameof(Index));
            }

            _context.FoodSizes.RemoveRange(food.FoodSizes);
            _context.SavedFoods.RemoveRange(food.SavedFoods);
            _context.Foods.Remove(food);

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa món \"{food.Name}\" khỏi thực đơn!";

            return RedirectToAction(nameof(Index));
        }
    }
}

