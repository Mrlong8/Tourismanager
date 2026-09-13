using System;
using System.Collections.Generic;
using TourisManager.Core.Entities;

namespace TourisManager.Data.Seed
{
    public static class UserData
    {
        public static List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    UserId = "usr-01",
                    Name = "Quản trị viên Hệ thống",
                    Email = "admin@tourismanager.com",
                    Password = "hashed_admin_password_123",
                    Role = "Admin",
                    AvataUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=admin",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 1, 1, 8, 0, 0)
                },
                new User
                {
                    UserId = "usr-02",
                    Name = "Trần Thị Thu Hà",
                    Email = "thuha.seller@gmail.com",
                    Password = "hashed_seller_password_456",
                    Role = "Seller",
                    AvataUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=thuha",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 1, 15, 9, 30, 0)
                },
                new User
                {
                    UserId = "usr-03",
                    Name = "Nguyễn Văn Hùng",
                    Email = "hungnguyen.seller@gmail.com",
                    Password = "hashed_seller_password_789",
                    Role = "Seller",
                    AvataUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=hungnguyen",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 2, 1, 10, 15, 0)
                },
                new User
                {
                    UserId = "usr-04",
                    Name = "Lê Hoàng Nam",
                    Email = "nam.lehoang@gmail.com",
                    Password = null, // Đăng nhập Google không dùng password
                    Role = "User",
                    AvataUrl = "https://lh3.googleusercontent.com/a/ACg8ocL_namle",
                    AuthProvider = "Google",
                    ProviderKey = "google-sub-10293847561",
                    CreateAt = new DateTime(2026, 2, 10, 14, 20, 0)
                },
                new User
                {
                    UserId = "usr-05",
                    Name = "Phạm Hoàng Anh",
                    Email = "hoanganh.pham@gmail.com",
                    Password = "hashed_user_password_111",
                    Role = "User",
                    AvataUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=hoanganh",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 2, 20, 11, 45, 0)
                },
                new User
                {
                    UserId = "usr-06",
                    Name = "Đỗ Hải Yến",
                    Email = "haiyen.do@gmail.com",
                    Password = null,
                    Role = "User",
                    AvataUrl = "https://lh3.googleusercontent.com/a/ACg8ocL_haiyendo",
                    AuthProvider = "Google",
                    ProviderKey = "google-sub-98765432109",
                    CreateAt = new DateTime(2026, 3, 1, 16, 10, 0)
                },
                new User
                {
                    UserId = "usr-07",
                    Name = "Vũ Bảo Long",
                    Email = "baolong.vu@gmail.com",
                    Password = "hashed_user_password_222",
                    Role = "User",
                    AvataUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=baolong",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 3, 5, 13, 0, 0)
                },
                new User
                {
                    UserId = "usr-08",
                    Name = "Ngô Phương Thảo",
                    Email = "phuongthao.ngo@gmail.com",
                    Password = "hashed_user_password_333",
                    Role = "User",
                    AvataUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=phuongthao",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 3, 8, 17, 30, 0)
                },
                new User
                {
                    UserId = "usr-09",
                    Name = "Bùi Đình Trọng",
                    Email = "dinhtrong.bui@gmail.com",
                    Password = null,
                    Role = "User",
                    AvataUrl = "https://lh3.googleusercontent.com/a/ACg8ocL_dinhtrong",
                    AuthProvider = "Google",
                    ProviderKey = "google-sub-55443322110",
                    CreateAt = new DateTime(2026, 3, 10, 19, 0, 0)
                },
                new User
                {
                    UserId = "usr-10",
                    Name = "Đặng Minh Trí",
                    Email = "minhtri.dang@gmail.com",
                    Password = "hashed_user_password_444",
                    Role = "User",
                    AvataUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=minhtri",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 3, 12, 21, 15, 0)
                }
            };
        }
    }
}