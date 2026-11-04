
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Core.Interfaces;
namespace TourisManager.Services
{
    public class LocationService : ILocationService
    {
        private readonly AppDbContext _context;
        public List<Location> GetDataAll()
        {
            return _context.Locations
                .AsNoTracking()
                .ToList();
        }
        public LocationService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy truy vấn địa điểm theo từ khóa và danh mục
        public IQueryable<Location> GetLocationsQuery(
            string? searchString,
            string? categoryId)
        {
            var query = _context.Locations
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string keyword = searchString.Trim();

                query = query.Where(l =>
                    (l.Name != null && l.Name.Contains(keyword)) ||
                    (l.Address != null && l.Address.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                query = query.Where(l => l.CategoryId == categoryId);
            }

            return query.OrderBy(l => l.Name);
        }

        // Lấy danh sách danh mục
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        // Tìm địa điểm theo ID
        public async Task<Location?> GetByIdAsync(string id)
        {
            return await _context.Locations
                .FirstOrDefaultAsync(l => l.LocationId == id);
        }

        // Lấy ID tài khoản đầu tiên khi cần dùng dự phòng
        public async Task<string?> GetFirstAccountIdAsync()
        {
            var account = await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return account?.AccountId;
        }

        // Thêm địa điểm
        public async Task CreateAsync(Location location)
        {
            location.CreateAt = DateTime.Now;

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }

        // Cập nhật địa điểm, giữ lại dữ liệu lịch sử
        public async Task<bool> UpdateAsync(string id, Location location)
        {
            var existingLocation = await _context.Locations
                .FirstOrDefaultAsync(l => l.LocationId == id);

            if (existingLocation == null)
                return false;

            existingLocation.Name = location.Name;
            existingLocation.Address = location.Address;
            existingLocation.CategoryId = string.IsNullOrEmpty(location.CategoryId)
                ? existingLocation.CategoryId
                : location.CategoryId;
            existingLocation.ImageUrl = location.ImageUrl;

            // Giữ nguyên AccountId, CreateAt và IconUrl
            await _context.SaveChangesAsync();

            return true;
        }

        // Xóa địa điểm
        public async Task<bool> DeleteAsync(string id)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.LocationId == id);

            if (location == null)
                return false;

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
