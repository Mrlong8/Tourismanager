using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TourisManager.Data.Seed
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate();
            if (!context.Accounts.Any())
            {
                context.Accounts.AddRange(AccountData.GetAccounts());
                context.SaveChanges();
            }
            RestaurantSeed.Import(context, RestaurantSeed.FindCatalogPath());
        }
    }
}
