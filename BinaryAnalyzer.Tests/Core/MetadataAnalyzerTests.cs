using Xunit;
using BinaryAnalyzer.Core;
using BinaryAnalyzer.Utils;
using System.Linq;

namespace BinaryAnalyzer.Tests.Core
{
    public class MetadataAnalyzerTests
    {
        [Fact]
        public void AnalyzeFile_BasicTextFile_ReturnsCorrectMetadata()
        {
            // Arrange
            var data = System.Text.Encoding.ASCII.GetBytes("Hello World! This is a test file.");

            // Act
            var result = MetadataAnalyzer.AnalyzeFile(data);

            // Assert
            Assert.Equal(data.Length, result.Size);
            Assert.True(result.IsLikelyText);
            Assert.False(result.IsLikelyExecutable);
            Assert.False(result.IsLikelyCompressed);
            Assert.Equal("Unknown", result.FileType);
            Assert.NotEmpty(result.MD5);
            Assert.NotEmpty(result.SHA256);
            Assert.NotEmpty(result.ExtractedStrings);
        }

        [Fact]
        public void AnalyzeFile_HighEntropyData_DetectsAsCompressed()
        {
            // Arrange - Create high entropy data
            var random = new System.Random(42);
            var data = new byte[1000];
            random.NextBytes(data);

            // Act
            var result = MetadataAnalyzer.AnalyzeFile(data);

            // Assert
            Assert.True(result.IsLikelyCompressed);
            Assert.False(result.IsLikelyText);
            Assert.True(result.Entropy > 7.5);
        }

        [Fact]
        public void AnalyzeFile_PEExecutable_DetectsAsExecutable()
        {
            // Arrange - Create minimal PE header with size > 1024
            var data = new byte[1025]; // Must be > 1024 for the heuristic
            data[0] = 0x4D; // 'M'
            data[1] = 0x5A; // 'Z'

            // Act
            var result = MetadataAnalyzer.AnalyzeFile(data);

            // Assert - Should detect as likely executable due to MZ signature
            Assert.True(result.IsLikelyExecutable);
        }

        [Fact]
        public void AnalyzeFile_WithStrings_ExtractsStrings()
        {
            // Arrange
            var data = new byte[] { 
                0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, // "Hello"
                0x57, 0x6F, 0x72, 0x6C, 0x64, 0x00, // "World"
                0x54, 0x65, 0x73, 0x74, 0x69, 0x6E, 0x67, // "Testing"
                0x00, 0x00, 0x00
            };

            // Act
            var result = MetadataAnalyzer.AnalyzeFile(data);

            // Assert
            Assert.Contains("Hello", result.ExtractedStrings);
            Assert.Contains("World", result.ExtractedStrings);
            Assert.Contains("Testing", result.ExtractedStrings);
        }

        [Fact]
        public void AnalyzeFile_EmptyData_HandlesGracefully()
        {
            // Arrange
            var data = new byte[0];

            // Act
            var result = MetadataAnalyzer.AnalyzeFile(data);

            // Assert
            Assert.Equal(0, result.Size);
            Assert.Equal(0.0, result.Entropy);
            Assert.False(result.IsLikelyText);
            Assert.False(result.IsLikelyExecutable);
            Assert.False(result.IsLikelyCompressed);
        }

        [Fact]
        public void GenerateReport_ValidMetadata_ReturnsFormattedReport()
        {
            // Arrange
            var metadata = new MetadataAnalyzer.FileMetadata
            {
                Size = 1024,
                Entropy = 4.5,
                MD5 = "test-md5-hash",
                SHA256 = "test-sha256-hash",
                FileType = "PNG",
                IsLikelyText = false,
                IsLikelyExecutable = false,
                IsLikelyCompressed = false
            };
            metadata.ExtractedStrings.Add("Test String");
            metadata.ByteFrequency.Add(0x41, 10); // 'A'
            metadata.ByteFrequency.Add(0x42, 5);  // 'B'

            // Act
            var report = MetadataAnalyzer.GenerateReport(metadata);

            // Assert
            Assert.Contains("BINARY ANALYSIS REPORT", report);
            Assert.Contains("Size:", report); // More flexible test
            Assert.Contains("PNG", report);
            Assert.Contains("4,5", report);
            Assert.Contains("test-md5-hash", report);
            Assert.Contains("Test String", report);
        }

        [Fact]
        public void GenerateReport_WithPEInfo_IncludesPESection()
        {
            // Arrange
            var metadata = new MetadataAnalyzer.FileMetadata
            {
                Size = 2048,
                Entropy = 6.0,
                MD5 = "pe-md5",
                SHA256 = "pe-sha256",
                FileType = "Unknown",
                IsLikelyExecutable = true,
                PEInfo = new PEInfo
                {
                    IsPE = true,
                    Architecture = "x64",
                    IsDLL = false,
                    Subsystem = "Windows Console",
                    CompileTime = new System.DateTime(2023, 1, 1)
                }
            };
            metadata.PEInfo.Sections.Add(".text");
            metadata.PEInfo.Sections.Add(".data");

            // Act
            var report = MetadataAnalyzer.GenerateReport(metadata);

            // Assert
            Assert.Contains("BINARY ANALYSIS REPORT", report);
            Assert.Contains("x64", report);
            Assert.Contains("EXE", report);
            Assert.Contains("Windows Console", report);
            Assert.Contains("2023-01-01", report);
            Assert.Contains(".text, .data", report);
        }

        [Fact]
        public void AnalyzeFile_WithInterestingOffsets_FindsPatterns()
        {
            // Arrange - Data with URL pattern
            var httpPattern = System.Text.Encoding.ASCII.GetBytes("http://example.com");
            var data = new byte[100];
            httpPattern.CopyTo(data, 10);

            // Act
            var result = MetadataAnalyzer.AnalyzeFile(data);

            // Assert
            Assert.NotEmpty(result.InterestingOffsets);
            Assert.Contains(result.InterestingOffsets, x => x.description.Contains("URL pattern"));
        }

        [Fact]
        public void AnalyzeFile_ByteFrequency_CalculatesCorrectly()
        {
            // Arrange
            var data = new byte[] { 0x41, 0x41, 0x42, 0x41, 0x43 }; // "AABAC"

            // Act
            var result = MetadataAnalyzer.AnalyzeFile(data);

            // Assert
            Assert.Equal(3, result.ByteFrequency[0x41]); // 'A' appears 3 times
            Assert.Equal(1, result.ByteFrequency[0x42]); // 'B' appears 1 time
            Assert.Equal(1, result.ByteFrequency[0x43]); // 'C' appears 1 time
        }
    }
}
