using System;
using System.Linq;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class QueryableExtensionsTests
    {
        [Fact]
        public void InlinePropertyWithoutAttribute()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameWithoutAttribute.Computed() == "Test User"
                          select employee).Decompile();

            Assert.Equal(expected.Expression.ToString(), actual.Expression.ToString());
        }

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
        public void InlineBooleanProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where true
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.IsActive
                          select employee).Decompile();

            Assert.Equal(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Fact]
        public void TestLdflda()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                where employee.Reference.Count == 0
                select employee);

            var actual = (from employee in employees.AsQueryable()
                where employee.Count == 0
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

        [Fact]
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
