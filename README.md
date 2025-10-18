# DelegateDecompiler

![https://ci.appveyor.com/project/hazzik/delegatedecompiler/branch/main](https://ci.appveyor.com/api/projects/status/github/hazzik/delegatedecompiler?branch=main&svg=true)
![https://nuget.org/packages/DelegateDecompiler](https://img.shields.io/nuget/dt/DelegateDecompiler.svg)

A library that is able to decompile a delegate or a method body to their lambda representation

## Sponsorship

If you like the library please consider [supporting my work](https://github.com/sponsors/hazzik).

## Examples

### Using computed properties in linq.

Assume we have a class with a computed property

```csharp
class Employee
{
    [Computed]
    public string FullName => FirstName + " " + LastName;
    public string LastName { get; set; }
    public string FirstName { get; set; }
}
```

And you are going to query employees by their full names

```csharp
var employees = (from employee in db.Employees
                 where employee.FullName == "Test User"
                 select employee).Decompile().ToList();
```

When you call `.Decompile` method it decompiles your computed properties to their underlying representation and the query will become simmilar to the following query

```csharp
var employees = (from employee in db.Employees
                 where (employee.FirstName + " " + employee.LastName)  == "Test User"
                 select employee).ToList();
```

If your class doesn't have a [Computed] attribute, you can use the `.Computed()` extension method..

```csharp
var employees = (from employee in db.Employees
                 where employee.FullName.Computed() == "Test User"
                 select employee).ToList();
```

Also, you can call methods that return a single item (Any, Count, First, Single, etc) as well as other methods in identical way like this:

```csharp
bool exists = db.Employees.Decompile().Any(employee => employee.FullName == "Test User");
```

Again, the `FullName` property will be decompiled:

```csharp
bool exists = db.Employees.Any(employee => (employee.FirstName + " " + employee.LastName) == "Test User");
```

## Limitations

Not every compiled code can be represented as a lambda expression. Some cases are explicitly not supported, other can break and produce unexpected results.

Some of the known cases listed below

### Loops

Loops often cannot be represented as an expression tree. 

So, the following imperative code would probably throw a `StackOverflowException`:

```csharp
var total = 0;
foreach (var item in this.Items) { total += item.TotalPrice; }
return total;
```

Instead, write it as a declarative Linq expression, which would be supported.

```csharp
return this.Items.Sum(i => i.TotalPrice);
```

### Recursion and self-referencing

Recursion and self-referencing of computed properties cannot be represented as an Expression tree, 
and would probably throw `StackOverflowException` similarly to loops.

### Pattern matching with `is ... or ...`

`is ... or ...` pattern matching cannot always be decompiled due to compiler optimizations. The compiler may optimize patterns to use comparison operators, making it impossible to distinguish between genuine comparisons and optimized patterns.

For example:
```csharp
enum TestEnum { Foo, Bar, Baz }

// This pattern matching:
x is TestEnum.Foo or TestEnum.Bar

// Might compile to:
(uint)x <= 1
```

## Using with EntityFramework and other ORMs

If you are using ORM specific features, like EF's `Include`, `AsNoTracking` or NH's `Fetch` then `Decompile` method should be called after all ORM specific methods, otherwise it may not work. Ideally use `Decompile` extension method just before materialization methods such as `ToList`, `ToArray`, `First`, `FirstOrDefault`, `Count`, `Any`, and etc.

### Async Support with [EntityFramework 6](https://www.nuget.org/packages/DelegateDecompiler.EntityFramework)

The [DelegateDecompiler.EntityFramework](https://nuget.org/packages/DelegateDecompiler.EntityFramework) package provides `DecompileAsync` extension method which adds support for EF's Async operations.
 
### Async Support with [EntityFramework Core 5.0 and later](https://www.nuget.org/packages/DelegateDecompiler.EntityFrameworkCore5)

The [DelegateDecompiler.EntityFrameworkCore5](https://nuget.org/packages/DelegateDecompiler.EntityFrameworkCore5) package provides `DecompileAsync` extension method which adds support for EF's Async operations.

#### Automatic decompilation

You can configure DelegateDecompiler to automatically decompile all EF Core queries:

```csharp
public class YourDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options) {
        options.AddDelegateDecompiler();
        // Other configuration
    }
}
```

With this approach you would not need to call `Decompile` or `DecompileAsync` methods on queries.
 
# Installation

Available on [NuGet](https://nuget.org/)

* Install-Package [DelegateDecompiler](https://nuget.org/packages/DelegateDecompiler)
* Install-Package [DelegateDecompiler.EntityFramework](https://nuget.org/packages/DelegateDecompiler.EntityFramework)
* Install-Package [DelegateDecompiler.EntityFrameworkCore5](https://nuget.org/packages/DelegateDecompiler.EntityFrameworkCore5)

# License

MIT license - http://opensource.org/licenses/mit
