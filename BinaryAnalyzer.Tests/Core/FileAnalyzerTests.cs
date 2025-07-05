using Xunit;
using BinaryAnalyzer.Core;
using System;

namespace BinaryAnalyzer.Tests.Core
{
    public class FileAnalyzerTests
    {
        [Fact]
        public void DetectFileType_PngSignature_ReturnsPNG()
        {
            // Arrange
            byte[] pngHeader = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

            // Act
            string result = FileAnalyzer.DetectFileType(pngHeader);

            // Assert
            Assert.Equal("PNG", result);
        }

        [Fact]
        public void DetectFileType_JpegSignature_ReturnsJPEG()
        {
            // Arrange
            byte[] jpegHeader = { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10 };

            // Act
            string result = FileAnalyzer.DetectFileType(jpegHeader);

            // Assert
            Assert.Equal("JPEG", result);
        }

        [Fact]
        public void DetectFileType_PdfSignature_ReturnsPDF()
        {
            // Arrange
            byte[] pdfHeader = { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 };

            // Act
            string result = FileAnalyzer.DetectFileType(pdfHeader);

            // Assert
            Assert.Equal("PDF", result);
        }

        [Fact]
        public void DetectFileType_ZipSignature_ReturnsZIP()
        {
            // Arrange
            byte[] zipHeader = { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00 };

            // Act
            string result = FileAnalyzer.DetectFileType(zipHeader);

            // Assert
            Assert.Equal("ZIP", result);
        }

        [Fact]
        public void DetectFileType_GifSignature_ReturnsGIF()
        {
            // Arrange
            byte[] gifHeader = { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 };

            // Act
            string result = FileAnalyzer.DetectFileType(gifHeader);

            // Assert
            Assert.Equal("GIF", result);
        }

        [Fact]
        public void DetectFileType_UnknownSignature_ReturnsUnknown()
        {
            // Arrange
            byte[] unknownHeader = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05 };

            // Act
            string result = FileAnalyzer.DetectFileType(unknownHeader);

            // Assert
            Assert.Equal("Unknown", result);
        }

        [Fact]
        public void DetectFileType_EmptyArray_ReturnsUnknown()
        {
            // Arrange
            byte[] emptyArray = new byte[0];

            // Act
            string result = FileAnalyzer.DetectFileType(emptyArray);

            // Assert
            Assert.Equal("Unknown", result);
        }

        [Fact]
        public void DetectFileType_PartialSignature_ReturnsUnknown()
        {
            // Arrange
            byte[] partialPngHeader = { 0x89, 0x50 }; // Only first 2 bytes of PNG

            // Act
            string result = FileAnalyzer.DetectFileType(partialPngHeader);

            // Assert
            Assert.Equal("Unknown", result);
        }
    }
}
