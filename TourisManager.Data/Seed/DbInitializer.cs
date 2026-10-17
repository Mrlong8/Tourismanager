using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;

namespace TourisManager.Data.Seed
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Tự động kiểm tra và áp dụng Migration nếu DB chưa được khởi tạo
            context.Database.Migrate();

     

            // 2. Seed Users (Nếu chưa có)
           
            if (!context.Accounts.Any())
            {
                var accounts = AccountData.GetAccounts();
                context.Accounts.AddRange(accounts);
                context.SaveChanges();
            }

            // 3. Seed Locations (Nếu chưa có)
            if (!context.Locations.Any())
            {
                var locations = LocationData.GetLocations();
                context.Locations.AddRange(locations);
                context.SaveChanges();
            }

            // 4. Seed Restaurants (Nếu chưa có)
          
        }
    }
}