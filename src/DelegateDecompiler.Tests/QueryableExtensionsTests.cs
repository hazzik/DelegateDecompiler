using System;
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
        public void InlineTooDeepProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.TooDeepName == "Test User"
                          select employee).Decompile();

            Assert.Equal(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Fact]
        public void InlinePropertyWithVariableClosure()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            string fullname = "Test User";
            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == fullname
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullName == fullname
                          select employee).Decompile();

            Console.WriteLine(expected);
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
    }
}
