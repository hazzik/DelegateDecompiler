# DelegateDecompiler - GitHub Copilot Instructions

## Project Overview

DelegateDecompiler is a .NET library that decompiles delegates and method bodies to their lambda representation. It enables computed properties in LINQ queries by translating `[Computed]` attributes into executable expressions that can be processed by Entity Framework and other ORMs.

### Key Purpose
- Convert method implementations marked with `[Computed]` attribute into LINQ expressions
- Enable complex computed properties in database queries
- Support multiple Entity Framework versions and .NET Framework/Core versions
- Provide IL (Intermediate Language) decompilation capabilities using Mono.Reflection

## Project Structure

```
src/
├── DelegateDecompiler/                          # Core library
├── DelegateDecompiler.EntityFramework/          # EF 6.x support
├── DelegateDecompiler.EntityFrameworkCore5/     # EF Core 5 support 
├── DelegateDecompiler.EntityFrameworkCore6.Tests/ # EF Core 6 tests
├── DelegateDecompiler.EntityFrameworkCore8.Tests/ # EF Core 8 tests
├── DelegateDecompiler.EntityFrameworkCore9.Tests/ # EF Core 9 tests
├── DelegateDecompiler.EntityFramework.Tests/    # EF 6.x tests
├── DelegateDecompiler.Tests/                    # Core library tests
└── DelegateDecompiler.Tests.VB/                 # VB.NET tests
```

## Core Components

### Main Library (`DelegateDecompiler/`)
- **`ComputedAttribute`**: Marks properties/methods for decompilation
- **`MethodBodyDecompiler`**: Core IL decompilation engine
- **`DecompileExtensions`**: Public API for decompiling delegates/queries
- **`DecompileExpressionVisitor`**: Expression tree visitor for transformation
- **`Processors/`**: Pluggable processors for different IL opcodes

### Key Concepts
- **Computed Properties**: Properties marked with `[Computed]` that are decompiled at runtime
- **IL Decompilation**: Converting CIL bytecode back to expression trees
- **Expression Visitors**: Classes that traverse and transform expression trees
- **Processor Architecture**: Modular system for handling different IL instructions

## Architecture Patterns

### Processor Pattern
The library uses a pluggable processor architecture where each processor handles specific IL opcodes:
- `ConstantProcessor`: Handles constant values
- `LdfldProcessor`: Handles field loading
- `ConvertProcessor`: Handles type conversions
- `UnaryExpressionProcessor`: Handles unary operations

### Expression Visitor Pattern
Multiple expression visitors handle different transformation aspects:
- `DecompileExpressionVisitor`: Main decompilation logic
- `OptimizeExpressionVisitor`: Optimizes generated expressions
- `ReplaceExpressionVisitor`: Replaces expression nodes

## Testing Strategy

### Test Organization
Tests are organized by functionality and Entity Framework version:

1. **Core Tests** (`DelegateDecompiler.Tests/`):
   - Unit tests for decompilation engine
   - Tests for specific language features (closures, generics, etc.)
   - Issue regression tests (Issue13.cs, Issue67.cs, etc.)

2. **Entity Framework Tests**:
   - Structured test groups using naming conventions
   - `TestGroup##` directories for categorization
   - Automatic documentation generation from test results

### Test Naming Conventions
- Test groups: `TestGroup##Description` (e.g., `TestGroup05BasicFeatures`)
- Test files: `Test##Description.cs` (e.g., `Test01Select.cs`)
- Test methods: `Test##Description` where ## provides ordering

### Documentation Generation
The EF test projects automatically generate documentation:
- `SummaryOfSupportedCommands.md`: High-level feature support
- `DetailedListOfSupportedCommands.md`: Detailed test results
- `DetailedListOfSupportedCommandsWithSQL.md`: Includes SQL output

## Development Guidelines

### Code Style
- C# 12.0 language features enabled
- Assembly signing with `DelegateDecompiler.snk`
- Treat warnings as errors (`TreatWarningsAsErrors=True`)
- EditorConfig defines naming conventions (PascalCase for types, interfaces start with 'I')

### Target Frameworks
- .NET Framework 4.0, 4.5
- .NET Standard 2.0, 2.1
- .NET 8.0, 9.0 for tests

### Dependencies
- **Mono.Reflection**: Core IL reading capabilities
- **NUnit**: Testing framework
- **Entity Framework**: Various versions for ORM integration

## Build & Deployment

### Build Process
- MSBuild-based with `Directory.Build.props` for common settings
- GitVersion for automatic versioning
- Strong-name signing for assemblies
- NuGet package generation on build

### Scripts
- `build.cmd`: Build the solution
- `test.cmd`: Run all tests across frameworks
- `!release.cmd`: Release process

### CI/CD
- AppVeyor for continuous integration
- Automatic NuGet package publishing
- Support for multiple .NET versions

## Common Patterns for Contributors

### Adding New Features
1. Add core logic to `DelegateDecompiler/` project
2. Create unit tests in `DelegateDecompiler.Tests/`
3. Add Entity Framework integration tests if applicable
4. Update README.md with examples if it's a public feature

### Adding IL Processor Support
1. Create new processor class implementing `IProcessor`
2. Add to processor registration in main decompiler
3. Add comprehensive tests covering edge cases
4. Document supported IL opcodes

### Adding Entity Framework Support
1. Follow existing test group naming conventions
2. Create test methods that compare LINQ vs Decompiled results
3. Use `CompareAndLogSingleton`/`CompareAndLogList` for validation
4. Tests automatically generate documentation

## Debugging Tips

### Common Issues
- **IL Decompilation Failures**: Check if method contains unsupported IL opcodes
- **Expression Tree Complexity**: Some compiler optimizations may not be supported
- **Entity Framework Translation**: Not all decompiled expressions translate to SQL

### Debug Information
- Enable `Microsoft.NETFramework.ReferenceAssemblies` for multi-targeting
- Use `ContinuousIntegrationBuild` for reproducible builds
- Source link enabled for debugging into source

## Testing Specific Guidance

### Entity Framework Test Patterns
```csharp
[Test]
public void TestFeatureName()
{
    var env = new EfTestContext();
    
    // Test LINQ version first
    var linqResult = env.Database.Items
        .Where(x => x.SomeProperty == "value")
        .Select(x => x.ComputedProperty);
    
    // Signal switch to decompiled version
    env.AboutToUseDelegateDecompiler();
    
    // Test decompiled version
    var decompiled = env.Database.Items
        .Where(x => x.SomeProperty == "value") 
        .Select(x => x.ComputedProperty)
        .Decompile();
        
    // Compare results
    env.CompareAndLogList(linqResult, decompiled);
}
```

### Documentation Generation
- Tests automatically generate markdown documentation
- Use descriptive test names as they appear in docs
- Group related functionality in same test group
- Test failures help identify unsupported scenarios

This project requires careful attention to IL decompilation accuracy, Entity Framework compatibility, and comprehensive testing across multiple .NET versions.