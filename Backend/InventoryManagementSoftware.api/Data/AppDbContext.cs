using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSoftware.Domain.Entities;

namespace InventoryManagementSoftware.api.Data
{
    public class AppDbContext : IdentityDbContext<InventoryManagementSoftware.Domain.Entities.ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Inventory configuration
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Owner)
                .WithMany()
                .HasForeignKey(i => i.OwnerId)
                .IsRequired(false);

            modelBuilder.Entity<Inventory>()
                .HasMany(i => i.Items)
                .WithOne(it => it.Inventory)
                .HasForeignKey(it => it.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .HasMany(i => i.Comments)
                .WithOne(c => c.Inventory)
                .HasForeignKey(c => c.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Item configuration
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Inventory)
                .WithMany(inv => inv.Items)
                .HasForeignKey(i => i.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.CreatedBy)
                .WithMany()
                .HasForeignKey(i => i.CreatedById)
                .IsRequired(false);

            modelBuilder.Entity<Item>()
                .HasMany(i => i.Comments)
                .WithOne(c => c.Item)
                .HasForeignKey(c => c.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite unique index on InventoryId + CustomId
            modelBuilder.Entity<Item>()
                .HasIndex(i => new { i.InventoryId, i.CustomId })
                .IsUnique();

            // Comment configuration
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Item)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Inventory)
                .WithMany(inv => inv.Comments)
                .HasForeignKey(c => c.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .IsRequired(false);

            // Like configuration
            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.ItemId, l.UserId });

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Item)
                .WithMany()
                .HasForeignKey(l => l.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure jsonb columns for PostgreSQL
            modelBuilder.Entity<Inventory>()
                .Property(i => i.Tags)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Inventory>()
                .Property(i => i.FieldDefinitions)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Inventory>()
                .Property(i => i.CustomIdTemplate)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Inventory>()
                .Property(i => i.AccessList)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Item>()
                .Property(i => i.Data)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Item>()
                .Property(i => i.LikedBy)
                .HasColumnType("jsonb");
        }
    }
}
