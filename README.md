# 🔍 Versatile Binary Analyzer

A comprehensive binary analysis tool built in C# (.NET 7) capable of analyzing, converting, and disassembling binary data with a command-line interface.

## ✨ Features

### 🚀 Core Capabilities
- **Binary-to-text/ASCII conversion** - Complete binary parsing and text extraction
- **File signature detection** - Automatic file type recognition via magic numbers
- **x86/x64 disassembly** - Integrated disassembly capabilities using Capstone.NET
- **Entropy calculation** - Shannon entropy analysis for data classification
- **Format decoding** - Support for Base64, Hex, and other common encodings
- **CLI interface** - Full command-line interface with multiple analysis modes

### 📊 Analysis Features
- **Hash calculation** (MD5, SHA256)
- **String extraction** with encoding detection
- **Byte frequency analysis** 
- **Heuristic classification** (text, executable, compressed)
- **PE file analysis** for Windows executables
- **Pattern searching** and data analysis
- **Metadata extraction** from various file types

### 📁 Supported File Types
- **Images**: PNG, JPEG, GIF, BMP, TIFF
- **Documents**: PDF, RTF
- **Archives**: ZIP, RAR, 7Z, TAR, GZIP
- **Executables**: PE (Windows), ELF (Linux), Mach-O (macOS)
- **Audio/Video**: MP3, MP4, AVI, WAV
- **Raw data**: Entropy analysis and hex dump

## 🛠️ Installation

### Prerequisites
- .NET 7.0 SDK or higher
- Visual Studio Code with C# extension (recommended)
- Windows/Linux/macOS

### Setup
```bash
git clone https://github.com/arhadnane/Binary-analysis-tool.git
cd "Binary analysis tool"
cd BinaryAnalyzer
dotnet restore
dotnet build
```

## 💻 Usage

### Basic Commands
```bash
# Quick analysis (default)
dotnet run -- <file_path>

# Quick analysis mode
dotnet run -- <file_path> --quick

# Detailed analysis report
dotnet run -- <file_path> --detailed

# Hexadecimal dump view
dotnet run -- <file_path> --hexdump
```

### Usage Examples
```bash
# Analyze a Windows executable
dotnet run -- C:\Windows\System32\notepad.exe --detailed

# Quick analysis of an image
dotnet run -- image.png --quick

# Hex dump of a binary file
dotnet run -- data.bin --hexdump

# Analyze a PDF document
dotnet run -- document.pdf --detailed
```

### Sample Output

#### Quick Analysis
```
📄 File: example.exe (2048 bytes)
🔍 Type: PE Executable
📊 Entropy: 6.2341
🔐 MD5: a1b2c3d4e5f6...
📝 Content: Likely executable
💻 Disassembly available
💡 Use --detailed for complete report
```

#### Detailed Analysis
```
=== BINARY ANALYSIS REPORT ===
Size: 2048 bytes
Type: PE Executable
Entropy: 6.2341
MD5: a1b2c3d4e5f6...
SHA256: f1e2d3c4b5a6...

=== HEURISTICS ===
Likely text: NO
Likely executable: YES
Likely compressed: NO

=== PE ANALYSIS ===
Architecture: x64
Sections: .text, .data, .rdata
Entry Point: 0x1400
Imports: kernel32.dll, user32.dll

=== BYTE FREQUENCY (Top 10) ===
  0x00: 150 (7.3%)
  0xFF: 89 (4.3%)
  ...
```

## 🏗️ Architecture

For detailed architecture documentation with visual diagrams, see [ARCHITECTURE.md](ARCHITECTURE.md).

### Project Structure
```
BinaryAnalyzer/
├── Core/                      # Core business logic
│   ├── BinaryParser.cs       # Binary → format conversion
│   ├── FileAnalyzer.cs       # File signature detection
│   ├── Disassembler.cs       # Capstone.NET integration
│   ├── PEAnalyzer.cs         # PE file analysis
│   └── MetadataAnalyzer.cs   # Metadata extraction
├── Utils/                     # Utilities
│   ├── Entropy.cs            # Shannon entropy calculation
│   └── Extensions.cs         # Extension methods
├── Program.cs                 # CLI entry point
└── BinaryAnalyzer.csproj     # Project configuration

BinaryAnalyzer.Tests/          # Test suite
├── Core/                      # Core module tests
├── Utils/                     # Utility tests
└── Integration/               # Integration tests
```

### Key Components

#### BinaryParser
Handles binary-to-text conversion, encoding detection, and format transformations.

#### FileAnalyzer
Provides file type detection using magic number signatures and heuristic analysis.

#### Disassembler
Integrates with Capstone.NET for x86/x64 disassembly capabilities.

#### PEAnalyzer
Specialized analysis for Windows PE (Portable Executable) files.

#### MetadataAnalyzer
Extracts and analyzes file metadata and generates comprehensive reports.

## 🧪 Testing

The project includes comprehensive test coverage with 67 unit and integration tests.

```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter Category=Core
dotnet test --filter Category=Integration
```

### Test Coverage
- ✅ Core modules (BinaryParser, FileAnalyzer, Disassembler, PEAnalyzer, MetadataAnalyzer)
- ✅ Utility functions (Entropy, Extensions)
- ✅ Integration tests (CLI and report generation)
- ✅ Edge cases and error handling

## 🔧 Development

### Adding New File Types
Extend the `MagicNumbers` dictionary in `FileAnalyzer.cs`:
```csharp
{ "NEW_TYPE", new byte[] { 0x???, 0x???, ... } }
```

### Extending Disassembly Features
The `Disassembler.cs` module uses Capstone.NET. Refer to the documentation to add support for other architectures (ARM, MIPS, etc.).

### Recommended Testing
- Files of various sizes (1 KB to 100+ MB)
- Corrupted or partial files
- Highly compressed vs. random data
- Executables with different architectures

## 🐛 Troubleshooting

### Common Issues

**"File not found"**
- Verify the absolute file path
- Check file permissions

**"Disassembly error"**
- File may not contain valid machine code
- Architecture may not be supported

**Memory exceptions**
- Very large files (>1GB) may cause issues
- Consider processing in chunks for large files

### Current Limitations
- Disassembly limited to first instructions
- No support for exotic file formats
- Basic CLI interface (no GUI)

## 🚀 Future Enhancements

### Advanced Features
- **Graphical interface**: WPF or Avalonia UI
- **Malware analysis**: Suspicious pattern detection
- **Report export**: JSON, XML, HTML formats
- **Network analysis**: Packet inspection
- **Database integration**: Store previous analyses

### Integrations
- **VirusTotal API**: Hash verification
- **YARA rules**: Signature detection
- **Hex editor**: Interactive visualization
- **Plugin system**: Modular extensions

## 📚 Dependencies

- **.NET 7.0**: Core framework
- **Capstone.NET**: Disassembly engine (optional)
- **xUnit**: Testing framework
- **System.Security.Cryptography**: Hash calculations

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📞 Support

For questions, bug reports, or feature requests, please open an issue on the repository.

## 📖 Additional Resources

- [Capstone Engine Documentation](http://www.capstone-engine.org/)
- [File Signatures Database](https://www.filesignatures.net/)
- [.NET Binary Data Processing](https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader)
- [PE Format Specification](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format)

---

**Last updated:** July 5, 2025

*Built with ❤️ using C# and .NET*
