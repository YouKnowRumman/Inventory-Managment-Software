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
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseSeeder(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Only seed if no inventories exist
                var existingCount = _db.Inventories.Count();
                if (existingCount > 0)
                    return;

                // Create default seed user
                var seedUser = await _userManager.FindByNameAsync("seeduser");
                if (seedUser == null)
                {
                    seedUser = new ApplicationUser
                    {
                        UserName = "seeduser",
                        Email = "seed@omnivault.com",
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(seedUser, "Seed@123456");
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"Failed to create seed user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        return;
                    }
                }

                var defaultUserId = seedUser.Id;

                // Create inventories
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
                    AccessList = JsonSerializer.Serialize(new string[] { }),
                    RowVersion = new byte[8]
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
                    AccessList = JsonSerializer.Serialize(new string[] { }),
                    RowVersion = new byte[8]
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
                    AccessList = JsonSerializer.Serialize(new[] { "user-admin" }),
                    RowVersion = new byte[8]
                };

                var inv4 = new Inventory
                {
                    Id = Guid.Parse("a0000000-0000-0000-0000-000000000004"),
                    Title = "Art Supplies",
                    Description = "Collection of professional art materials, paint, brushes, and canvas. High-quality supplies for creative professionals.",
                    Category = "Equipment",
                    ImageUrl = "https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=400&h=300&fit=crop",
                    OwnerId = defaultUserId,
                    IsPublic = true,
                    Tags = JsonSerializer.Serialize(new[] { "art", "supplies", "creative" }),
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-15),
                    AccessList = JsonSerializer.Serialize(new string[] { }),
                    RowVersion = new byte[8]
                };

                var inv5 = new Inventory
                {
                    Id = Guid.Parse("a0000000-0000-0000-0000-000000000005"),
                    Title = "Photography Equipment",
                    Description = "Professional cameras, lenses, tripods, and lighting equipment. Complete photography toolkit for studio and field work.",
                    Category = "Equipment",
                    ImageUrl = "https://images.unsplash.com/photo-1502920917128-1aa500764cbd?w=400&h=300&fit=crop",
                    OwnerId = defaultUserId,
                    IsPublic = true,
                    Tags = JsonSerializer.Serialize(new[] { "photography", "camera", "equipment", "tech" }),
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10),
                    AccessList = JsonSerializer.Serialize(new string[] { }),
                    RowVersion = new byte[8]
                };

                _db.Inventories.AddRange(inv1, inv2, inv3, inv4, inv5);
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
                    Data = JsonSerializer.Serialize(new { Model = "XPS 13", SerialNumber = "ABC123456", PurchaseDate = "2023-01-15", Condition = "Excellent" }),
                    LikeCount = 2,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2" }),
                    RowVersion = new byte[8]
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
                    Data = JsonSerializer.Serialize(new { Model = "HP E243i", Resolution = "1920x1200", PurchaseDate = "2023-02-20", Status = "Active" }),
                    LikeCount = 1,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1" }),
                    RowVersion = new byte[8]
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
                    Data = JsonSerializer.Serialize(new { Model = "Corsair K95", Switches = "Cherry MX Brown", PurchaseDate = "2023-03-10", RGB = "Enabled" }),
                    LikeCount = 3,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3" }),
                    RowVersion = new byte[8]
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000004"),
                    CustomId = "OE-004",
                    InventoryId = inv1.Id,
                    Title = "Wireless Mouse",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-25),
                    Data = JsonSerializer.Serialize(new { Model = "Logitech MX Master 3S", DPI = "8000", Battery = "Full" }),
                    LikeCount = 2,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-2", "user-3" }),
                    RowVersion = new byte[8]
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000005"),
                    CustomId = "OE-005",
                    InventoryId = inv1.Id,
                    Title = "USB-C Hub",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-24),
                    UpdatedAt = DateTime.UtcNow.AddDays(-24),
                    Data = JsonSerializer.Serialize(new { Model = "CalDigit Thunderbolt 3", Ports = "15 ports", Location = "Desk" }),
                    LikeCount = 1,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-4" }),
                    RowVersion = new byte[8]
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000006"),
                    CustomId = "LB-001",
                    InventoryId = inv2.Id,
                    Title = "Pride and Prejudice",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-23),
                    UpdatedAt = DateTime.UtcNow.AddDays(-23),
                    Data = JsonSerializer.Serialize(new { Author = "Jane Austen", ISBN = "978-0141439518", PageCount = 432, Edition = "First Edition 1813" }),
                    LikeCount = 5,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3", "user-4", "user-5" }),
                    RowVersion = new byte[8]
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000007"),
                    CustomId = "LB-002",
                    InventoryId = inv2.Id,
                    Title = "The Great Gatsby",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-22),
                    UpdatedAt = DateTime.UtcNow.AddDays(-22),
                    Data = JsonSerializer.Serialize(new { Author = "F. Scott Fitzgerald", ISBN = "978-0743273565", PageCount = 180, Year = 1925 }),
                    LikeCount = 4,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3", "user-4" }),
                    RowVersion = new byte[8]
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000008"),
                    CustomId = "LB-003",
                    InventoryId = inv2.Id,
                    Title = "1984",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-21),
                    UpdatedAt = DateTime.UtcNow.AddDays(-21),
                    Data = JsonSerializer.Serialize(new { Author = "George Orwell", ISBN = "978-0451524935", PageCount = 328, Condition = "Good" }),
                    LikeCount = 6,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3", "user-4", "user-5", "user-6" }),
                    RowVersion = new byte[8]
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000009"),
                    CustomId = "LB-004",
                    InventoryId = inv2.Id,
                    Title = "To Kill a Mockingbird",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-20),
                    Data = JsonSerializer.Serialize(new { Author = "Harper Lee", ISBN = "978-0061120084", PageCount = 281, Location = "Shelf 2" }),
                    LikeCount = 7,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-1", "user-2", "user-3", "user-4", "user-5", "user-6", "user-7" }),
                    RowVersion = new byte[8]
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000010"),
                    CustomId = "ART-001",
                    InventoryId = inv4.Id,
                    Title = "Winsor Newton Professional Oil Paints",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    UpdatedAt = DateTime.UtcNow.AddDays(-18),
                    Data = JsonSerializer.Serialize(new { Type = "Oil Paint", Colors = "24 colors", Quality = "Professional Grade" }),
                    LikeCount = 3,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-3", "user-5", "user-6" }),
                    RowVersion = new byte[8]
                },
                new Item
                {
                    Id = Guid.Parse("b0000000-0000-0000-0000-000000000011"),
                    CustomId = "ART-002",
                    InventoryId = inv4.Id,
                    Title = "Premium Sable Brushes Set",
                    CreatedById = defaultUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-17),
                    UpdatedAt = DateTime.UtcNow.AddDays(-17),
                    Data = JsonSerializer.Serialize(new { Type = "Sable Hair", SetSize = "10 brushes", Purpose = "Fine Details" }),
                    LikeCount = 2,
                    LikedBy = JsonSerializer.Serialize(new[] { "user-4", "user-7" }),
                    RowVersion = new byte[8]
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
                    ItemId = items[0].Id,
                    Text = "Highly recommend this model!",
                    CreatedById = "user-3",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[2].Id,
                    Text = "Best keyboard I have ever used",
                    CreatedById = "user-1",
                    CreatedAt = DateTime.UtcNow.AddDays(-12)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[2].Id,
                    Text = "The switches are very responsive",
                    CreatedById = "user-4",
                    CreatedAt = DateTime.UtcNow.AddDays(-8)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[5].Id,
                    Text = "A timeless classic! Must read.",
                    CreatedById = "user-2",
                    CreatedAt = DateTime.UtcNow.AddDays(-11)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[5].Id,
                    Text = "The romance and wit are simply unmatched",
                    CreatedById = "user-5",
                    CreatedAt = DateTime.UtcNow.AddDays(-9)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[6].Id,
                    Text = "Fitzgerald's masterpiece",
                    CreatedById = "user-3",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[8].Id,
                    Text = "Harper Lee is brilliant. Highly recommended.",
                    CreatedById = "user-6",
                    CreatedAt = DateTime.UtcNow.AddDays(-6)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    InventoryId = inv2.Id,
                    Text = "Excellent collection of classic literature!",
                    CreatedById = "user-3",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[9].Id,
                    Text = "Professional quality paints, worth every penny",
                    CreatedById = "user-5",
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    ItemId = items[10].Id,
                    Text = "Perfect for detailed artwork",
                    CreatedById = "user-6",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };

            _db.Comments.AddRange(comments);
            await _db.SaveChangesAsync();

            // Add sample likes
            var likes = new[]
            {
                new Like { ItemId = items[0].Id, UserId = "user-1", CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Like { ItemId = items[0].Id, UserId = "user-2", CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Like { ItemId = items[2].Id, UserId = "user-1", CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Like { ItemId = items[2].Id, UserId = "user-2", CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Like { ItemId = items[2].Id, UserId = "user-3", CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Like { ItemId = items[5].Id, UserId = "user-1", CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Like { ItemId = items[5].Id, UserId = "user-2", CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Like { ItemId = items[5].Id, UserId = "user-3", CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Like { ItemId = items[5].Id, UserId = "user-4", CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Like { ItemId = items[5].Id, UserId = "user-5", CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Like { ItemId = items[6].Id, UserId = "user-1", CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Like { ItemId = items[6].Id, UserId = "user-2", CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Like { ItemId = items[6].Id, UserId = "user-3", CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Like { ItemId = items[6].Id, UserId = "user-4", CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Like { ItemId = items[7].Id, UserId = "user-1", CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Like { ItemId = items[7].Id, UserId = "user-2", CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Like { ItemId = items[7].Id, UserId = "user-3", CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Like { ItemId = items[7].Id, UserId = "user-4", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Like { ItemId = items[7].Id, UserId = "user-5", CreatedAt = DateTime.UtcNow.AddDays(-4) },
                new Like { ItemId = items[7].Id, UserId = "user-6", CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new Like { ItemId = items[8].Id, UserId = "user-1", CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Like { ItemId = items[8].Id, UserId = "user-2", CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Like { ItemId = items[8].Id, UserId = "user-3", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Like { ItemId = items[8].Id, UserId = "user-4", CreatedAt = DateTime.UtcNow.AddDays(-4) },
                new Like { ItemId = items[8].Id, UserId = "user-5", CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new Like { ItemId = items[8].Id, UserId = "user-6", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Like { ItemId = items[8].Id, UserId = "user-7", CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Like { ItemId = items[9].Id, UserId = "user-3", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Like { ItemId = items[9].Id, UserId = "user-5", CreatedAt = DateTime.UtcNow.AddDays(-4) },
                new Like { ItemId = items[9].Id, UserId = "user-6", CreatedAt = DateTime.UtcNow.AddDays(-3) },
            };

            _db.Likes.AddRange(likes);
            await _db.SaveChangesAsync();

                Console.WriteLine("✅ Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database seeding failed: {ex.Message}");
                throw;
            }
        }
    }
}
