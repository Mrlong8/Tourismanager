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
        public DbSet<DetailAccount> DetailAccounts { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodSize> FoodSizes { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<TypeFood> TypeFoods { get; set; }
        public DbSet<SavedFood> SavedFoods { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<LocationCategory> LocationCategories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ActivityStatus> ActivityStatuses { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationService> ReservationServices { get; set; }
        public DbSet<RoomChat> RoomChats { get; set; }
        public DbSet<Chat> Chats { get; set; }



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
            modelBuilder.Entity<OrderDetail>()
                 .HasKey(x => new
                 {
                     x.OrderId,
                     x.FoodId
                 });


            modelBuilder.Entity<FoodSize>()
                .HasKey(x => new
                {
                    x.FoodId,
                    x.SizeId
                });


            modelBuilder.Entity<SavedFood>()
                .HasKey(x => new
                {
                    x.FoodId,
                    x.AccountId
                });


            modelBuilder.Entity<LocationCategory>()
                .HasKey(x => new
                {
                    x.CategoryId,
                    x.LocationId
                });


            // =========================================
            // Account - DetailAccount
            // 1 - 1
            // =========================================

            modelBuilder.Entity<DetailAccount>()
                .HasOne(x => x.Account)
                .WithOne(x => x.DetailAccount)
                .HasForeignKey<DetailAccount>(
                    x => x.AccountId);


            // =========================================
            // Account - Order
            // 1 - N
            // =========================================

            modelBuilder.Entity<Order>()
                .HasOne(x => x.Account)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.AccountId);


            // =========================================
            // Location - Order
            // 1 - N
            // =========================================

            modelBuilder.Entity<Order>()
                .HasOne(x => x.Location)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // PaymentMethod - Order
            // 1 - N
            // =========================================

            modelBuilder.Entity<Order>()
                .HasOne(x => x.PaymentMethod)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.PaymentMethodId);


            // =========================================
            // Order - OrderDetail
            // 1 - N
            // =========================================

            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Order)
                .WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.NoAction); ;


            // =========================================
            // Food - OrderDetail
            // 1 - N
            // =========================================

            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Food)
                .WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.FoodId);


            // =========================================
            // Food - TypeFood
            // 1 - N
            // =========================================

            modelBuilder.Entity<Food>()
                .HasOne(x => x.TypeFood)
                .WithMany(x => x.Foods)
                .HasForeignKey(x => x.TypeFoodId);


            // =========================================
            // Location - Food
            // 1 - N
            // =========================================

            modelBuilder.Entity<Food>()
                .HasOne(x => x.Location)
                .WithMany(x => x.Foods)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // Food - FoodSize
            // =========================================

            modelBuilder.Entity<FoodSize>()
                .HasOne(x => x.Food)
                .WithMany(x => x.FoodSizes)
                .HasForeignKey(x => x.FoodId);


            // =========================================
            // Size - FoodSize
            // =========================================

            modelBuilder.Entity<FoodSize>()
                .HasOne(x => x.Size)
                .WithMany(x => x.FoodSizes)
                .HasForeignKey(x => x.SizeId);


            // =========================================
            // Account - SavedFood
            // =========================================

            modelBuilder.Entity<SavedFood>()
                .HasOne(x => x.Account)
                .WithMany(x => x.SavedFoods)
                .HasForeignKey(x => x.AccountId);


            // =========================================
            // Food - SavedFood
            // =========================================

            modelBuilder.Entity<SavedFood>()
                .HasOne(x => x.Food)
                .WithMany(x => x.SavedFoods)
                .HasForeignKey(x => x.FoodId);


            // =========================================
            // Account - Location
            // =========================================

            modelBuilder.Entity<Location>()
                .HasOne(x => x.Account)
                .WithMany(x => x.Locations)
                .HasForeignKey(x => x.AccountId);

            modelBuilder.Entity<Location>()
            .Property(x => x.Latitude)
            .HasPrecision(18, 6);

            modelBuilder.Entity<Location>()
                .Property(x => x.Longitude)
                .HasPrecision(18, 6);


            // =========================================
            // Area - Location
            // =========================================

            modelBuilder.Entity<Location>()
                .HasOne(x => x.Area)
                .WithMany(x => x.Locations)
                .HasForeignKey(x => x.AreaId);


            // =========================================
            // Account - Review
            // =========================================

            modelBuilder.Entity<Review>()
                .HasOne(x => x.Account)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.AccountId);


            // =========================================
            // Location - Review
            // =========================================

            modelBuilder.Entity<Review>()
                .HasOne(x => x.Location)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // Location - Image
            // =========================================

            modelBuilder.Entity<Image>()
                .HasOne(x => x.Location)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // Location - ActivityStatus
            // =========================================

            modelBuilder.Entity<ActivityStatus>()
                .HasOne(x => x.Location)
                .WithMany(x => x.ActivityStatuses)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // Location - Reservation
            // =========================================

            modelBuilder.Entity<Reservation>()
                .HasOne(x => x.Location)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // Account - ReservationService
            // =========================================

            modelBuilder.Entity<ReservationService>()
                .HasOne(x => x.Account)
                .WithMany(x => x.ReservationServices)
                .HasForeignKey(x => x.AccountId);


            // =========================================
            // Location - ReservationService
            // =========================================

            modelBuilder.Entity<ReservationService>()
                .HasOne(x => x.Location)
                .WithMany(x => x.ReservationServices)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // Order - ReservationService
            // =========================================

            modelBuilder.Entity<ReservationService>()
                .HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId);


            // =========================================
            // Location - RoomChat
            // =========================================

            modelBuilder.Entity<RoomChat>()
                .HasOne(x => x.Location)
                .WithMany(x => x.RoomChats)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // Account - RoomChat
            // =========================================

            modelBuilder.Entity<RoomChat>()
                .HasOne(x => x.Account)
                .WithMany(x => x.RoomChats)
                .HasForeignKey(x => x.AccountId);


            // =========================================
            // RoomChat - Chat
            // =========================================

            modelBuilder.Entity<Chat>()
                .HasOne(x => x.RoomChat)
                .WithMany(x => x.Chats)
                .HasForeignKey(x => x.RoomChatId);


            // =========================================
            // Category - LocationCategory
            // =========================================

            modelBuilder.Entity<LocationCategory>()
                .HasOne(x => x.Category)
                .WithMany(x => x.LocationCategories)
                .HasForeignKey(x => x.CategoryId);


            // =========================================
            // Location - LocationCategory
            // =========================================

            modelBuilder.Entity<LocationCategory>()
                .HasOne(x => x.Location)
                .WithMany(x => x.LocationCategories)
                .HasForeignKey(x => x.LocationId);


            // =========================================
            // DECIMAL
            // =========================================

            modelBuilder.Entity<Order>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);


            modelBuilder.Entity<OrderDetail>()
                .Property(x => x.TotalPrice)
                .HasPrecision(18, 2);


            modelBuilder.Entity<FoodSize>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);
        }
    }

   
}
