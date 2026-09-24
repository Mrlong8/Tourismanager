using Microsoft.EntityFrameworkCore;
using TourisManager.Core.Entities;
namespace TourisManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // gọi constructer của lớp cha DbContext với các tùy chọn được cung cấp
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationImage> LocationImages { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewContent> ReviewContents { get; set; }
        public DbSet<Favorite> Favorites { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 1. Tự động chuyển các cột Id dạng string thành varchar(36) trong SQL Server để tối ưu dung lượng & hiệu năng
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.Name.EndsWith("Id") && property.ClrType == typeof(string))
                    {
                        property.SetColumnType("varchar(36)");
                    }
                }
            }
            // Location → Account
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Creator)
                .WithMany(a => a.Locations)
                .HasForeignKey(l => l.CreateBy)
                .HasPrincipalKey(a => a.AccountId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Location>()
                .Property(l => l.Latitude)
                .HasPrecision(18, 6);

            modelBuilder.Entity<Location>()
                .Property(l => l.Longitude)
                .HasPrecision(18, 6);

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Latitude)
                .HasPrecision(18, 6);

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Longitude)
                .HasPrecision(18, 6);
        }
    }

   
}
