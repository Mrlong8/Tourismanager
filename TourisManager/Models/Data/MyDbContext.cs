using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection.Emit;
using TourisManager.Models.Entity;

namespace TourisManager.Models.Data
{
    public partial class MyDbContext : DbContext
    {
        public MyDbContext() { }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public virtual DbSet<Destination> Destinations { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Destination>().ToTable(nameof(Destinations));
            modelBuilder.Entity<User>().ToTable(nameof(Users));
        }


    }
}
