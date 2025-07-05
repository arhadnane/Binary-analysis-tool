// Point d'entrée CLI
using System;
using System.IO;
using System.Linq;
using BinaryAnalyzer.Core;
using BinaryAnalyzer.Utils;

namespace BinaryAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("🔬 Polyvalent Binary Analyzer");
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine("  BinaryAnalyzer <file>           - Quick analysis");
                Console.WriteLine("  BinaryAnalyzer <file> --detailed - Detailed report");
                Console.WriteLine("  BinaryAnalyzer <file> --hexdump  - Hexadecimal display");
                Console.WriteLine();
                Console.WriteLine("Examples:");
                Console.WriteLine("  BinaryAnalyzer document.pdf");
                Console.WriteLine("  BinaryAnalyzer malware.exe --detailed");
                Console.WriteLine("  BinaryAnalyzer data.bin --hexdump");
                return;
            }
            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }
            byte[] data = File.ReadAllBytes(filePath);
            
            // Check additional arguments
            bool detailedReport = args.Length > 1 && args[1] == "--detailed";
            bool hexDump = args.Length > 1 && args[1] == "--hexdump";
            
            if (detailedReport)
            {
                // Complete detailed report
                var metadata = MetadataAnalyzer.AnalyzeFile(data);
                Console.WriteLine(MetadataAnalyzer.GenerateReport(metadata));
            }
            else if (hexDump)
            {
                // Hex dump display
                Console.WriteLine($"=== HEX DUMP of {Path.GetFileName(filePath)} ===");
                Console.WriteLine(data.ToHexDump());
            }
            else
            {
                // Quick analysis (default behavior)
                Console.WriteLine($"📄 File: {Path.GetFileName(filePath)} ({data.Length:N0} bytes)");
                Console.WriteLine($"🔍 Type: {FileAnalyzer.DetectFileType(data)}");
                Console.WriteLine($"📊 Entropy: {Entropy.Calculate(data):F4}");
                Console.WriteLine($"🔐 MD5: {data.GetMD5Hash()}");
                
                if (data.IsLikelyText())
                {
                    Console.WriteLine("📝 Content: Likely text");
                }
                else
                {
                    Console.WriteLine($"🔢 Hex (first 32 bytes): {data.ToHex().Substring(0, Math.Min(64, data.Length * 2))}...");
                }
                
                var strings = data.ExtractStrings().Take(5).ToList();
                if (strings.Any())
                {
                    Console.WriteLine($"📋 Strings found: {string.Join(", ", strings.Select(s => $"\"{s}\""))}");
                }
                
                Console.WriteLine("\n💻 Disassembly (first line):");
                try
                {
                    foreach (var line in Disassembler.Disassemble(data, true))
                    {
                        Console.WriteLine($"   {line}");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ❌ Error: {ex.Message}");
                }
                
                Console.WriteLine("\n💡 Use --detailed for complete report or --hexdump for hexadecimal display");
            }
        }
    }
}
