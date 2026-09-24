using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Services
{
    public class AccountServiceImpl : AccountService
    {
        private readonly AppDbContext db;

        public AccountServiceImpl(AppDbContext _db)
        {
            db = _db;
        }

        public bool Create(Account account)
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

                db.Accounts.Add(account);
                return db.SaveChanges() > 0;
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

        public Account? Login(string usernameOrEmail, string password)
        {
            var account = db.Accounts.FirstOrDefault(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail);
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
        public Account? FindById(string accountId)
        {
            return db.Accounts
                .AsNoTracking()
                .FirstOrDefault(
                    a => a.AccountId == accountId
                );
        }

        // Tìm kiếm theo Username hoặc Email
        public Account FindByUsernameOrEmail(string usernameOrEmail)
        {
            return db.Accounts.AsNoTracking().FirstOrDefault(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail);
        }

        // tìm kiếm username bị trùng
        public bool IsUserNameExists(string UserName)
        {
            var userName = db.Accounts.FirstOrDefault(a => a.Username == UserName);
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
        public bool IsUserEmailExists(string Email)
        {
            var userName = db.Accounts.FirstOrDefault(a => a.Email == Email);
            if (userName != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Update(Account account)
        {
            try
            {
                db.Entry(account).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        

    }
}