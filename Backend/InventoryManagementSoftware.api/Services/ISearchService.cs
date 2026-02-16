using InventoryManagementSoftware.api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResultDto>> GlobalSearchAsync(string query, int limit = 20);
        Task<IEnumerable<SearchResultDto>> SearchInventoriesAsync(string query, int limit = 20);
        Task<IEnumerable<SearchResultDto>> SearchItemsAsync(string query, int limit = 20);
        Task<IEnumerable<string>> AutocompleteTagsAsync(string prefix, int limit = 10);
    }
}
