// Méthodes d'extension utiles
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace BinaryAnalyzer.Utils
{
    public static class Extensions
    {
        public static string ToHex(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static string ToAscii(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Converts bytes to UTF-8 representation
        /// </summary>
        public static string ToUtf8(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Counts the frequency of each byte
        /// </summary>
        public static Dictionary<byte, int> GetByteFrequency(this byte[] bytes)
        {
            var frequency = new Dictionary<byte, int>();
            foreach (var b in bytes)
            {
                frequency[b] = frequency.ContainsKey(b) ? frequency[b] + 1 : 1;
            }
            return frequency;
        }

        /// <summary>
        /// Detects if data contains mainly printable ASCII text
        /// </summary>
        public static bool IsLikelyText(this byte[] bytes, double threshold = 0.7)
        {
            if (bytes.Length == 0) return false;
            
            int printableCount = bytes.Count(b => b >= 32 && b <= 126 || b == 9 || b == 10 || b == 13);
            return (double)printableCount / bytes.Length >= threshold;
        }

        /// <summary>
        /// Finds printable ASCII strings of minimum length
        /// </summary>
        public static IEnumerable<string> ExtractStrings(this byte[] bytes, int minLength = 4)
        {
            var current = new StringBuilder();
            
            foreach (var b in bytes)
            {
                if (b >= 32 && b <= 126) // Printable ASCII characters
                {
                    current.Append((char)b);
                }
                else
                {
                    if (current.Length >= minLength)
                    {
                        yield return current.ToString();
                    }
                    current.Clear();
                }
            }
            
            if (current.Length >= minLength)
            {
                yield return current.ToString();
            }
        }

        /// <summary>
        /// Calculates MD5 checksum of the data
        /// </summary>
        public static string GetMD5Hash(this byte[] bytes)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Calculates SHA256 checksum of the data
        /// </summary>
        public static string GetSHA256Hash(this byte[] bytes)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Searches for specific binary patterns
        /// </summary>
        public static List<int> FindPattern(this byte[] bytes, byte[] pattern)
        {
            var positions = new List<int>();
            
            for (int i = 0; i <= bytes.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (bytes[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    positions.Add(i);
                }
            }
            
            return positions;
        }

        /// <summary>
        /// Analyzes null byte zones (padding/alignment)
        /// </summary>
        public static IEnumerable<(int start, int length)> FindNullRuns(this byte[] bytes, int minLength = 4)
        {
            int start = -1;
            int length = 0;
            
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                {
                    if (start == -1) start = i;
                    length++;
                }
                else
                {
                    if (start != -1 && length >= minLength)
                    {
                        yield return (start, length);
                    }
                    start = -1;
                    length = 0;
                }
            }
            
            if (start != -1 && length >= minLength)
            {
                yield return (start, length);
            }
        }

        /// <summary>
        /// Formats data as hex dump style (like hexdump -C)
        /// </summary>
        public static string ToHexDump(this byte[] bytes, int bytesPerLine = 16)
        {
            var result = new StringBuilder();
            
            for (int i = 0; i < bytes.Length; i += bytesPerLine)
            {
                // Offset
                result.AppendFormat("{0:X8}  ", i);
                
                // Hex bytes
                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (i + j < bytes.Length)
                    {
                        result.AppendFormat("{0:X2} ", bytes[i + j]);
                    }
                    else
                    {
                        result.Append("   ");
                    }
                    
                    if (j == 7) result.Append(" "); // Separator in middle
                }
                
                result.Append(" |");
                
                // ASCII representation
                for (int j = 0; j < bytesPerLine && i + j < bytes.Length; j++)
                {
                    byte b = bytes[i + j];
                    char c = (b >= 32 && b <= 126) ? (char)b : '.';
                    result.Append(c);
                }
                
                result.AppendLine("|");
            }
            
            return result.ToString();
        }
    }
}
