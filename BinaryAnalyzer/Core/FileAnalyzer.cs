// Détection des signatures de fichiers (magic numbers)
using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryAnalyzer.Core
{
    public static class FileAnalyzer
    {
        private static readonly Dictionary<string, byte[]> MagicNumbers = new()
        {
            { "PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47 } },
            { "JPEG", new byte[] { 0xFF, 0xD8, 0xFF } },
            { "GIF", new byte[] { 0x47, 0x49, 0x46, 0x38 } },
            { "PDF", new byte[] { 0x25, 0x50, 0x44, 0x46 } },
            { "ZIP", new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
            // Ajoutez d'autres signatures si besoin
        };

        public static string DetectFileType(byte[] data)
        {
            foreach (var kvp in MagicNumbers)
            {
                var magic = kvp.Value;
                if (data.Length >= magic.Length)
                {
                    bool match = true;
                    for (int i = 0; i < magic.Length; i++)
                    {
                        if (data[i] != magic[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                        return kvp.Key;
                }
            }
            return "Unknown";
        }

        public static string DetectFileType(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] header = new byte[8];
            fs.Read(header, 0, header.Length);
            return DetectFileType(header);
        }
    }
}
