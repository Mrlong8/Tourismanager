
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Core.Interfaces;
using System.Data;
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
                .OrderBy(u =>
                    u.Role == "Admin" ? 0 :
                    u.Role == "Seller" ? 1 : 2)

                // Hai Seller cũ được xếp đầu nhóm Seller
                .ThenBy(u =>
                    u.Username == "hungnguyen" ? 0 :
                    u.Username == "thuha_seller" ? 1 : 2)

                // seller003, seller004... seller271
                .ThenBy(u => u.Username)
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

        public async Task<(bool Success, string Message)> CreateSellersAndAssignLocationsAsync(
            string defaultPassword)
        {
            if (string.IsNullOrWhiteSpace(defaultPassword))
                return (false, "Mật khẩu không được để trống.");

            await using var transaction = await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

            try
            {
                // Lấy toàn bộ Location và Account
                var locations = await _context.Locations
                    .OrderBy(l => l.LocationId)
                    .ToListAsync();

                var accounts = await _context.Accounts.ToListAsync();

                // Kiểm tra số lượng Location
                if (locations.Count != 271)
                {
                    return (false,
                        $"Số lượng Location hiện tại là {locations.Count}, không phải 271. Không thực hiện.");
                }

                // Lấy tài khoản Seller hiện tại
                var sellers = accounts
                    .Where(a => a.Role == "Seller")
                    .OrderBy(a => a.Username)
                    .ToList();

                // Nếu đã hoàn thành trước đó thì không tạo lại
                if (sellers.Count == 271)
                {
                    bool completed = locations.All(l =>
                        l.AccountId != null &&
                        sellers.Any(s => s.AccountId == l.AccountId));

                    bool eachSellerHasOneLocation = sellers.All(s =>
                        locations.Count(l => l.AccountId == s.AccountId) == 1);

                    if (completed && eachSellerHasOneLocation)
                    {
                        return (true, "271 Seller đã được phân công đầy đủ. Không cần tạo thêm.");
                    }
                }

                // Chỉ cho phép chạy lần đầu với đúng 2 Seller
                if (sellers.Count != 2)
                {
                    return (false,
                        $"Hiện có {sellers.Count} Seller. Chức năng này yêu cầu đúng 2 Seller ban đầu.");
                }

                // Hai Seller hiện tại chưa được quản lý Location nào
                if (sellers.Any(s => locations.Any(l => l.AccountId == s.AccountId)))
                {
                    return (false,
                        "Có Seller đang quản lý Location. Vui lòng kiểm tra dữ liệu trước khi phân công.");
                }

                // Chỉ chấp nhận Location chưa gán hoặc đang gán Admin
                var accountIds = accounts.ToDictionary(
                    a => a.AccountId,
                    a => a.Role);

                bool invalidAssignment = locations.Any(l =>
                    l.AccountId != null &&
                    (!accountIds.ContainsKey(l.AccountId) ||
                     (accountIds[l.AccountId] != "Admin" &&
                      accountIds[l.AccountId] != "Seller")));

                if (invalidAssignment)
                {
                    return (false,
                        "Có Location đang liên kết với tài khoản không phải Admin hoặc Seller. Không thực hiện.");
                }

                // Kiểm tra tên tài khoản và email dự kiến
                var newUsernames = Enumerable.Range(3, 269)
                    .Select(i => $"seller{i:D3}")
                    .ToList();

                var newEmails = newUsernames
                    .Select(username => $"{username}@tourismanager.demo")
                    .ToList();

                bool usernameExists = accounts.Any(a =>
                    a.Username != null &&
                    newUsernames.Contains(a.Username));

                bool emailExists = accounts.Any(a =>
                    newEmails.Contains(a.Email));

                if (usernameExists || emailExists)
                {
                    return (false,
                        "Username hoặc Email Seller dự kiến đã tồn tại. Không tạo tài khoản để tránh trùng.");
                }

                // Tạo 269 Seller mới
                var newSellers = new List<Account>();

                for (int i = 3; i <= 271; i++)
                {
                    string username = $"seller{i:D3}";

                    newSellers.Add(new Account
                    {
                        AccountId = Guid.NewGuid().ToString(),
                        Username = username,
                        Name = $"Seller {i:D3}",
                        Email = $"{username}@tourismanager.demo",
                        Password = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                        Role = "Seller",
                        AuthProvider = "Local",
                        CreateAt = DateTime.Now
                    });
                }

                // Ghép 2 Seller cũ và 269 Seller mới
                sellers.AddRange(newSellers);

                // Kiểm tra đủ 271 Seller
                if (sellers.Count != locations.Count)
                {
                    return (false, "Số Seller không bằng số Location.");
                }

                // Phân công mỗi Seller đúng một Location
                for (int i = 0; i < locations.Count; i++)
                {
                    locations[i].AccountId = sellers[i].AccountId;
                }

                // Lưu tài khoản và Location trong cùng transaction
                await _context.Accounts.AddRangeAsync(newSellers);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return (true,
                    "Đã tạo 269 Seller mới và phân công đủ 271 Seller cho 271 Location.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return (false,
                    "Không thể tạo Seller: " + ex.Message);
            }
        }

    }
}
