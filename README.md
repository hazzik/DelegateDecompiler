DelegateDecompiler
====================

A tool which is able to decompile a delegate or a method body to its lambda representation


## Examples

### Using computed properties in linq.

Asume we have a class with a computed property

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

And you are going to query employees by their full names

    var employees = (from employee in db.Employees
                     where employee.FullName == "Test User"
                     select employee).Decompile().ToList();

When you call `.Decompile` method it decompiles your computed properties to their underlying representation and the query will become simmilar to the following query

    var employees = (from employee in db.Employees
                     where (employee.FirstName + " " + employee.LastName)  == "Test User"
                     select employee).ToList();
