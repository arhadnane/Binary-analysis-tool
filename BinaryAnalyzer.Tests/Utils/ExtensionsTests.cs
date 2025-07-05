using Xunit;
using BinaryAnalyzer.Utils;
using System.Linq;
using System.Collections.Generic;

namespace BinaryAnalyzer.Tests.Utils
{
    public class ExtensionsTests
    {
        [Fact]
        public void ToHex_ValidBytes_ReturnsCorrectHexString()
        {
            // Arrange
            byte[] bytes = { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello"

            // Act
            string result = bytes.ToHex();

            // Assert
            Assert.Equal("48656C6C6F", result);
        }

        [Fact]
        public void ToHex_EmptyArray_ReturnsEmptyString()
        {
            // Arrange
            byte[] bytes = new byte[0];

            // Act
            string result = bytes.ToHex();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void ToHex_SingleByte_ReturnsCorrectHex()
        {
            // Arrange
            byte[] bytes = { 0xFF };

            // Act
            string result = bytes.ToHex();

            // Assert
            Assert.Equal("FF", result);
        }

        [Fact]
        public void ToAscii_ValidBytes_ReturnsCorrectString()
        {
            // Arrange
            byte[] bytes = { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello"

            // Act
            string result = bytes.ToAscii();

            // Assert
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void ToAscii_EmptyArray_ReturnsEmptyString()
        {
            // Arrange
            byte[] bytes = new byte[0];

            // Act
            string result = bytes.ToAscii();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void ToAscii_NonPrintableBytes_ReturnsString()
        {
            // Arrange
            byte[] bytes = { 0x00, 0x01, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x7F };

            // Act
            string result = bytes.ToAscii();

            // Assert
            Assert.Equal(8, result.Length);
            Assert.Contains("Hello", result);
        }

        [Fact]
        public void ToUtf8_ValidBytes_ReturnsCorrectString()
        {
            // Arrange
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("Hello World");

            // Act
            string result = bytes.ToUtf8();

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void GetByteFrequency_ValidData_ReturnsCorrectFrequency()
        {
            // Arrange
            byte[] bytes = { 0x41, 0x41, 0x42, 0x41, 0x43 }; // "AABAC"

            // Act
            var result = bytes.GetByteFrequency();

            // Assert
            Assert.Equal(3, result[0x41]); // 'A' appears 3 times
            Assert.Equal(1, result[0x42]); // 'B' appears 1 time
            Assert.Equal(1, result[0x43]); // 'C' appears 1 time
        }

        [Fact]
        public void IsLikelyText_TextData_ReturnsTrue()
        {
            // Arrange
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Hello World! This is readable text.");

            // Act
            bool result = bytes.IsLikelyText();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsLikelyText_BinaryData_ReturnsFalse()
        {
            // Arrange
            byte[] bytes = { 0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE, 0xFD };

            // Act
            bool result = bytes.IsLikelyText();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ExtractStrings_ValidData_ReturnsStrings()
        {
            // Arrange
            byte[] bytes = { 
                0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, // "Hello"
                0x57, 0x6F, 0x72, 0x6C, 0x64, 0x00, // "World"
                0x54, 0x65, 0x73, 0x74, // "Test"
                0x00
            };

            // Act
            var result = bytes.ExtractStrings(4).ToList();

            // Assert
            Assert.Contains("Hello", result);
            Assert.Contains("World", result);
            Assert.Contains("Test", result);
        }

        [Fact]
        public void ExtractStrings_ShortStrings_FiltersOut()
        {
            // Arrange
            byte[] bytes = { 
                0x48, 0x69, 0x00, // "Hi" (too short)
                0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00 // "Hello"
            };

            // Act
            var result = bytes.ExtractStrings(4).ToList();

            // Assert
            Assert.DoesNotContain("Hi", result);
            Assert.Contains("Hello", result);
        }

        [Fact]
        public void GetMD5Hash_ValidData_ReturnsCorrectHash()
        {
            // Arrange
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Hello World");

            // Act
            string result = bytes.GetMD5Hash();

            // Assert
            Assert.Equal(32, result.Length); // MD5 is 32 hex characters
            Assert.Equal("b10a8db164e0754105b7a99be72e3fe5", result);
        }

        [Fact]
        public void GetSHA256Hash_ValidData_ReturnsCorrectHash()
        {
            // Arrange
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Hello World");

            // Act
            string result = bytes.GetSHA256Hash();

            // Assert
            Assert.Equal(64, result.Length); // SHA256 is 64 hex characters
            Assert.Equal("a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e", result);
        }

        [Fact]
        public void FindPattern_ExistingPattern_ReturnsPositions()
        {
            // Arrange
            byte[] data = { 0x00, 0x01, 0x02, 0x01, 0x02, 0x03, 0x01, 0x02 };
            byte[] pattern = { 0x01, 0x02 };

            // Act
            var result = data.FindPattern(pattern);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(3, result);
            Assert.Contains(6, result);
        }

        [Fact]
        public void FindPattern_NonExistentPattern_ReturnsEmpty()
        {
            // Arrange
            byte[] data = { 0x00, 0x01, 0x02, 0x03 };
            byte[] pattern = { 0xFF, 0xFE };

            // Act
            var result = data.FindPattern(pattern);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void FindNullRuns_WithNullRuns_ReturnsRuns()
        {
            // Arrange
            byte[] data = { 
                0x01, 0x00, 0x00, 0x00, 0x00, 0x00, // 5 nulls
                0x02, 0x00, 0x00, 0x03 // 2 nulls (too short)
            };

            // Act
            var result = data.FindNullRuns(4).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].start);
            Assert.Equal(5, result[0].length);
        }

        [Fact]
        public void ToHexDump_ValidData_ReturnsFormattedDump()
        {
            // Arrange
            byte[] data = { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x57, 0x6F, 0x72, 0x6C, 0x64 }; // "Hello World"

            // Act
            string result = data.ToHexDump();

            // Assert
            Assert.Contains("00000000", result); // Offset
            Assert.Contains("48 65 6C 6C", result); // Hex bytes
            Assert.Contains("Hello", result); // ASCII representation
            Assert.Contains("|", result); // ASCII delimiters
        }

        [Fact]
        public void ToHexDump_EmptyData_ReturnsEmpty()
        {
            // Arrange
            byte[] data = new byte[0];

            // Act
            string result = data.ToHexDump();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void ToHexDump_CustomBytesPerLine_FormatsCorrectly()
        {
            // Arrange
            byte[] data = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            // Act
            string result = data.ToHexDump(4);

            // Assert
            var lines = result.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(2, lines.Length); // Should have 2 lines with 4 bytes each
        }
    }
}
