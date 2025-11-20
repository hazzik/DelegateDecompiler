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
# Restore packages
dotnet restore

# Build the solution
dotnet build -c Debug

# Run tests
dotnet test -c Debug -f net8.0 src/DelegateDecompiler.Tests
dotnet test -c Debug -f net8.0 src/DelegateDecompiler.Tests.VB
dotnet test -c Debug -f net8.0 src/DelegateDecompiler.EntityFramework.Tests
dotnet test -c Debug -f net8.0 src/DelegateDecompiler.EntityFrameworkCore6.Tests
dotnet test -c Debug -f net8.0 src/DelegateDecompiler.EntityFrameworkCore8.Tests
dotnet test -c Debug -f net9.0 src/DelegateDecompiler.EntityFrameworkCore9.Tests
dotnet test -c Debug -f net10.0 src\DelegateDecompiler.EntityFrameworkCore10.Tests
```

Use `-p:DisableGitVersionTask=true` flag to avoid build issues if GitVersion is not set up.

**Development Guidelines:**

**Test-First Development (MANDATORY):**
- **ALWAYS START FROM TESTS** - Write test cases that define expected behavior BEFORE implementing
- **TESTS ARE THE SOURCE OF TRUTH** - Never change tests to match broken code
- If tests fail: fix implementation, not tests
- Tests define the API contract and expected behavior

**Required Workflow:**
1. **Run tests first** - See exact state of the system
2. **Add/modify tests** - Define expected behavior
3. **Run tests after each change** - Verify impact immediately
4. **Change one thing at a time** - Make small, isolated changes
5. **Verify all tests pass** - Ensure no regressions at the end
6. **NEVER modify tests to match implementation bugs** - Fix code, not tests

**Core Requirements:**
- All tests in `src/DelegateDecompiler.Tests` and `src/DelegateDecompiler.Tests.VB` must pass
- Run tests frequently during development
- Use `-p:DisableGitVersionTask=true` if GitVersion issues occur

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
