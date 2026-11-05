using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
using TourisManager.Core.Interfaces;
using TourisManager.Helpers;

namespace TourisManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly IWebHostEnvironment _environment;

        public LocationController(
            ILocationService locationService,
            IWebHostEnvironment environment)
        {
            _locationService = locationService;
            _environment = environment;
        }

        public async Task<IActionResult> Index(
            string? searchString,
            string? categoryId,
            int? pageIndex)
        {
            var query = _locationService.GetLocationsQuery(
                searchString,
                categoryId);

            ViewBag.Categories =
                await _locationService.GetCategoriesAsync();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryId;

            const int pageSize = 8;
            int pageNumber = Math.Max(pageIndex ?? 1, 1);

            var pagedData = await PaginatedList<Location>.CreateAsync(
                query,
                pageNumber,
                pageSize);

            return View(pagedData);
        }

        // Chi tiết địa điểm
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var location = await _locationService.GetByIdAsync(id);

            if (location == null)
                return NotFound();

            var model = new TourisManager.Models.LocationDetailViewModel
            {
                Id = location.LocationId,
                Name = location.Name,
                Address = location.Address ?? "",
                Latitude = (double)location.Latitude,
                Longitude = (double)location.Longitude,
                ImageUrl = location.ImageUrl,

                Extra = new TourisManager.Models.LocationExtra
                {
                    Description = location.Description
                        ?? "Thông tin đang được cập nhật."
                }
            };

            return View(model);
        }

        // GET: Tạo địa điểm
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View();
        }

        // POST: Tạo địa điểm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Location location,
            IFormFile? imageFile)
        {
            ModelState.Remove(nameof(Location.AccountId));

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(location.CategoryId);
                return View(location);
            }

            var accountId =
                HttpContext.Session.GetString("AccountId");

            if (string.IsNullOrWhiteSpace(accountId))
            {
                accountId =
                    await _locationService.GetFirstAccountIdAsync();
            }

            if (string.IsNullOrWhiteSpace(accountId))
            {
                ModelState.AddModelError(
                    "",
                    "Hệ thống chưa có tài khoản trong bảng Account.");

                await LoadCategoriesAsync(location.CategoryId);
                return View(location);
            }

            location.AccountId = accountId;

            if (imageFile != null && imageFile.Length > 0)
            {
                location.ImageUrl =
                    await SaveImageAsync(imageFile);
            }

            await _locationService.CreateAsync(location);

            TempData["Success"] = "Thêm địa điểm thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Sửa địa điểm
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var location =
                await _locationService.GetByIdAsync(id);

            if (location == null)
                return NotFound();

            await LoadCategoriesAsync(location.CategoryId);

            return View(location);
        }

        // POST: Sửa địa điểm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            Location location,
            IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(id) ||
                id != location.LocationId)
            {
                return NotFound();
            }

            var oldLocation =
                await _locationService.GetByIdAsync(id);

            if (oldLocation == null)
                return NotFound();

            location.AccountId = oldLocation.AccountId;
            location.CreateAt = oldLocation.CreateAt;
            location.IconUrl = oldLocation.IconUrl;
            location.ImageUrl = oldLocation.ImageUrl;

            if (string.IsNullOrWhiteSpace(location.CategoryId))
            {
                location.CategoryId =
                    oldLocation.CategoryId;
            }

            ModelState.Remove(nameof(Location.AccountId));
            ModelState.Remove(nameof(Location.CategoryId));

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(location.CategoryId);
                return View(location);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                location.ImageUrl =
                    await SaveImageAsync(imageFile);
            }

            var updated =
                await _locationService.UpdateAsync(id, location);

            if (!updated)
                return NotFound();

            TempData["Success"] = "Cập nhật địa điểm thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Xác nhận xóa
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var location =
                await _locationService.GetByIdAsync(id);

            if (location == null)
                return NotFound();

            return View(location);
        }

        // POST: Xóa địa điểm
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            try
            {
                var deleted =
                    await _locationService.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                TempData["Success"] =
                    "Xóa địa điểm thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "Không thể xóa địa điểm này vì có dữ liệu liên quan!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategoriesAsync(
            string? selectedCategoryId = null)
        {
            var categories =
                await _locationService.GetCategoriesAsync();

            ViewBag.CategoryId = new SelectList(
                categories,
                "CategoryId",
                "Name",
                selectedCategoryId);
        }

        private async Task<string> SaveImageAsync(
            IFormFile imageFile)
        {
            var uploadDir = Path.Combine(
                _environment.WebRootPath,
                "Image",
                "Location");

            Directory.CreateDirectory(uploadDir);

            var extension =
                Path.GetExtension(imageFile.FileName);

            var fileName =
                $"{Guid.NewGuid():N}{extension}";

            var filePath =
                Path.Combine(uploadDir, fileName);

            await using var fileStream =
                new FileStream(
                    filePath,
                    FileMode.Create);

            await imageFile.CopyToAsync(fileStream);

            return $"/Image/Location/{fileName}";
        }
    }
}