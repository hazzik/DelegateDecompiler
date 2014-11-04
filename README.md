DelegateDecompiler
====================

A tool which is able to decompile a delegate or a method body to its lambda representation

<a href='https://pledgie.com/campaigns/27088'><img alt='Click here to lend your support to: DelegateDecompiler and make a donation at pledgie.com !' src='https://pledgie.com/campaigns/27088.png?skin_name=chrome' border='0' ></a>

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

If your class doesn't have a [Computed] attribute, you can use the `.Computed()` extension method..

    var employees = (from employee in db.Employees
                     where employee.FullName.Computed() == "Test User"
                     select employee).ToList();

					 
Using the new Decompiled method you can call methods that return a single item (Any, Count, First, Single, etc) as well as other methods in identical way like this:

	bool exists = db.Employees.Decompiled().Any(employee => employee.FullName == "Test User");	

	var employees = db.Employees.Where(employee => employee.FullName == "Test User"));
			 
					 
# Installation

Available on [NuGet](https://nuget.org/packages/DelegateDecompiler)

# License

MIT license - http://opensource.org/licenses/mit

[![Bitdeli Badge](https://d2weczhvl823v0.cloudfront.net/hazzik/delegatedecompiler/trend.png)](https://bitdeli.com/free "Bitdeli Badge")

