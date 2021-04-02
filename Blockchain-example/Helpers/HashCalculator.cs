using System;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain_example.Helpers
{
    public static class HashCalculator
    {
        public static string CalculateHash(string previousHash, DateTime timestamp, long proof)
        {
            var blockData = previousHash + timestamp + proof;

            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockData));
            StringBuilder builder = new StringBuilder();

            foreach (var _byte in bytes)
            {
                builder.Append(_byte.ToString("x2")); // x2 -> format in hexadecimal
            }

            return builder.ToString();
        }
    }
}