using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Core.Interfaces;

namespace TourisManager.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy số liệu tổng quan cho trang Admin
        public async Task<(int TotalUsers, int TotalLocations)>
            GetDashboardStatsAsync()
        {
            var totalUsers = await _context.Accounts.CountAsync();
            var totalLocations = await _context.Locations.CountAsync();

            return (totalUsers, totalLocations);
        }

        // Lấy danh sách tài khoản
        public async Task<List<Account>> GetUsersAsync()
        {
            return await _context.Accounts
                .AsNoTracking()
                .OrderByDescending(u => u.CreateAt)
                .ToListAsync();
        }// Thống kê tài khoản theo vai trò
        public async Task<Dictionary<string, int>> GetUsersByRoleAsync()
        {
            return await _context.Accounts
                .AsNoTracking()
                .GroupBy(a => a.Role)
                .Select(g => new
                {
                    Role = g.Key,
                    Total = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.Role,
                    x => x.Total);
        }

        // Thống kê địa điểm theo khu vực
        // Thống kê địa điểm theo tỉnh/thành phố từ địa chỉ
        public async Task<Dictionary<string, int>> GetLocationsByAreaAsync()
        {
            var locations = await _context.Locations
                .AsNoTracking()
                .Select(l => l.Address)
                .ToListAsync();

            return locations
                .GroupBy(address =>
                {
                    if (string.IsNullOrWhiteSpace(address))
                        return "Chưa xác định";

                    var parts = address.Split(
                        ',',
                        StringSplitOptions.TrimEntries |
                        StringSplitOptions.RemoveEmptyEntries);

                    return parts.Length > 0
                        ? parts[^1]
                        : "Chưa xác định";
                })
                .OrderByDescending(group => group.Count())
                .ToDictionary(
                    group => group.Key,
                    group => group.Count());
        }
    }
}
