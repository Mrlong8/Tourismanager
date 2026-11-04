
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Core.Interfaces;
namespace TourisManager.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách tài khoản và tìm kiếm
        public async Task<(List<Account> Users, int TotalCount)> GetUsersAsync(
    string? searchString,
    int pageNumber,
    int pageSize)
        {
            var query = _context.Accounts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string keyword = searchString.Trim();

                query = query.Where(u =>
                    (u.Username != null && u.Username.Contains(keyword)) ||
                    (u.Name != null && u.Name.Contains(keyword)) ||
                    (u.Email != null && u.Email.Contains(keyword)));
            }

            int totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        // Tìm tài khoản theo ID
        public async Task<Account?> GetByIdAsync(string id)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(u => u.AccountId == id);
        }

        // Thêm tài khoản
        public async Task<bool> CreateAsync(Account user)
        {
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            if (string.IsNullOrEmpty(user.AccountId))
            {
                user.AccountId = Guid.NewGuid().ToString();
            }

            if (user.CreateAt == default)
            {
                user.CreateAt = DateTime.Now;
            }

            _context.Accounts.Add(user);

            return await _context.SaveChangesAsync() > 0;
        }

        // Cập nhật tài khoản
        public async Task<bool> UpdateAsync(string id, Account user)
        {
            var existingUser = await _context.Accounts
                .FirstOrDefaultAsync(u => u.AccountId == id);

            if (existingUser == null)
                return false;

            existingUser.Username = user.Username;
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.AvataUrl = user.AvataUrl;
            existingUser.AuthProvider = user.AuthProvider;

            // Chỉ đổi mật khẩu nếu nhập mật khẩu mới
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingUser.Password =
                    BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        // Xóa tài khoản
        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _context.Accounts
                .FirstOrDefaultAsync(u => u.AccountId == id);

            if (user == null)
                return false;

            _context.Accounts.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
