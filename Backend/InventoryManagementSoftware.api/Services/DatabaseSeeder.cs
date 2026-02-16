using InventoryManagementSoftware.api.Data;
using InventoryManagementSoftware.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _db;

        public DatabaseSeeder(AppDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            // Only seed if no inventories exist
            var existingCount = _db.Inventories.Count();
            if (existingCount > 0)
                return;

            // Create default user
            var defaultUserId = "seed-user-001";

            var inv1 = new Inventory
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                Title = "Office Equipment",
                Description = "Laptops, monitors, keyboards, and other office peripherals. Track all company equipment with custom fields for model, serial number, purchase date, and warranty information.",
                Category = "Equipment",
                ImageUrl = "https://images.unsplash.com/photo-1587825140708-dfaf72ae4b04?w=400&h=300&fit=crop",
                OwnerId = defaultUserId,
                IsPublic = true,
                Tags = JsonSerializer.Serialize(new[] { "equipment", "office", "technology" }),
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30),
                AccessList = JsonSerializer.Serialize(new string[] { })
            };

            var inv2 = new Inventory
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000002"),
                Title = "Library Books",
                Description = "Fiction and non-fiction collection for the office library. Manage book inventory with ISBN, author, publication date, and availability status.",
                Category = "Books",
                ImageUrl = "https://images.unsplash.com/photo-1507842217343-583f20270319?w=400&h=300&fit=crop",
                OwnerId = defaultUserId,
                IsPublic = true,
                Tags = JsonSerializer.Serialize(new[] { "books", "library", "knowledge" }),
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = DateTime.UtcNow.AddDays(-25),
                AccessList = JsonSerializer.Serialize(new string[] { })
            };

            var inv3 = new Inventory
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000003"),
                Title = "HR Documents",
                Description = "Employee records, contracts, and documentation. Confidential inventory for HR department.",
                Category = "Documents",
                ImageUrl = "https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=400&h=300&fit=crop",
                OwnerId = defaultUserId,
                IsPublic = false,
                Tags = JsonSerializer.Serialize(new[] { "hr", "confidential", "documents" }),
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20),
                AccessList = JsonSerializer.Serialize(new[] { "user-admin" })
            };

            _db.Inventories.AddRange(inv1, inv2, inv3);
            await _db.SaveChangesAsync();

            // Add items
            var items = new[]
            {
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                    CustomId = "OE-001",
                    InventoryId = inv1.Id,
                    Title = "Dell XPS 13 Laptop",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-28),
                    UpdatedAt = DateTime.UtcNow.AddDays(-28),
                    Data = JsonSerializer.Serialize(new { Model = "XPS 13", SerialNumber = "ABC123456", PurchaseDate = "2023-01-15" }),
                    LikeCount = 2,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2" })
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000002"),
                    CustomId = "OE-002",
                    InventoryId = inv1.Id,
                    Title = "HP Monitor 24 inch",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-27),
                    UpdatedAt = DateTime.UtcNow.AddDays(-27),
                    Data = JsonSerializer.Serialize(new { Model = "HP E243i", Resolution = "1920x1200", PurchaseDate = "2023-02-20" }),
                    LikeCount = 1,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1" })
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000003"),
                    CustomId = "OE-003",
                    InventoryId = inv1.Id,
                    Title = "Mechanical Keyboard",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-26),
                    UpdatedAt = DateTime.UtcNow.AddDays(-26),
                    Data = JsonSerializer.Serialize(new { Model = "Corsair K95", Switches = "Cherry MX Brown", PurchaseDate = "2023-03-10" }),
                    LikeCount = 3,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3" })
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000004"),
                    CustomId = "LB-001",
                    InventoryId = inv2.Id,
                    Title = "Pride and Prejudice",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-23),
                    UpdatedAt = DateTime.UtcNow.AddDays(-23),
                    Data = JsonSerializer.Serialize(new { Author = "Jane Austen", ISBN = "978-0141439518", PageCount = 432 }),
                    LikeCount = 5,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3", "user-4", "user-5" })
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000005"),
                    CustomId = "LB-002",
                    InventoryId = inv2.Id,
                    Title = "The Great Gatsby",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-22),
                    UpdatedAt = DateTime.UtcNow.AddDays(-22),
                    Data = JsonSerializer.Serialize(new { Author = "F. Scott Fitzgerald", ISBN = "978-0743273565", PageCount = 180 }),
                    LikeCount = 4,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3", "user-4" })
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000006"),
                    CustomId = "LB-003",
                    InventoryId = inv2.Id,
                    Title = "1984",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-21),
                    UpdatedAt = DateTime.UtcNow.AddDays(-21),
                    Data = JsonSerializer.Serialize(new { Author = "George Orwell", ISBN = "978-0451524935", PageCount = 328 }),
                    LikeCount = 6,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3", "user-4", "user-5", "user-6" })
                }
            };

            _db.Items.AddRange(items);
            await _db.SaveChangesAsync();

            // Add sample comments
            var comments = new[]
            {
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[0].Id,
                    Text = "Great laptop for development work!",
                    CreatedById = "user-1",
                    CreatedAt = DateTime.UtcNow.AddDays(-14)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[0].Id,
                    Text = "Perfect for remote work setup",
                    CreatedById = "user-2",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    InventoryId = inv2.Id,
                    Text = "Excellent collection of classic literature!",
                    CreatedById = "user-3",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                }
            };

            _db.Comments.AddRange(comments);
            await _db.SaveChangesAsync();
        }
    }
}
