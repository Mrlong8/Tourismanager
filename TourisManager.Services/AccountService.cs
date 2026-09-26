using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext db;

        public  AccountService(AppDbContext _db)
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
        // tìm kiếm theo id
        public async  Task<Account?> FindByIdAsync(string accountId)
        {
            return await db.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    a => a.AccountId == accountId
                );
        }

        // Tìm kiếm theo Username hoặc Email
        public async Task<Account> FindByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail);
        }

        // tìm kiếm username bị trùng
        public async Task<bool> IsUserNameExistsAsync(string UserName)
        {
            var userName = await db.Accounts.FirstOrDefaultAsync(a => a.Username == UserName);
            if (userName != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // kiểm tra email bị trùng
        public async Task<bool> IsUserEmailExistsAsync(string Email)
        {
            var userName = await db.Accounts.FirstOrDefaultAsync(a => a.Email == Email);
            if (userName != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UpdateAsync(Account account)
        {
            try
            {
                db.Entry(account).State = EntityState.Modified;
                return await db.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool Success,string Message)> ChangePasswordAsync(string accountID, string newPassword, string CurentPassword)
        {
            var account = await db.Accounts.FindAsync(accountID);
            if(account == null)
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