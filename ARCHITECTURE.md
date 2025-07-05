# 🏗️ Binary Analyzer Architecture

This document provides a comprehensive overview of the Binary Analyzer's architecture, component relationships, and data flow using visual diagrams.

## 📋 Table of Contents

- [System Overview](#system-overview)
- [Component Architecture](#component-architecture)
- [Data Flow](#data-flow)
- [Module Dependencies](#module-dependencies)
- [File Processing Pipeline](#file-processing-pipeline)
- [Class Relationships](#class-relationships)
- [Testing Architecture](#testing-architecture)
- [Deployment Architecture](#deployment-architecture)

## 🔍 System Overview

The Binary Analyzer is designed as a modular, extensible system for analyzing binary files with clear separation of concerns.

```mermaid
graph TB
    subgraph "User Interface Layer"
        CLI[Command Line Interface]
        Args[Argument Parser]
    end
    
    subgraph "Core Analysis Engine"
        BA[BinaryParser]
        FA[FileAnalyzer] 
        DA[Disassembler]
        PE[PEAnalyzer]
        MA[MetadataAnalyzer]
    end
    
    subgraph "Utility Layer"
        ENT[Entropy Calculator]
        EXT[Extensions]
        HASH[Hash Calculator]
    end
    
    subgraph "External Dependencies"
        CAP[Capstone.NET]
        NET[.NET Framework]
        CRYPTO[System.Security.Cryptography]
    end
    
    CLI --> Args
    Args --> BA
    Args --> FA
    Args --> DA
    Args --> PE
    Args --> MA
    
    BA --> ENT
    BA --> EXT
    FA --> HASH
    DA --> CAP
    PE --> CRYPTO
    MA --> NET
    
    style CLI fill:#e1f5fe
    style CAP fill:#fff3e0
    style NET fill:#fff3e0
    style CRYPTO fill:#fff3e0
```

## 🧩 Component Architecture

### Core Components Structure

```mermaid
graph LR
    subgraph "BinaryAnalyzer Project"
        subgraph "Core/"
            BP[BinaryParser.cs]
            FA[FileAnalyzer.cs]
            DS[Disassembler.cs]
            PE[PEAnalyzer.cs]
            MA[MetadataAnalyzer.cs]
        end
        
        subgraph "Utils/"
            ENT[Entropy.cs]
            EXT[Extensions.cs]
        end
        
        PROG[Program.cs]
        PROJ[BinaryAnalyzer.csproj]
    end
    
    subgraph "BinaryAnalyzer.Tests Project"
        subgraph "Core/"
            BPT[BinaryParserTests.cs]
            FAT[FileAnalyzerTests.cs]
            DST[DisassemblerTests.cs]
            PET[PEAnalyzerTests.cs]
            MAT[MetadataAnalyzerTests.cs]
        end
        
        subgraph "Utils/"
            ENTT[EntropyTests.cs]
            EXTT[ExtensionsTests.cs]
        end
        
        subgraph "Integration/"
            INT[ProgramIntegrationTests.cs]
        end
        
        TESTPROJ[BinaryAnalyzer.Tests.csproj]
    end
    
    BP -.-> BPT
    FA -.-> FAT
    DS -.-> DST
    PE -.-> PET
    MA -.-> MAT
    ENT -.-> ENTT
    EXT -.-> EXTT
    PROG -.-> INT
    
    style BP fill:#e8f5e8
    style FA fill:#e8f5e8
    style DS fill:#e8f5e8
    style PE fill:#e8f5e8
    style MA fill:#e8f5e8
    style BPT fill:#fff3e0
    style FAT fill:#fff3e0
    style DST fill:#fff3e0
    style PET fill:#fff3e0
    style MAT fill:#fff3e0
```

## 🔄 Data Flow

### File Analysis Pipeline

```mermaid
flowchart TD
    START([Start Analysis]) --> INPUT[Read File Input]
    INPUT --> VALIDATE{File Exists?}
    
    VALIDATE -->|No| ERROR[Error: File Not Found]
    VALIDATE -->|Yes| DETECT[File Type Detection]
    
    DETECT --> MAGIC[Magic Number Check]
    MAGIC --> HEURISTIC[Heuristic Analysis]
    HEURISTIC --> ENTROPY[Calculate Entropy]
    
    ENTROPY --> MODE{Analysis Mode?}
    
    MODE -->|Quick| QUICK_OUT[Quick Report Output]
    MODE -->|Detailed| DETAILED[Detailed Analysis]
    MODE -->|Hexdump| HEX[Hexadecimal Dump]
    
    DETAILED --> HASH[Calculate Hashes]
    HASH --> STRINGS[Extract Strings]
    STRINGS --> FREQ[Byte Frequency]
    FREQ --> SPEC{Specific Analysis?}
    
    SPEC -->|PE File| PE_ANALYSIS[PE Analysis]
    SPEC -->|Executable| DISASM[Disassembly]
    SPEC -->|Other| METADATA[Metadata Extraction]
    
    PE_ANALYSIS --> DETAILED_OUT[Detailed Report]
    DISASM --> DETAILED_OUT
    METADATA --> DETAILED_OUT
    
    HEX --> HEX_OUT[Hexdump Output]
    QUICK_OUT --> END([End])
    DETAILED_OUT --> END
    HEX_OUT --> END
    ERROR --> END
    
    style START fill:#e8f5e8
    style END fill:#ffebee
    style ERROR fill:#ffcdd2
    style VALIDATE fill:#fff3e0
    style MODE fill:#fff3e0
    style SPEC fill:#fff3e0
```

## 🔗 Module Dependencies

### Dependency Graph

```mermaid
graph TD
    PROGRAM[Program.cs] --> CLI_ARGS[Command Line Parsing]
    
    CLI_ARGS --> BINARY_PARSER[BinaryParser]
    CLI_ARGS --> FILE_ANALYZER[FileAnalyzer]
    CLI_ARGS --> DISASSEMBLER[Disassembler]
    CLI_ARGS --> PE_ANALYZER[PEAnalyzer]
    CLI_ARGS --> METADATA_ANALYZER[MetadataAnalyzer]
    
    BINARY_PARSER --> ENTROPY[Entropy Utils]
    BINARY_PARSER --> EXTENSIONS[Extensions Utils]
    
    FILE_ANALYZER --> ENTROPY
    FILE_ANALYZER --> HASH_UTILS[Hash Utilities]
    
    DISASSEMBLER --> CAPSTONE[Capstone.NET]
    
    PE_ANALYZER --> BINARY_PARSER
    PE_ANALYZER --> CRYPTO[System.Security.Cryptography]
    
    METADATA_ANALYZER --> FILE_ANALYZER
    METADATA_ANALYZER --> BINARY_PARSER
    
    ENTROPY --> MATH[System.Math]
    EXTENSIONS --> SYSTEM_LINQ[System.Linq]
    
    style PROGRAM fill:#e1f5fe
    style CAPSTONE fill:#fff3e0
    style CRYPTO fill:#fff3e0
    style MATH fill:#fff3e0
    style SYSTEM_LINQ fill:#fff3e0
```

## 🔧 File Processing Pipeline

### Processing Stages

```mermaid
stateDiagram-v2
    [*] --> FileInput
    FileInput --> Validation
    
    Validation --> FileNotFound : File doesn't exist
    Validation --> FileTypeDetection : File exists
    
    FileNotFound --> [*]
    
    FileTypeDetection --> MagicNumberCheck
    MagicNumberCheck --> HeuristicAnalysis
    HeuristicAnalysis --> EntropyCalculation
    
    EntropyCalculation --> ModeSelection
    
    ModeSelection --> QuickAnalysis : --quick
    ModeSelection --> DetailedAnalysis : --detailed
    ModeSelection --> HexdumpOutput : --hexdump
    
    QuickAnalysis --> QuickReport
    QuickReport --> [*]
    
    DetailedAnalysis --> HashCalculation
    HashCalculation --> StringExtraction
    StringExtraction --> ByteFrequency
    ByteFrequency --> SpecializedAnalysis
    
    SpecializedAnalysis --> PEAnalysis : PE File
    SpecializedAnalysis --> DisassemblyAnalysis : Executable
    SpecializedAnalysis --> MetadataExtraction : Other
    
    PEAnalysis --> DetailedReport
    DisassemblyAnalysis --> DetailedReport
    MetadataExtraction --> DetailedReport
    DetailedReport --> [*]
    
    HexdumpOutput --> HexReport
    HexReport --> [*]
```

## 🏛️ Class Relationships

### Core Classes UML

```mermaid
classDiagram
    class Program {
        +Main(string[] args)
        -ParseArguments(string[] args)
        -ProcessFile(string filePath, AnalysisMode mode)
    }
    
    class BinaryParser {
        +ParseBinary(byte[] data) string
        +DetectEncoding(byte[] data) Encoding
        +ConvertToHex(byte[] data) string
        +ConvertToBase64(byte[] data) string
        +ExtractStrings(byte[] data) List~string~
    }
    
    class FileAnalyzer {
        +AnalyzeFile(string filePath) FileAnalysisResult
        +DetectFileType(byte[] data) string
        +CalculateHashes(byte[] data) HashResult
        +GetByteFrequency(byte[] data) Dictionary~byte, int~
        -MagicNumbers Dictionary~string, byte[]~
    }
    
    class Disassembler {
        +DisassembleBytes(byte[] data) List~Instruction~
        +DisassembleFile(string filePath) DisassemblyResult
        -InitializeCapstone() CapstoneDisassembler
    }
    
    class PEAnalyzer {
        +AnalyzePE(string filePath) PEAnalysisResult
        +GetSections() List~PESection~
        +GetImports() List~ImportedFunction~
        +GetExports() List~ExportedFunction~
    }
    
    class MetadataAnalyzer {
        +ExtractMetadata(string filePath) MetadataResult
        +AnalyzeImageMetadata(byte[] data) ImageMetadata
        +AnalyzeDocumentMetadata(byte[] data) DocumentMetadata
    }
    
    class Entropy {
        +Calculate(byte[] data) double
        +CalculateShannon(byte[] data) double
        +ClassifyEntropy(double entropy) EntropyClassification
    }
    
    class Extensions {
        +ToHexString(byte[] bytes) string
        +IsPrintable(char c) bool
        +ChunkBy(IEnumerable~T~ source, int size) IEnumerable~T[]~
    }
    
    Program --> BinaryParser
    Program --> FileAnalyzer
    Program --> Disassembler
    Program --> PEAnalyzer
    Program --> MetadataAnalyzer
    
    BinaryParser --> Entropy
    BinaryParser --> Extensions
    FileAnalyzer --> Entropy
    PEAnalyzer --> BinaryParser
    MetadataAnalyzer --> FileAnalyzer
    MetadataAnalyzer --> BinaryParser
```

## 🧪 Testing Architecture

### Test Structure

```mermaid
graph TB
    subgraph "Test Categories"
        UNIT[Unit Tests]
        INTEGRATION[Integration Tests]
        PERFORMANCE[Performance Tests]
    end
    
    subgraph "Unit Tests"
        CORE_TESTS[Core Module Tests]
        UTIL_TESTS[Utility Tests]
        
        CORE_TESTS --> BP_TEST[BinaryParser Tests]
        CORE_TESTS --> FA_TEST[FileAnalyzer Tests]
        CORE_TESTS --> DA_TEST[Disassembler Tests]
        CORE_TESTS --> PE_TEST[PEAnalyzer Tests]
        CORE_TESTS --> MA_TEST[MetadataAnalyzer Tests]
        
        UTIL_TESTS --> ENT_TEST[Entropy Tests]
        UTIL_TESTS --> EXT_TEST[Extensions Tests]
    end
    
    subgraph "Integration Tests"
        CLI_TEST[CLI Integration]
        FILE_TEST[File Processing]
        REPORT_TEST[Report Generation]
    end
    
    subgraph "Test Data"
        SAMPLE_PE[Sample PE Files]
        SAMPLE_IMG[Sample Images]
        SAMPLE_DOC[Sample Documents]
        SAMPLE_BIN[Sample Binary Data]
    end
    
    BP_TEST -.-> SAMPLE_BIN
    FA_TEST -.-> SAMPLE_PE
    FA_TEST -.-> SAMPLE_IMG
    DA_TEST -.-> SAMPLE_PE
    PE_TEST -.-> SAMPLE_PE
    MA_TEST -.-> SAMPLE_DOC
    
    CLI_TEST -.-> SAMPLE_PE
    FILE_TEST -.-> SAMPLE_IMG
    REPORT_TEST -.-> SAMPLE_BIN
    
    style UNIT fill:#e8f5e8
    style INTEGRATION fill:#fff3e0
    style PERFORMANCE fill:#ffebee
```

## 🚀 Deployment Architecture

### Build and Deployment Pipeline

```mermaid
graph LR
    subgraph "Development"
        SRC[Source Code]
        TESTS[Unit Tests]
        DOC[Documentation]
    end
    
    subgraph "Build Process"
        RESTORE[dotnet restore]
        BUILD[dotnet build]
        TEST[dotnet test]
        PACK[dotnet publish]
    end
    
    subgraph "Artifacts"
        EXE[Executable]
        DEPS[Dependencies]
        CONFIG[Configuration]
    end
    
    subgraph "Deployment Targets"
        WIN[Windows x64]
        LINUX[Linux x64]
        MAC[macOS x64]
        PORTABLE[Framework Dependent]
    end
    
    SRC --> RESTORE
    TESTS --> TEST
    DOC --> BUILD
    
    RESTORE --> BUILD
    BUILD --> TEST
    TEST --> PACK
    
    PACK --> EXE
    PACK --> DEPS
    PACK --> CONFIG
    
    EXE --> WIN
    EXE --> LINUX
    EXE --> MAC
    EXE --> PORTABLE
    
    DEPS --> WIN
    DEPS --> LINUX
    DEPS --> MAC
    DEPS --> PORTABLE
    
    style SRC fill:#e8f5e8
    style TESTS fill:#fff3e0
    style EXE fill:#e1f5fe
    style WIN fill:#ffebee
    style LINUX fill:#ffebee
    style MAC fill:#ffebee
    style PORTABLE fill:#ffebee
```

## 📊 Performance Considerations

### Memory and Processing Flow

```mermaid
graph TD
    FILE_INPUT[File Input] --> SIZE_CHECK{File Size Check}
    
    SIZE_CHECK -->|< 100MB| MEMORY_LOAD[Load to Memory]
    SIZE_CHECK -->|> 100MB| STREAM_PROCESS[Stream Processing]
    
    MEMORY_LOAD --> ANALYSIS[In-Memory Analysis]
    STREAM_PROCESS --> CHUNK_ANALYSIS[Chunked Analysis]
    
    ANALYSIS --> QUICK_PATH{Quick Mode?}
    CHUNK_ANALYSIS --> QUICK_PATH
    
    QUICK_PATH -->|Yes| BASIC_ANALYSIS[Basic Analysis Only]
    QUICK_PATH -->|No| FULL_ANALYSIS[Full Analysis]
    
    BASIC_ANALYSIS --> MEMORY_CLEANUP[Memory Cleanup]
    FULL_ANALYSIS --> CACHE_RESULTS[Cache Results]
    
    CACHE_RESULTS --> MEMORY_CLEANUP
    MEMORY_CLEANUP --> OUTPUT[Generate Output]
    
    style FILE_INPUT fill:#e8f5e8
    style SIZE_CHECK fill:#fff3e0
    style QUICK_PATH fill:#fff3e0
    style MEMORY_CLEANUP fill:#ffebee
    style OUTPUT fill:#e1f5fe
```

## 🔧 Extension Points

### Plugin Architecture (Future)

```mermaid
graph TB
    subgraph "Core System"
        CORE[Core Engine]
        PLUGIN_MGR[Plugin Manager]
        API[Plugin API]
    end
    
    subgraph "Built-in Analyzers"
        PE[PE Analyzer]
        ELF[ELF Analyzer]
        MACH[Mach-O Analyzer]
    end
    
    subgraph "Custom Plugins"
        MALWARE[Malware Scanner]
        CRYPTO[Crypto Analyzer]
        NETWORK[Network Analyzer]
        CUSTOM[Custom Analyzers]
    end
    
    CORE --> PLUGIN_MGR
    PLUGIN_MGR --> API
    
    API --> PE
    API --> ELF
    API --> MACH
    
    API -.-> MALWARE
    API -.-> CRYPTO
    API -.-> NETWORK
    API -.-> CUSTOM
    
    style CORE fill:#e8f5e8
    style PLUGIN_MGR fill:#fff3e0
    style API fill:#e1f5fe
    style MALWARE fill:#ffebee
    style CRYPTO fill:#ffebee
    style NETWORK fill:#ffebee
    style CUSTOM fill:#ffebee
```

## 📚 Architecture Principles

### Design Patterns Used

1. **Single Responsibility Principle**: Each analyzer handles one specific type of analysis
2. **Dependency Injection**: Core components are loosely coupled
3. **Factory Pattern**: File type detection and analyzer selection
4. **Strategy Pattern**: Different analysis modes (quick, detailed, hexdump)
5. **Observer Pattern**: Progress reporting and logging (future enhancement)

### Key Architectural Decisions

- **Modular Design**: Each analyzer is independent and can be extended
- **Immutable Data**: Analysis results are read-only objects
- **Stream Processing**: Large files are processed in chunks
- **Error Isolation**: Failures in one analyzer don't affect others
- **Extensible Framework**: Easy to add new file format support

---

*This architecture documentation is maintained alongside the codebase and should be updated when significant structural changes are made.*
