using InventoryManagementSoftware.api.Data;
using InventoryManagementSoftware.api.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public class SearchService : ISearchService
    {
        private readonly AppDbContext _context;

        public SearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SearchResultDto>> GlobalSearchAsync(string query, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchResultDto>();

            var normalizedQuery = query.ToLower().Trim();
            var results = new List<SearchResultDto>();

            // Search inventories
            var inventories = await _context.Inventories
                .Where(i => EF.Functions.ILike(i.Title, $"%{normalizedQuery}%") ||
                           EF.Functions.ILike(i.Description, $"%{normalizedQuery}%") ||
                           EF.Functions.ILike(i.Tags, $"%{normalizedQuery}%"))
                .Include(i => i.Owner)
                .Take(limit / 2)
                .ToListAsync();

            foreach (var inv in inventories)
            {
                results.Add(new SearchResultDto
                {
                    Type = "inventory",
                    Id = inv.Id,
                    Title = inv.Title,
                    Description = inv.Description,
                    Category = inv.Category,
                    CreatorName = inv.Owner?.UserName,
                    RelevanceScore = CalculateRelevance(inv.Title, normalizedQuery)
                });
            }

            // Search items
            var items = await _context.Items
                .Where(i => EF.Functions.ILike(i.Title, $"%{normalizedQuery}%") ||
                           EF.Functions.ILike(i.Data, $"%{normalizedQuery}%"))
                .Include(i => i.Inventory)
                .ThenInclude(inv => inv.Owner)
                .Take(limit / 2)
                .ToListAsync();

            foreach (var item in items)
            {
                results.Add(new SearchResultDto
                {
                    Type = "item",
                    Id = item.Id,
                    InventoryId = item.InventoryId,
                    Title = item.Title,
                    Category = item.Inventory?.Category,
                    CreatorName = item.Inventory?.Owner?.UserName,
                    RelevanceScore = CalculateRelevance(item.Title, normalizedQuery)
                });
            }

            return results.OrderByDescending(r => r.RelevanceScore).Take(limit);
        }

        public async Task<IEnumerable<SearchResultDto>> SearchInventoriesAsync(string query, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchResultDto>();

            var normalizedQuery = query.ToLower().Trim();

            var inventories = await _context.Inventories
                .Where(i => EF.Functions.ILike(i.Title, $"%{normalizedQuery}%") ||
                           EF.Functions.ILike(i.Description, $"%{normalizedQuery}%"))
                .Include(i => i.Owner)
                .Take(limit)
                .ToListAsync();

            return inventories.Select(i => new SearchResultDto
            {
                Type = "inventory",
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Category = i.Category,
                CreatorName = i.Owner?.UserName,
                ItemCount = i.Items.Count,
                RelevanceScore = CalculateRelevance(i.Title, normalizedQuery)
            });
        }

        public async Task<IEnumerable<SearchResultDto>> SearchItemsAsync(string query, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchResultDto>();

            var normalizedQuery = query.ToLower().Trim();

            var items = await _context.Items
                .Where(i => EF.Functions.ILike(i.Title, $"%{normalizedQuery}%") ||
                           EF.Functions.ILike(i.Data, $"%{normalizedQuery}%"))
                .Include(i => i.Inventory)
                .ThenInclude(inv => inv.Owner)
                .Take(limit)
                .ToListAsync();

            return items.Select(i => new SearchResultDto
            {
                Type = "item",
                Id = i.Id,
                InventoryId = i.InventoryId,
                Title = i.Title,
                Category = i.Inventory?.Category,
                CreatorName = i.Inventory?.Owner?.UserName,
                RelevanceScore = CalculateRelevance(i.Title, normalizedQuery)
            });
        }

        public async Task<IEnumerable<string>> AutocompleteTagsAsync(string prefix, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return new List<string>();

            var normalizedPrefix = prefix.ToLower().Trim();
            var inventories = await _context.Inventories.ToListAsync();
            var tags = new HashSet<string>();

            foreach (var inv in inventories)
            {
                if (!string.IsNullOrEmpty(inv.Tags))
                {
                    try
                    {
                        var tagsList = JsonSerializer.Deserialize<List<string>>(inv.Tags) ?? new List<string>();
                        foreach (var tag in tagsList)
                        {
                            if (tag.ToLower().StartsWith(normalizedPrefix))
                                tags.Add(tag);
                        }
                    }
                    catch { /* Ignore parsing errors */ }
                }
            }

            return tags.Take(limit);
        }

        private static double CalculateRelevance(string title, string query)
        {
            if (string.IsNullOrEmpty(title)) return 0;

            var lowerTitle = title.ToLower();
            int matches = 0;

            // Exact match gets highest score
            if (lowerTitle.Equals(query)) return 100;

            // Starts with gets high score
            if (lowerTitle.StartsWith(query)) return 50;

            // Count word matches
            var titleWords = lowerTitle.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            var queryWords = query.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var qWord in queryWords)
            {
                if (titleWords.Any(tWord => tWord.Contains(qWord)))
                    matches++;
            }

            return matches * 10;
        }
    }
}
