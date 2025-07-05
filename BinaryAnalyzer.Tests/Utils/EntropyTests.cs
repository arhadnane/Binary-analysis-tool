using Xunit;
using BinaryAnalyzer.Utils;
using System;

namespace BinaryAnalyzer.Tests.Utils
{
    public class EntropyTests
    {
        [Fact]
        public void Calculate_UniformDistribution_ReturnsMaxEntropy()
        {
            // Arrange - All 256 possible byte values once each
            byte[] data = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                data[i] = (byte)i;
            }

            // Act
            double entropy = Entropy.Calculate(data);

            // Assert - Maximum entropy for 8-bit data is 8.0
            Assert.True(entropy > 7.9 && entropy <= 8.0);
        }

        [Fact]
        public void Calculate_SingleByte_ReturnsZeroEntropy()
        {
            // Arrange - All bytes are the same
            byte[] data = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                data[i] = 0x42;
            }

            // Act
            double entropy = Entropy.Calculate(data);

            // Assert
            Assert.Equal(0.0, entropy, 10); // 10 decimal places precision
        }

        [Fact]
        public void Calculate_TwoDifferentBytes_ReturnsOneEntropy()
        {
            // Arrange - Equal distribution of two different bytes
            byte[] data = new byte[200];
            for (int i = 0; i < 100; i++)
            {
                data[i] = 0x00;
                data[i + 100] = 0xFF;
            }

            // Act
            double entropy = Entropy.Calculate(data);

            // Assert - Should be exactly 1.0 for equal distribution of 2 values
            Assert.Equal(1.0, entropy, 10);
        }

        [Fact]
        public void Calculate_EmptyArray_ReturnsZero()
        {
            // Arrange
            byte[] data = new byte[0];

            // Act
            double entropy = Entropy.Calculate(data);

            // Assert
            Assert.Equal(0.0, entropy);
        }

        [Fact]
        public void Calculate_NullArray_ReturnsZero()
        {
            // Arrange
            byte[] data = null;

            // Act
            double entropy = Entropy.Calculate(data);

            // Assert
            Assert.Equal(0.0, entropy);
        }

        [Fact]
        public void Calculate_SingleByteArray_ReturnsZero()
        {
            // Arrange
            byte[] data = { 0x42 };

            // Act
            double entropy = Entropy.Calculate(data);

            // Assert
            Assert.Equal(0.0, entropy);
        }

        [Fact]
        public void Calculate_TextData_ReturnsReasonableEntropy()
        {
            // Arrange - "Hello World" in ASCII
            byte[] data = { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x57, 0x6F, 0x72, 0x6C, 0x64 };

            // Act
            double entropy = Entropy.Calculate(data);

            // Assert - Text should have moderate entropy (between 2 and 4 typically)
            Assert.True(entropy > 2.0 && entropy < 4.0);
        }
    }
}
