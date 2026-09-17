using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Services
{
    public class AccountServiceImpl : AccountService
    {
        private AppDbContext db;
        public AccountServiceImpl(AppDbContext _db)
        {
            db = _db;
        }
        public bool Create(User user)
        {
            try
            {
                // 1. Tự động tìm UserId lớn nhất trong CSDL và cộng lên 1 (Ví dụ: usr-10 -> usr-11)
                var lastUser = db.Users
                    .Where(u => u.UserId.StartsWith("usr-"))
                    .OrderByDescending(u => u.UserId)
                    .FirstOrDefault();

                int nextId = 1;
                if (lastUser != null && lastUser.UserId.Length > 4)
                {
                    int.TryParse(lastUser.UserId.Substring(4), out nextId);
                    nextId++;
                }

                user.UserId = $"usr-{nextId:D2}"; // Kết quả: usr-11

                // 2. Gán các giá trị mặc định cho cột bắt buộc trong CSDL
                user.Role = "User";
                user.AuthProvider = "Local";
                user.CreateAt = DateTime.Now;

                // 3. Thêm vào DbSet và lưu
                db.Users.Add(user);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                // In chi tiết lỗi ra cửa sổ Output của Visual Studio
                System.Diagnostics.Debug.WriteLine("=== LỖI THÊM USER ===");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine(ex.InnerException.Message);
                }
                return false;
            }
        }

        public bool Login(string email, string password)
        {
            var account = db.Users.SingleOrDefault(a => a.Email == email);
            if (account != null)
            {
                return BCrypt.Net.BCrypt.Verify(password,account.Password);
                // nếu sao sánh đúng trả về true
            }
            return false;

        }

        public User FinebyEmail(string email)
        {
            var account = db.Users.AsNoTracking().FirstOrDefault(u => u.Email == email);
            return account;
        }
        public bool Update(User user)
        {
            try
            {
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return db.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }


    }
}
