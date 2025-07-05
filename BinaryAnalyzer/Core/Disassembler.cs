// Disassembler simplifié (version sans Capstone.NET)
using System;
using System.Collections.Generic;

namespace BinaryAnalyzer.Core
{
    public static class Disassembler
    {
        public static IEnumerable<string> Disassemble(byte[] code, bool is64Bit = true)
        {
            // Version simplifiée - affiche les premiers bytes en hex
            if (code.Length > 0)
            {
                var hexBytes = BitConverter.ToString(code, 0, Math.Min(16, code.Length)).Replace("-", " ");
                yield return $"0x0000: {hexBytes} (Raw bytes - Capstone.NET disabled)";
            }
        }
    }
}
