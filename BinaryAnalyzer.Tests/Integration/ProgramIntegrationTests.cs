using Xunit;
using BinaryAnalyzer.Core;
using BinaryAnalyzer.Utils;
using System;
using System.IO;

namespace BinaryAnalyzer.Tests.Integration
{
    public class ProgramIntegrationTests
    {
        [Fact]
        public void Program_WithValidFile_RunsWithoutException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "Hello World Test");
            
            try
            {
                // Simulate program arguments
                string[] args = { tempFile };

                // Act & Assert - Should not throw
                var originalOut = Console.Out;
                using var stringWriter = new StringWriter();
                Console.SetOut(stringWriter);

                // Simulate the main program logic
                byte[] data = File.ReadAllBytes(tempFile);
                var fileType = FileAnalyzer.DetectFileType(data);
                var entropy = Entropy.Calculate(data);
                var md5 = data.GetMD5Hash();

                Console.SetOut(originalOut);

                // Verify basic analysis worked
                Assert.NotNull(fileType);
                Assert.True(entropy >= 0);
                Assert.NotEmpty(md5);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void Program_WithDetailedAnalysis_ProducesReport()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var testData = System.Text.Encoding.ASCII.GetBytes("This is a test file with some content for analysis.");
            File.WriteAllBytes(tempFile, testData);

            try
            {
                // Act
                var metadata = MetadataAnalyzer.AnalyzeFile(testData);
                var report = MetadataAnalyzer.GenerateReport(metadata);

                // Assert
                Assert.Contains("BINARY ANALYSIS REPORT", report);
                Assert.Contains("HEURISTICS", report);
                Assert.Contains("BYTE FREQUENCY", report);
                Assert.True(metadata.IsLikelyText);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void Program_WithBinaryFile_AnalyzesCorrectly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var binaryData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE, 0xFD };
            File.WriteAllBytes(tempFile, binaryData);

            try
            {
                // Act
                var metadata = MetadataAnalyzer.AnalyzeFile(binaryData);

                // Assert
                Assert.False(metadata.IsLikelyText);
                Assert.False(metadata.IsLikelyExecutable);
                Assert.Equal(7, metadata.Size);
                Assert.NotEmpty(metadata.MD5);
                Assert.NotEmpty(metadata.SHA256);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void Program_WithEmptyFile_HandlesGracefully()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllBytes(tempFile, new byte[0]);

            try
            {
                // Act
                var data = File.ReadAllBytes(tempFile);
                var metadata = MetadataAnalyzer.AnalyzeFile(data);

                // Assert
                Assert.Equal(0, metadata.Size);
                Assert.Equal(0.0, metadata.Entropy);
                Assert.False(metadata.IsLikelyText);
                Assert.False(metadata.IsLikelyExecutable);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
