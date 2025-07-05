// Analyseur de structure PE (Portable Executable)
using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryAnalyzer.Core
{
    public class PEInfo
    {
        public bool IsPE { get; set; }
        public string Architecture { get; set; } = "Unknown";
        public DateTime? CompileTime { get; set; }
        public List<string> Sections { get; set; } = new();
        public List<string> Imports { get; set; } = new();
        public string Subsystem { get; set; } = "Unknown";
        public bool IsDLL { get; set; }
    }

    public static class PEAnalyzer
    {
        public static PEInfo AnalyzePE(byte[] data)
        {
            var info = new PEInfo();
            
            if (data.Length < 64) return info;

            // Check DOS signature "MZ"
            if (data[0] != 0x4D || data[1] != 0x5A) return info;

            // Get PE header offset
            uint peOffset = BitConverter.ToUInt32(data, 60);
            if (peOffset >= data.Length - 4) return info;

            // Check PE signature "PE\0\0"
            if (data[peOffset] != 0x50 || data[peOffset + 1] != 0x45 ||
                data[peOffset + 2] != 0x00 || data[peOffset + 3] != 0x00)
                return info;

            info.IsPE = true;

            try
            {
                // Machine type (architecture)
                ushort machine = BitConverter.ToUInt16(data, (int)peOffset + 4);
                info.Architecture = machine switch
                {
                    0x014c => "x86",
                    0x8664 => "x64",
                    0x01c0 => "ARM",
                    0xaa64 => "ARM64",
                    _ => $"Unknown (0x{machine:X4})"
                };

                // Timestamp
                uint timestamp = BitConverter.ToUInt32(data, (int)peOffset + 8);
                if (timestamp > 0)
                {
                    info.CompileTime = new DateTime(1970, 1, 1).AddSeconds(timestamp);
                }

                // Characteristics
                ushort characteristics = BitConverter.ToUInt16(data, (int)peOffset + 22);
                info.IsDLL = (characteristics & 0x2000) != 0;

                // Optional header
                ushort optionalHeaderSize = BitConverter.ToUInt16(data, (int)peOffset + 20);
                if (optionalHeaderSize > 0)
                {
                    int optionalHeaderOffset = (int)peOffset + 24;
                    if (optionalHeaderOffset + 68 < data.Length)
                    {
                        ushort subsystem = BitConverter.ToUInt16(data, optionalHeaderOffset + 68);
                        info.Subsystem = subsystem switch
                        {
                            1 => "Native",
                            2 => "Windows GUI",
                            3 => "Windows Console",
                            9 => "Windows CE",
                            _ => $"Unknown ({subsystem})"
                        };
                    }
                }

                // Sections
                ushort numberOfSections = BitConverter.ToUInt16(data, (int)peOffset + 6);
                int sectionHeaderOffset = (int)peOffset + 24 + optionalHeaderSize;

                for (int i = 0; i < numberOfSections && sectionHeaderOffset + 40 < data.Length; i++)
                {
                    var sectionName = Encoding.ASCII.GetString(data, sectionHeaderOffset, 8).TrimEnd('\0');
                    info.Sections.Add(sectionName);
                    sectionHeaderOffset += 40;
                }
            }
            catch
            {
                // In case of error, return partial info
            }

            return info;
        }
    }
}
