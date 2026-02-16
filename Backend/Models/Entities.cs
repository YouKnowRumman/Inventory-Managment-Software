using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSoftware.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Additional profile fields can go here
        public bool IsBlocked { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = null!;
    }

    public class Tag
    {
        public int Id { get; set; }
        [Required] public string Value { get; set; } = null!;
        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
    }

    public class InventoryTag
    {
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;
        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }

    public class Inventory
    {
        public Guid Id { get; set; }
        [Required] public string Title { get; set; } = null!;
        public string? Description { get; set; } // Markdown
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? ImageUrl { get; set; } // only URL
        public bool IsPublic { get; set; } = true;

        // Template stored as JSON: descriptions of allowed custom fields & limits (enforced in service/validation)
        [Column(TypeName = "jsonb")]
        public Dictionary<string, object>? Template { get; set; }

        public string OwnerId { get; set; } = null!;
        public ApplicationUser? Owner { get; set; }

        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<InventoryWhitelistEntry> Whitelist { get; set; } = new List<InventoryWhitelistEntry>();

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        // Generated tsvector column (handled by migration / EF model configuration)
        public string? SearchVector { get; set; }
    }

    public class InventoryWhitelistEntry
    {
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
    }

    public class Item
    {
        public Guid Id { get; set; }

        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;

        [Required]
        public string CustomId { get; set; } = null!; // part of unique composite index with InventoryId

        public string? Title { get; set; }
        public string? Description { get; set; } // Markdown

        [Column(TypeName = "jsonb")]
        public Dictionary<string, object?>? CustomFields { get; set; } // variable custom fields

        public string CreatedById { get; set; } = null!;
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public string? SearchVector { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public ApplicationUser? Author { get; set; }
        [Required] public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class Like
    {
        public Guid ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}