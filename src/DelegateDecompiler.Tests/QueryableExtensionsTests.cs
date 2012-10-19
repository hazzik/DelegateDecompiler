using System.Linq;
using Xunit;

namespace DelegateDecompiler.Tests
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
                          select employee).Decompile();

            Assert.Equal(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Fact]
        public void InlineMethod()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod() == "Test User"
                          select employee).Decompile();

            Assert.Equal(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Fact]
        public void InlineMethodWithArg()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where ("Mr " + employee.FirstName + " " + employee.LastName) == "Mr Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod("Mr ") == "Mr Test User"
                          select employee).Decompile();

            Assert.Equal(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Fact(Skip = "Compiller uses Concat(string[]) overload")]
        public void InlineMethodWithTwoArgs()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where ("Mr " + employee.FirstName + " " + employee.LastName + " Jr.") == "Mr Test User Jr."
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod("Mr ", " Jr.") == "Mr Test User Jr."
                          select employee).Decompile();

            Assert.Equal(expected.Expression.ToString(), actual.Expression.ToString());
        }

        class Employee
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            [Decompile]
            public string FullName
            {
                get { return FirstName + " " + LastName; }
            }

            [Decompile]
            public string FullNameMethod()
            {
                return FirstName + " " + LastName;
            }

            [Decompile]
            public string FullNameMethod(string prefix)
            {
                return prefix + FirstName + " " + LastName;
            }

            [Decompile]
            public string FullNameMethod(string prefix, string postfix)
            {
                return prefix + FirstName + " " + LastName + postfix;
            }
        }
    }
}
