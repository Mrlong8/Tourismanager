
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
        }
    }
}
