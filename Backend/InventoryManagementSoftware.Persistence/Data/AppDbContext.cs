using InventoryManagementSoftware.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSoftware.Persistence.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Inventory> Inventories { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure composite unique index on InventoryId + CustomId to enforce unique CustomId per inventory
            modelBuilder.Entity<Item>()
                .HasIndex(i => new { i.InventoryId, i.CustomId })
                .IsUnique();

            modelBuilder.Entity<Inventory>().Property<string?>("FieldDefinitions").HasColumnType("jsonb");
            modelBuilder.Entity<Item>().Property<string?>("Data").HasColumnType("jsonb");
        }
    }
}
