using System;
using System.Linq;
using InventoryManagementSoftware.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace InventoryManagementSoftware.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Like> Likes => Set<Like>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Inventory <-> Tag many-to-many
            builder.Entity<InventoryTag>().HasKey(it => new { it.InventoryId, it.TagId });
            builder.Entity<InventoryTag>()
                .HasOne(p => p.Inventory).WithMany(p => p.InventoryTags).HasForeignKey(p => p.InventoryId);
            builder.Entity<InventoryTag>()
                .HasOne(p => p.Tag).WithMany(p => p.InventoryTags).HasForeignKey(p => p.TagId);

            // Whitelist
            builder.Entity<InventoryWhitelistEntry>().HasKey(e => new { e.InventoryId, e.UserId });
            builder.Entity<InventoryWhitelistEntry>()
                .HasOne(w => w.Inventory).WithMany(i => i.Whitelist).HasForeignKey(w => w.InventoryId);

            // Composite unique index for custom id per inventory
            builder.Entity<Item>()
                .HasIndex(i => new { i.InventoryId, i.CustomId })
                .IsUnique();

            // Likes unique per user per item
            builder.Entity<Like>().HasKey(l => new { l.ItemId, l.UserId });

            // Search vector columns: Use Postgres tsvector and GIN indexes
            builder.Entity<Inventory>()
                .Property<string>("SearchVector")
                .HasColumnType("tsvector");
            builder.Entity<Inventory>()
                .HasIndex("SearchVector")
                .HasMethod("GIN");

            builder.Entity<Item>()
                .Property<string>("SearchVector")
                .HasColumnType("tsvector");
            builder.Entity<Item>()
                .HasIndex("SearchVector")
                .HasMethod("GIN");

            // Configure jsonb storage for template/custom fields
            builder.Entity<Inventory>().Property(i => i.Template).HasColumnType("jsonb");
            builder.Entity<Item>().Property(i => i.CustomFields).HasColumnType("jsonb");

            // Concurrency tokens (RowVersion) are configured via [Timestamp] attribute on entities

            // Additional model configuration omitted for brevity: seed categories, constraints, etc.
        }
    }
}