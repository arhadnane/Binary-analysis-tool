using Xunit;
using BinaryAnalyzer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryAnalyzer.Tests.Core
{
    public class PEAnalyzerTests
    {
        [Fact]
        public void AnalyzePE_ValidPEHeader_ReturnsPEInfo()
        {
            // Arrange - Create a minimal valid PE header
            var data = new byte[1024];
            
            // DOS header
            data[0] = 0x4D; // 'M'
            data[1] = 0x5A; // 'Z'
            
            // PE offset at position 60
            System.BitConverter.GetBytes((uint)128).CopyTo(data, 60);
            
            // PE signature at offset 128
            data[128] = 0x50; // 'P'
            data[129] = 0x45; // 'E'
            data[130] = 0x00;
            data[131] = 0x00;
            
            // Machine type (x64)
            System.BitConverter.GetBytes((ushort)0x8664).CopyTo(data, 132);
            
            // Number of sections
            System.BitConverter.GetBytes((ushort)2).CopyTo(data, 134);
            
            // Timestamp
            System.BitConverter.GetBytes((uint)1609459200).CopyTo(data, 136); // 2021-01-01
            
            // Characteristics (not DLL)
            System.BitConverter.GetBytes((ushort)0x0102).CopyTo(data, 150);
            
            // Optional header size
            System.BitConverter.GetBytes((ushort)240).CopyTo(data, 148);
            
            // Subsystem (Windows GUI)
            System.BitConverter.GetBytes((ushort)2).CopyTo(data, 220);

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.True(result.IsPE);
            Assert.Equal("x64", result.Architecture);
            Assert.False(result.IsDLL);
            Assert.Equal("Windows GUI", result.Subsystem);
            Assert.NotNull(result.CompileTime);
        }

        [Fact]
        public void AnalyzePE_InvalidDOSSignature_ReturnsNotPE()
        {
            // Arrange
            var data = new byte[1024];
            data[0] = 0x00; // Invalid DOS signature
            data[1] = 0x00;

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.False(result.IsPE);
        }

        [Fact]
        public void AnalyzePE_InvalidPESignature_ReturnsNotPE()
        {
            // Arrange
            var data = new byte[1024];
            
            // Valid DOS header
            data[0] = 0x4D; // 'M'
            data[1] = 0x5A; // 'Z'
            BitConverter.GetBytes((uint)128).CopyTo(data, 60);
            
            // Invalid PE signature
            data[128] = 0x00;
            data[129] = 0x00;
            data[130] = 0x00;
            data[131] = 0x00;

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.False(result.IsPE);
        }

        [Fact]
        public void AnalyzePE_TooSmallData_ReturnsNotPE()
        {
            // Arrange
            var data = new byte[32]; // Too small

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.False(result.IsPE);
        }

        [Fact]
        public void AnalyzePE_X86Architecture_ReturnsCorrectArchitecture()
        {
            // Arrange
            var data = CreateValidPEHeader();
            // Set machine type to x86
            System.BitConverter.GetBytes((ushort)0x014c).CopyTo(data, 132);

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.True(result.IsPE);
            Assert.Equal("x86", result.Architecture);
        }

        [Fact]
        public void AnalyzePE_ARMArchitecture_ReturnsCorrectArchitecture()
        {
            // Arrange
            var data = CreateValidPEHeader();
            // Set machine type to ARM
            System.BitConverter.GetBytes((ushort)0x01c0).CopyTo(data, 132);

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.True(result.IsPE);
            Assert.Equal("ARM", result.Architecture);
        }

        [Fact]
        public void AnalyzePE_DLLCharacteristics_ReturnsDLL()
        {
            // Arrange
            var data = CreateValidPEHeader();
            // Set DLL characteristic bit
            System.BitConverter.GetBytes((ushort)0x2000).CopyTo(data, 150);

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.True(result.IsPE);
            Assert.True(result.IsDLL);
        }

        [Fact]
        public void AnalyzePE_ConsoleSubsystem_ReturnsCorrectSubsystem()
        {
            // Arrange
            var data = CreateValidPEHeader();
            // Set subsystem to Windows Console
            System.BitConverter.GetBytes((ushort)3).CopyTo(data, 220);

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.True(result.IsPE);
            Assert.Equal("Windows Console", result.Subsystem);
        }

        [Fact]
        public void AnalyzePE_WithSections_ReturnsCorrectSections()
        {
            // Arrange
            var data = CreateValidPEHeaderWithSections();

            // Act
            var result = PEAnalyzer.AnalyzePE(data);

            // Assert
            Assert.True(result.IsPE);
            Assert.Contains(".text", result.Sections);
            Assert.Contains(".data", result.Sections);
        }

        private byte[] CreateValidPEHeader()
        {
            var data = new byte[1024];
            
            // DOS header
            data[0] = 0x4D; // 'M'
            data[1] = 0x5A; // 'Z'
            System.BitConverter.GetBytes((uint)128).CopyTo(data, 60);
            
            // PE signature
            data[128] = 0x50; // 'P'
            data[129] = 0x45; // 'E'
            data[130] = 0x00;
            data[131] = 0x00;
            
            // Machine type (x64)
            System.BitConverter.GetBytes((ushort)0x8664).CopyTo(data, 132);
            
            // Number of sections
            System.BitConverter.GetBytes((ushort)0).CopyTo(data, 134);
            
            // Timestamp
            System.BitConverter.GetBytes((uint)1609459200).CopyTo(data, 136);
            
            // Characteristics
            System.BitConverter.GetBytes((ushort)0x0102).CopyTo(data, 150);
            
            // Optional header size
            System.BitConverter.GetBytes((ushort)240).CopyTo(data, 148);
            
            // Subsystem
            System.BitConverter.GetBytes((ushort)2).CopyTo(data, 220);
            
            return data;
        }

        private byte[] CreateValidPEHeaderWithSections()
        {
            var data = CreateValidPEHeader();
            
            // Set number of sections to 2
            System.BitConverter.GetBytes((ushort)2).CopyTo(data, 134);
            
            // Section headers start after PE header + optional header
            int sectionOffset = 128 + 24 + 240; // PE offset + COFF header + optional header
            
            // First section: .text
            var textName = System.Text.Encoding.ASCII.GetBytes(".text\0\0\0");
            textName.CopyTo(data, sectionOffset);
            
            // Second section: .data
            var dataName = System.Text.Encoding.ASCII.GetBytes(".data\0\0\0");
            dataName.CopyTo(data, sectionOffset + 40);
            
            return data;
        }
    }
}
