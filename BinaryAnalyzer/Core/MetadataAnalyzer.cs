// Analyseur de métadonnées avancées
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalyzer.Utils;

namespace BinaryAnalyzer.Core
{
    public static class MetadataAnalyzer
    {
        public class FileMetadata
        {
            public long Size { get; set; }
            public double Entropy { get; set; }
            public string MD5 { get; set; } = "";
            public string SHA256 { get; set; } = "";
            public string FileType { get; set; } = "";
            public bool IsLikelyText { get; set; }
            public bool IsLikelyExecutable { get; set; }
            public bool IsLikelyCompressed { get; set; }
            public List<string> ExtractedStrings { get; set; } = new();
            public Dictionary<byte, int> ByteFrequency { get; set; } = new();
            public List<(int offset, string description)> InterestingOffsets { get; set; } = new();
            public PEInfo? PEInfo { get; set; }
        }

        public static FileMetadata AnalyzeFile(byte[] data)
        {
            var metadata = new FileMetadata
            {
                Size = data.Length,
                Entropy = Entropy.Calculate(data),
                MD5 = data.GetMD5Hash(),
                SHA256 = data.GetSHA256Hash(),
                FileType = FileAnalyzer.DetectFileType(data),
                IsLikelyText = data.IsLikelyText(),
                ByteFrequency = data.GetByteFrequency()
            };

            // Heuristic analysis
            metadata.IsLikelyCompressed = metadata.Entropy > 7.5;
            metadata.IsLikelyExecutable = metadata.FileType.Contains("PE") || 
                                        data.Length > 1024 && (data[0] == 0x4D && data[1] == 0x5A);

            // String extraction
            metadata.ExtractedStrings = data.ExtractStrings(4).Take(50).ToList();

            // PE analysis if applicable
            if (metadata.IsLikelyExecutable)
            {
                metadata.PEInfo = PEAnalyzer.AnalyzePE(data);
            }

            // Search for interesting offsets
            metadata.InterestingOffsets = FindInterestingOffsets(data);

            return metadata;
        }

        private static List<(int offset, string description)> FindInterestingOffsets(byte[] data)
        {
            var offsets = new List<(int, string)>();

            // URLs
            var urlPatterns = new[] { "http://", "https://", "ftp://" };
            foreach (var pattern in urlPatterns)
            {
                var patternBytes = Encoding.ASCII.GetBytes(pattern);
                var positions = data.FindPattern(patternBytes);
                foreach (var pos in positions)
                {
                    offsets.Add((pos, $"URL pattern: {pattern}"));
                }
            }

            // Email addresses
            var emailPattern = Encoding.ASCII.GetBytes("@");
            var emailPositions = data.FindPattern(emailPattern);
            foreach (var pos in emailPositions.Take(10)) // Limit to 10
            {
                // Check if it's really an email address
                if (IsLikelyEmailAt(data, pos))
                {
                    offsets.Add((pos, "Possible email address"));
                }
            }

            // Compression signatures
            var compressionSignatures = new Dictionary<byte[], string>
            {
                { new byte[] { 0x1F, 0x8B }, "GZIP header" },
                { new byte[] { 0x42, 0x5A, 0x68 }, "BZIP2 header" },
                { new byte[] { 0xFD, 0x37, 0x7A, 0x58, 0x5A }, "XZ header" }
            };

            foreach (var sig in compressionSignatures)
            {
                var positions = data.FindPattern(sig.Key);
                foreach (var pos in positions)
                {
                    offsets.Add((pos, sig.Value));
                }
            }

            // Suspicious padding/alignment
            var nullRuns = data.FindNullRuns(16).Take(10);
            foreach (var run in nullRuns)
            {
                offsets.Add((run.start, $"Null padding ({run.length} bytes)"));
            }

            return offsets.OrderBy(x => x.Item1).ToList();
        }

        private static bool IsLikelyEmailAt(byte[] data, int atPosition)
        {
            // Simple heuristic to detect email address
            if (atPosition < 3 || atPosition >= data.Length - 3) return false;

            // Check for alphabetic characters before and after @
            bool hasAlphaBefore = false;
            bool hasAlphaAfter = false;

            for (int i = Math.Max(0, atPosition - 10); i < atPosition; i++)
            {
                if (char.IsLetterOrDigit((char)data[i]))
                {
                    hasAlphaBefore = true;
                    break;
                }
            }

            for (int i = atPosition + 1; i < Math.Min(data.Length, atPosition + 10); i++)
            {
                if (char.IsLetterOrDigit((char)data[i]))
                {
                    hasAlphaAfter = true;
                    break;
                }
            }

            return hasAlphaBefore && hasAlphaAfter;
        }

        public static string GenerateReport(FileMetadata metadata)
        {
            var report = new StringBuilder();
            
            report.AppendLine("=== BINARY ANALYSIS REPORT ===");
            report.AppendLine($"Size: {metadata.Size:N0} bytes");
            report.AppendLine($"Type: {metadata.FileType}");
            report.AppendLine($"Entropy: {metadata.Entropy:F4}");
            report.AppendLine($"MD5: {metadata.MD5}");
            report.AppendLine($"SHA256: {metadata.SHA256}");
            report.AppendLine();

            report.AppendLine("=== HEURISTICS ===");
            report.AppendLine($"Likely text: {(metadata.IsLikelyText ? "YES" : "NO")}");
            report.AppendLine($"Likely executable: {(metadata.IsLikelyExecutable ? "YES" : "NO")}");
            report.AppendLine($"Likely compressed: {(metadata.IsLikelyCompressed ? "YES" : "NO")}");
            report.AppendLine();

            if (metadata.PEInfo?.IsPE == true)
            {
                var pe = metadata.PEInfo;
                report.AppendLine("=== PE ANALYSIS ===");
                report.AppendLine($"Architecture: {pe.Architecture}");
                report.AppendLine($"Type: {(pe.IsDLL ? "DLL" : "EXE")}");
                report.AppendLine($"Subsystem: {pe.Subsystem}");
                if (pe.CompileTime.HasValue)
                    report.AppendLine($"Compiled on: {pe.CompileTime:yyyy-MM-dd HH:mm:ss}");
                report.AppendLine($"Sections: {string.Join(", ", pe.Sections)}");
                report.AppendLine();
            }

            if (metadata.ExtractedStrings.Any())
            {
                report.AppendLine("=== EXTRACTED STRINGS (first 10) ===");
                foreach (var str in metadata.ExtractedStrings.Take(10))
                {
                    report.AppendLine($"  \"{str}\"");
                }
                report.AppendLine();
            }

            if (metadata.InterestingOffsets.Any())
            {
                report.AppendLine("=== INTERESTING OFFSETS ===");
                foreach (var (offset, desc) in metadata.InterestingOffsets.Take(20))
                {
                    report.AppendLine($"  0x{offset:X8}: {desc}");
                }
                report.AppendLine();
            }

            // Top 10 most frequent bytes
            var topBytes = metadata.ByteFrequency
                .OrderByDescending(x => x.Value)
                .Take(10);
            
            report.AppendLine("=== BYTE FREQUENCY (Top 10) ===");
            foreach (var (b, count) in topBytes)
            {
                var percentage = (double)count / metadata.Size * 100;
                var ascii = (b >= 32 && b <= 126) ? $"'{(char)b}'" : "non-printable";
                report.AppendLine($"  0x{b:X2} ({ascii}): {count:N0} ({percentage:F1}%)");
            }

            return report.ToString();
        }
    }
}
