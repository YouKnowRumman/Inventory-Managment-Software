using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Data;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSoftware.api.Services
{
    public class CustomIdGeneratorService : ICustomIdGeneratorService
    {
        private readonly AppDbContext _context;

        public CustomIdGeneratorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateCustomIdAsync(Guid inventoryId, CustomIdGenerationDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Strategy))
                throw new ArgumentException("Invalid custom ID generation configuration");

            string generatedId = dto.Strategy switch
            {
                "FixedText" => dto.Format ?? "FIXED",
                "RandomBits20" => RandomBits(20),
                "RandomBits32" => RandomBits(32),
                "RandomDigits6" => RandomDigits(6),
                "RandomDigits9" => RandomDigits(9),
                "Guid" => Guid.NewGuid().ToString("N"),
                "DateTime" => DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                "Sequence" => await GenerateSequenceIdAsync(inventoryId),
                _ => throw new NotSupportedException($"Strategy '{dto.Strategy}' is not supported")
            };

            // Replace placeholders in format if provided
            if (!string.IsNullOrEmpty(dto.Format))
            {
                generatedId = dto.Format.Replace("{AUTO}", generatedId);
            }

            // Verify uniqueness
            if (!await IsCustomIdUniqueAsync(inventoryId, generatedId))
            {
                throw new Exception("Generated ID conflicts with existing entry. Please retry or provide manual ID.");
            }

            return generatedId;
        }

        public async Task<bool> IsCustomIdUniqueAsync(Guid inventoryId, string customId)
        {
            return !await _context.Items
                .AnyAsync(i => i.InventoryId == inventoryId && i.CustomId == customId);
        }

        private static string RandomBits(int bits)
        {
            int bytes = (bits + 7) / 8;
            Span<byte> buffer = stackalloc byte[8];
            RandomNumberGenerator.Fill(buffer.Slice(0, bytes));

            ulong value = 0;
            for (int i = 0; i < bytes; i++)
                value = (value << 8) | buffer[i];

            ulong mask = bits >= 64 ? ulong.MaxValue : ((1UL << bits) - 1);
            return (value & mask).ToString("X");
        }

        private static string RandomDigits(int digits)
        {
            var sb = new StringBuilder(digits);
            Span<byte> buffer = stackalloc byte[16];
            RandomNumberGenerator.Fill(buffer);

            for (int i = 0; i < digits; i++)
            {
                int digit = buffer[i] % 10;
                sb.Append((char)('0' + digit));
            }

            return sb.ToString();
        }

        private async Task<string> GenerateSequenceIdAsync(Guid inventoryId)
        {
            // Find the highest numeric CustomId for this inventory
            var items = await _context.Items
                .Where(i => i.InventoryId == inventoryId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            int nextSeq = 1;
            foreach (var item in items)
            {
                if (int.TryParse(item.CustomId.Split('-').LastOrDefault(), out int seq))
                {
                    nextSeq = seq + 1;
                    break;
                }
            }

            return nextSeq.ToString("D6"); // 6-digit padded sequence
        }
    }
}
