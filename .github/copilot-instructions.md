# DelegateDecompiler - GitHub Copilot Instructions

## Project Overview

DelegateDecompiler is a .NET library that decompiles delegates and method bodies to their lambda representation. It enables computed properties in LINQ queries by translating `[Computed]` attributes into executable expressions that can be processed by Entity Framework and other ORMs.

## Core Components

- **`ComputedAttribute`**: Marks properties/methods for decompilation
- **`MethodBodyDecompiler`**: Core IL decompilation engine  
- **`DecompileExtensions`**: Public API for decompiling delegates/queries
- **`Processors/`**: Pluggable processors for different IL opcodes

## Build & Test

```bash
# Build the solution
dotnet restore
dotnet build --no-restore -c Release DelegateDecompiler.sln

# Run tests (requires building first)
.\test.cmd
```

## Project Structure

```
src/
├── DelegateDecompiler/                          # Core library
├── DelegateDecompiler.EntityFramework/          # EF 6.x support
├── DelegateDecompiler.EntityFrameworkCore5/     # EF Core 5+ support 
├── DelegateDecompiler.Tests/                    # Core library tests
└── DelegateDecompiler.EntityFramework*.Tests/   # EF integration tests
```

## Usage Example

```csharp
class Employee
{
    [Computed]
    public string FullName => FirstName + " " + LastName;
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// Usage in LINQ - decompiles FullName to FirstName + " " + LastName
var results = employees
    .Where(e => e.FullName == "John Doe")
    .Decompile()
    .ToList();
```

## Development Notes

- **Target Frameworks**: .NET Framework 4.0/4.5, .NET Standard 2.0/2.1, .NET 8.0/9.0
- **Key Dependency**: Mono.Reflection for IL reading
- **Testing**: NUnit with automatic documentation generation for EF tests
- **Architecture**: Pluggable processor pattern for handling IL opcodes
- **Performance**: Decompilation results are cached using `ConcurrentDictionary`