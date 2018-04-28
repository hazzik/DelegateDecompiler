DelegateDecompiler
====================

A tool which is able to decompile a delegate or a method body to its lambda representation

## Examples

### Using computed properties in linq.

Asume we have a class with a computed property

```csharp
class Employee
{
    [Computed]
    public string FullName
    {
        get { return FirstName + " " + LastName; }
    }

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

## Using with EntityFramework and other ORMs

If you are using ORM specific features, like EF's `Include`, `AsNoTracking` or NH's `Fetch` then `Decompile` method should be called after all ORM specific methods, otherwise it may not work. Ideally use `Decompile` extension method just before materialization methods such as `ToList`, `ToArray`, `First`, `FirstOrDefault`, `Count`, `Any`, and etc.

### Async Support with [EntityFramework 6](https://www.nuget.org/packages/DelegateDecompiler.EntityFramework)

The [DelegateDecompiler.EntityFramework](https://nuget.org/packages/DelegateDecompiler.EntityFramework) package provides `DecompileAsync` extension method which adds support for EF's Async operations.
 
### Async Support with [EntityFramework Core](https://www.nuget.org/packages/DelegateDecompiler.EntityFrameworkCore)

The [DelegateDecompiler.EntityFrameworkCore](https://nuget.org/packages/DelegateDecompiler.EntityFrameworkCore) package provides `DecompileAsync` extension method which adds support for EF's Async operations.
 
# Installation

Available on [NuGet](https://nuget.org/)

* Install-Package [DelegateDecompiler](https://nuget.org/packages/DelegateDecompiler)
* Install-Package [DelegateDecompiler.EntityFramework](https://nuget.org/packages/DelegateDecompiler.EntityFramework)
* Install-Package [DelegateDecompiler.EntityFrameworkCore](https://nuget.org/packages/DelegateDecompiler.EntityFrameworkCore)

# License

MIT license - http://opensource.org/licenses/mit
