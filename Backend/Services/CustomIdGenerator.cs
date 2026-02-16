using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.Api.Services
{
    public enum CustomIdStrategy
    {
        FixedText,
        RandomBits20,
        RandomBits32,
        RandomDigits6,
        RandomDigits9,
        Guid,
        DateTime,
        Sequence
    }

    public interface ICustomIdGenerator
    {
        Task<string> GenerateAsync(Guid inventoryId, string format, CustomIdStrategy strategy);
    }

    public class CustomIdGenerator : ICustomIdGenerator
    {
        // Sequence strategy requires DB access; for demo, Sequence is not implemented here.
        // In production implement a DB-backed sequence per-inventory (serial table or SELECT ... FOR UPDATE)

        public Task<string> GenerateAsync(Guid inventoryId, string format, CustomIdStrategy strategy)
        {
            // format string can contain placeholders like {FIXED}, {RAND32}, {GUID}, {DATE:yyyyMMdd}, {SEQ}
            string result = strategy switch
            {
                CustomIdStrategy.FixedText => format,
                CustomIdStrategy.RandomBits20 => RandomBits(20),
                CustomIdStrategy.RandomBits32 => RandomBits(32),
                CustomIdStrategy.RandomDigits6 => RandomDigits(6),
                CustomIdStrategy.RandomDigits9 => RandomDigits(9),
                CustomIdStrategy.Guid => Guid.NewGuid().ToString("N"),
                CustomIdStrategy.DateTime => DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                _ => throw new NotSupportedException("Strategy not supported in this implementation.")
            };

            // allow basic format composition (replace {AUTO} with generated value)
            return Task.FromResult(format.Replace("{AUTO}", result));
        }

        private static string RandomBits(int bits)
        {
            int bytes = (bits + 7) / 8;
            Span<byte> buf = stackalloc byte[8];
            RandomNumberGenerator.Fill(buf.Slice(0, bytes));
            ulong value = 0;
            for (int i = 0; i < bytes; i++) value = (value << 8) | buf[i];
            ulong mask = bits >= 64 ? ulong.MaxValue : ((1UL << bits) - 1);
            return (value & mask).ToString("X");
        }

        private static string RandomDigits(int digits)
        {
            var sb = new StringBuilder(digits);
            Span<byte> buf = stackalloc byte[16];
            RandomNumberGenerator.Fill(buf);
            for (int i = 0; i < digits; i++)
            {
                int digit = buf[i] % 10;
                sb.Append((char)('0' + digit));
            }
            return sb.ToString();
        }
    }
}