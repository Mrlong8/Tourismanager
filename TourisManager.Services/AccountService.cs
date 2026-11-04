using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TourisManager.Core.Entities;
using TourisManager.Data;
using TourisManager.Services;

namespace TourisManager.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext db;

        public AccountService(AppDbContext _db)
        {
            db = _db;
        }

        public async Task<bool> CreateAsync(Account account)
        {
            try
            {
                var lastAccount = db.Accounts
                    .Where(a => a.AccountId.StartsWith("acc-"))
                    .OrderByDescending(a => a.AccountId)
                    .FirstOrDefault();

                int nextId = 1;
                if (lastAccount != null && lastAccount.AccountId.Length > 4)
                {
                    int.TryParse(lastAccount.AccountId.Substring(4), out nextId);
                    nextId++;
                }

                account.AccountId = $"acc-{nextId:D2}";
                // Chỉ gán mặc định nếu đối tượng truyền vào chưa có giá trị
                if (string.IsNullOrEmpty(account.Role))
                {
                    account.Role = "User";
                }

                if (string.IsNullOrEmpty(account.AuthProvider))
                {
                    account.AuthProvider = "Local"; // Nếu đăng nhập thường thì mới là Local
                }
                account.CreateAt = DateTime.Now;

                await db.Accounts.AddAsync(account);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== LỖI THÊM ACCOUNT ===");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Chi tiết: " + ex.InnerException.Message);
                }
                return false;
            }
        }

        public async Task<Account?> LoginAsync(string usernameOrEmail, string password)
        {
            var account = await db.Accounts.FirstOrDefaultAsync(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail);
            if (account != null && !string.IsNullOrEmpty(account.Password))
            {
                bool isValue = BCrypt.Net.BCrypt.Verify(password, account.Password);
                if (isValue)
                {
                    return account;
                }
            }
            return null;
        }

        // Tìm kiếm theo id
        public async Task<Account?> FindByIdAsync(string accountId)
        {
            return await db.Accounts
                .Include(ac => ac.DetailAccount)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    a => a.AccountId == accountId
                );
        }

        // Tìm kiếm theo Username hoặc Email
        public async Task<Account?> FindByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail);
        }

        // Tìm kiếm username bị trùng
        public async Task<bool> IsUserNameExistsAsync(string UserName)
        {
            var userName = await db.Accounts.FirstOrDefaultAsync(a => a.Username == UserName);
            return userName != null;
        }

        // Kiểm tra email bị trùng
        public async Task<bool> IsUserEmailExistsAsync(string Email)
        {
            var userName = await db.Accounts.FirstOrDefaultAsync(a => a.Email == Email);
            return userName != null;
        }

        // Cập nhật thông tin tài khoản cơ bản (dùng chung, an toàn cho AccountController và upload Avatar)
        public async Task<bool> UpdateAsync(Account account)
        {
            try
            {
                var existingAccount = await db.Accounts.FirstOrDefaultAsync(a => a.AccountId == account.AccountId);
                if (existingAccount == null) return false;

                if (!string.IsNullOrEmpty(account.Name))
                {
                    existingAccount.Name = account.Name;
                }
                if (!string.IsNullOrEmpty(account.AvataUrl))
                {
                    existingAccount.AvataUrl = account.AvataUrl;
                }
                if (!string.IsNullOrEmpty(account.Role))
                {
                    existingAccount.Role = account.Role;
                }
                if (!string.IsNullOrEmpty(account.AuthProvider))
                {
                    existingAccount.AuthProvider = account.AuthProvider;
                }
                if (!string.IsNullOrEmpty(account.ProviderKey))
                {
                    existingAccount.ProviderKey = account.ProviderKey;
                }

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI UPDATE ACCOUNT: {ex.Message} ===");
                return false;
            }
        }

        // Cập nhật hồ sơ cá nhân và bảng DetailAccount (hàm riêng biệt cho Profile)
        public async Task<bool> UpdateProfileAsync(Account account)
        {
            try
            {
                // 1. Tìm account hiện tại trong DB kèm theo DetailAccount
                var existingAccount = await db.Accounts
                    .Include(a => a.DetailAccount)
                    .FirstOrDefaultAsync(a => a.AccountId == account.AccountId);

                if (existingAccount == null) return false;

                // 2. Cập nhật các trường của Account
                existingAccount.Name = account.Name;
                if (!string.IsNullOrEmpty(account.AvataUrl))
                {
                    existingAccount.AvataUrl = account.AvataUrl;
                }

                // 3. Xử lý bảng DetailAccount
                if (account.DetailAccount != null)
                {
                    if (existingAccount.DetailAccount == null)
                    {
                        // Nếu DB chưa có DetailAccount -> Thêm mới
                        var newDetail = new DetailAccount
                        {
                            DetailId = Guid.NewGuid().ToString(),
                            AccountId = existingAccount.AccountId,
                            SDT = account.DetailAccount.SDT,
                            Address = account.DetailAccount.Address,
                            Latitude = account.DetailAccount.Latitude,
                            Longitude = account.DetailAccount.Longitude
                        };
                        await db.DetailAccounts.AddAsync(newDetail);
                        existingAccount.DetailAccount = newDetail;
                    }
                    else
                    {
                        // Nếu DB đã có DetailAccount -> Cập nhật dữ liệu
                        existingAccount.DetailAccount.SDT = account.DetailAccount.SDT;
                        existingAccount.DetailAccount.Address = account.DetailAccount.Address;
                        existingAccount.DetailAccount.Latitude = account.DetailAccount.Latitude;
                        existingAccount.DetailAccount.Longitude = account.DetailAccount.Longitude;
                    }
                }

                // 4. Lưu thay đổi
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LỖI UPDATE PROFILE: {ex.Message} ===");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Chi tiết: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        // Overload cập nhật Profile trực tiếp từ các tham số
        public async Task<bool> UpdateProfileAsync(string accountId, string name, string? sdt, string? address, double? latitude, double? longitude)
        {
            var account = new Account
            {
                AccountId = accountId,
                Name = name,
                DetailAccount = new DetailAccount
                {
                    AccountId = accountId,
                    SDT = sdt,
                    Address = address,
                    Latitude = latitude,
                    Longitude = longitude
                }
            };
            return await UpdateProfileAsync(account);
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string accountID, string newPassword, string CurentPassword)
        {
            var account = await db.Accounts.FindAsync(accountID);
            if (account == null)
            {
                return (false, "Không tìm thấy thông tin tài khoản!");
            }
            // Kiểm tra tài khoản Google OAuth (Không có mật khẩu local)
            if (!string.IsNullOrEmpty(account.AuthProvider) && account.AuthProvider.Equals("Google", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Tài khoản đăng nhập bằng Google không thể đổi mật khẩu tại đây!");
            }

            bool isCurrentPassValid = BCrypt.Net.BCrypt.Verify(CurentPassword, account.Password);
            if (!isCurrentPassValid)
            {
                return (false, "Mật khẩu hiện tại không chính xác!");
            }
            if (BCrypt.Net.BCrypt.Verify(newPassword, account.Password))
            {
                return (false, "Mật khẩu mới không được trùng với mật khẩu hiện tại!");
            }

            account.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            db.Accounts.Update(account);
            await db.SaveChangesAsync();

            return (true, "Đổi mật khẩu thành công!");
        }
    }
}
