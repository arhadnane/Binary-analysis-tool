using Xunit;
using BinaryAnalyzer.Core;
using System.Linq;

namespace BinaryAnalyzer.Tests.Core
{
    public class DisassemblerTests
    {
        [Fact]
        public void Disassemble_ValidCode_ReturnsDisassembly()
        {
            // Arrange
            byte[] code = { 0x48, 0x89, 0xE5, 0xC3 }; // Some x64 assembly bytes

            // Act
            var result = Disassembler.Disassemble(code, true).ToList();

            // Assert
            Assert.Single(result);
            Assert.Contains("0x0000:", result[0]);
            Assert.Contains("Raw bytes", result[0]);
        }

        [Fact]
        public void Disassemble_EmptyCode_ReturnsEmpty()
        {
            // Arrange
            byte[] code = new byte[0];

            // Act
            var result = Disassembler.Disassemble(code, true).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Disassemble_32BitMode_WorksCorrectly()
        {
            // Arrange
            byte[] code = { 0x89, 0xE5, 0xC3 }; // Some x86 assembly bytes

            // Act
            var result = Disassembler.Disassemble(code, false).ToList();

            // Assert
            Assert.Single(result);
            Assert.Contains("0x0000:", result[0]);
        }

        [Fact]
        public void Disassemble_LargeCode_ShowsFirst16Bytes()
        {
            // Arrange
            byte[] code = new byte[32]; // 32 bytes of zeros
            for (int i = 0; i < 32; i++)
            {
                code[i] = (byte)i;
            }

            // Act
            var result = Disassembler.Disassemble(code, true).ToList();

            // Assert
            Assert.Single(result);
            // Should show only first 16 bytes (32 hex characters + spaces)
            var hexPart = result[0].Split('(')[0].Trim();
            var bytesShown = hexPart.Split(' ').Length - 1; // -1 for "0x0000:"
            Assert.True(bytesShown <= 16);
        }
    }
}
