using System;
using System.Linq;
using Xunit;

namespace DelegateDecompiller.Tests
{
    public class QueryableExtensionsTests
    {
        [Fact]
        public void InlineProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullName == "Test User"
                          select employee).DecompileExpressions();

            Assert.Equal(expected.Expression.ToString(), actual.Expression.ToString());
        }
    }

    class Employee
    {
        [Decompile]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public string LastName { get; set; }

        public string FirstName { get; set; }
    }
}
