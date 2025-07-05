# 📋 Instructions - Versatile Binary Analyzer

## 🎯 Project Description
A comprehensive binary analyzer in C# (.NET 7) capable of analyzing, converting, and disassembling binary data with a command-line interface.

## 🚀 Installation and Setup

### Prerequisites
- .NET 7.0 SDK or higher
- Visual Studio Code with C# extension
- Windows/Linux/macOS

### Dependency Installation
```bash
cd BinaryAnalyzer
dotnet restore
dotnet build
```

## 💻 Usage

### Basic Command
```bash
dotnet run -- <file_path>
```

### Usage Examples
```bash
# Analyze an executable file
dotnet run -- C:\Windows\System32\notepad.exe

# Analyze an image
dotnet run -- image.png

# Analyze a PDF document
dotnet run -- document.pdf
```

## 🔧 Features

### ✅ Implemented Features
- **File type detection**: Automatic recognition via magic numbers
- **Entropy calculation**: Measuring data randomness
- **Format conversion**: Hex, ASCII, Base64
- **x86/x64 disassembly**: Using Capstone.NET
- **CLI interface**: Simple and intuitive

### 📊 Supported File Types
- **Images**: PNG, JPEG, GIF
- **Documents**: PDF
- **Archives**: ZIP
- **Executables**: PE, ELF (basic detection)
- **Raw data**: Entropy analysis and hex dump

## 📁 Code Structure

```
BinaryAnalyzer/
├── Core/                    # Main business logic
│   ├── BinaryParser.cs     # Binary → format conversion
│   ├── FileAnalyzer.cs     # File signature detection
│   └── Disassembler.cs     # Capstone.NET integration
├── Utils/                   # Utilities
│   ├── Entropy.cs          # Shannon entropy calculation
│   └── Extensions.cs       # Extension methods
├── Properties/
│   └── launchSettings.json # Debug configuration
├── Program.cs              # CLI entry point
└── BinaryAnalyzer.csproj   # Project configuration
```

## 🛠️ Development

### Adding New File Types
Modify the `MagicNumbers` dictionary in `FileAnalyzer.cs`:
```csharp
{ "NEW_TYPE", new byte[] { 0x???, 0x???, ... } }
```

### Extending Disassembly Features
The `Disassembler.cs` module uses Capstone.NET. Consult the documentation to add other architectures (ARM, MIPS, etc.).

### Recommended Testing
- Files of different sizes (1 KB to 100+ MB)
- Corrupted or partial files
- Highly compressed vs. random data
- Executables with different architectures

## 🐛 Troubleshooting

### Common Errors
1. **"File not found"**: Check the absolute file path
2. **"Disassembly error"**: The file may not contain valid machine code
3. **Memory exceptions**: Very large files (>1GB) may cause problems

### Current Limitations
- Disassembly limited to first instructions
- No support for exotic formats
- Basic CLI interface (no GUI)

## 📚 Possible Extensions

### Advanced Features
- **Graphical interface**: WPF or Avalonia UI
- **Malware analysis**: Suspicious pattern detection
- **Report export**: JSON, XML, HTML
- **Network support**: Packet analysis
- **Database**: Storage of previous analyses

### Integrations
- **VirusTotal API**: Hash verification
- **YARA rules**: Signature detection
- **Hex editor**: Interactive visualization
- **Plugin system**: Modular extension

## 📖 Useful Resources
- [Capstone Engine Documentation](http://www.capstone-engine.org/)
- [File Signatures Database](https://www.filesignatures.net/)
- [.NET Binary Data Processing](https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader)

---
*Last updated: July 4, 2025*
