using Xunit;
using BinaryAnalyzer.Core;
using System;

namespace BinaryAnalyzer.Tests.Core
{
    public class BinaryParserTests
    {
        [Fact]
        public void FromBinaryString_ValidInput_ReturnsCorrectBytes()
        {
            // Arrange
            string binary = "01001000 01100101 01101100 01101100 01101111"; // "Hello"
            byte[] expected = { 0x48, 0x65, 0x6C, 0x6C, 0x6F };

            // Act
            byte[] result = BinaryParser.FromBinaryString(binary);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FromBinaryString_EmptyString_ThrowsArgumentException()
        {
            // Arrange
            string binary = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => BinaryParser.FromBinaryString(binary));
        }

        [Fact]
        public void FromBinaryString_InvalidLength_ThrowsArgumentException()
        {
            // Arrange
            string binary = "0100100"; // 7 bits, not multiple of 8

            // Act & Assert
            Assert.Throws<ArgumentException>(() => BinaryParser.FromBinaryString(binary));
        }

        [Fact]
        public void ToAscii_ValidBytes_ReturnsCorrectString()
        {
            // Arrange
            byte[] bytes = { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello"

            // Act
            string result = BinaryParser.ToAscii(bytes);

            // Assert
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void ToBase64_ValidBytes_ReturnsCorrectString()
        {
            // Arrange
            byte[] bytes = { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello"

            // Act
            string result = BinaryParser.ToBase64(bytes);

            // Assert
            Assert.Equal("SGVsbG8=", result);
        }

        [Fact]
        public void ToHex_ValidBytes_ReturnsCorrectHexString()
        {
            // Arrange
            byte[] bytes = { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello"

            // Act
            string result = BinaryParser.ToHex(bytes);

            // Assert
            Assert.Equal("48656C6C6F", result);
        }
    }
}
