using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection.Emit;

namespace TourisManager.Models
{
    public partial class MyDbContext : DbContext
    {
        public MyDbContext() { }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public virtual DbSet<Destination> Destinations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Destination>().ToTable(nameof(Destinations));

        }


    }
}
