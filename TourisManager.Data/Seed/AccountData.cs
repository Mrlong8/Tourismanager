using System;
using System.Collections.Generic;
using TourisManager.Core.Entities;

namespace TourisManager.Data.Seed
{
    public static class AccountData
    {
        public static List<Account> GetAccounts()
        {
            return new List<Account>
            {
                new Account
                {
                    AccountId = "acc-01",
                    Username = "admin",
                    Name = "Quản trị viên Hệ thống",
                    Email = "admin@tourismanager.com",
                    Password = "hashed_admin_password_123",
                    Role = "Admin",
                    AvataUrl = "/Image/Upload/u1.jpg",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 1, 1, 8, 0, 0)
                },
                new Account
                {
                    AccountId = "acc-02",
                    Username = "thuha_seller",
                    Name = "Trần Thị Thu Hà",
                    Email = "thuha.seller@gmail.com",
                    Password = "hashed_seller_password_456",
                    Role = "Seller",
                    AvataUrl = "/Image/Upload/u10.jpeg",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 1, 15, 9, 30, 0)
                },
                new Account
                {
                    AccountId = "acc-03",
                    Username = "hungnguyen",
                    Name = "Nguyễn Văn Hùng",
                    Email = "hungnguyen.seller@gmail.com",
                    Password = "hashed_seller_password_789",
                    Role = "Seller",
                    AvataUrl = "/Image/Upload/u11.jpg",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 2, 1, 10, 15, 0)
                },
                new Account
                {
                    AccountId = "acc-04",
                    Username = null,
                    Name = "Lê Hoàng Nam",
                    Email = "nam.lehoang@gmail.com",
                    Password = null,
                    Role = "User",
                    AvataUrl = "/Image/Upload/u12.jpg",
                    AuthProvider = "Google",
                    ProviderKey = "google-sub-10293847561",
                    CreateAt = new DateTime(2026, 2, 10, 14, 20, 0)
                },
                new Account
                {
                    AccountId = "acc-05",
                    Username = "hoanganh",
                    Name = "Phạm Hoàng Anh",
                    Email = "hoanganh.pham@gmail.com",
                    Password = "hashed_user_password_111",
                    Role = "User",
                    AvataUrl = "/Image/Upload/u13.jpg",
                    AuthProvider = "Local",
                    ProviderKey = null,
                    CreateAt = new DateTime(2026, 2, 20, 11, 45, 0)
                }
            };
        }
    }
}