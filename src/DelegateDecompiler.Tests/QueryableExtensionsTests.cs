using System;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class QueryableExtensionsTests : DecompilerTestsBase
    {
        [Test]
        public void InlinePropertyWithoutAttribute()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameWithoutAttribute.Computed() == "Test User"
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullName == "Test User"
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlinePropertyNonGeneric()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (
                from employee in employees.AsQueryable()
                where (employee.FirstName + " " + employee.LastName) == "Test User"
                select employee);

            var actual = ((IQueryable)(
                from employee in employees.AsQueryable()
                where employee.FullName == "Test User"
                select employee)).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void ConcatNonStringInlineProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User", From = 0, To = 100 } };

            var expected1 = (from employee in employees.AsQueryable()
                             where (employee.From + "-" + employee.To) == "0-100"
                             select employee);

            var expected2 = (from employee in employees.AsQueryable()
                             where (employee.From.ToString() + "-" + employee.To.ToString()) == "0-100"
                             select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FromTo == "0-100"
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected1.Expression, expected2.Expression);
        }

        [Test]
        public void InlinePropertyOrderBy()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            orderby (employee.FirstName + " " + employee.LastName)
                            select employee);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.FullName == "Test User"
                          orderby employee.FullName
                          select employee);

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlinePropertyOrderByThenBy()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            orderby (employee.FirstName + " " + employee.LastName)
                            select employee).ThenBy(x => true);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.FullName == "Test User"
                          orderby employee.FullName
                          select employee).ThenBy(x => x.IsActive);

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineBooleanProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where true
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.IsActive
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void TestLdflda()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where employee.Reference.Count == 0
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.Count == 0
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineTooDeepProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.TooDeepName == "Test User"
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
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
            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineMethod()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod() == "Test User"
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineMethodWithArg()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where ("Mr " + employee.FirstName + " " + employee.LastName) == "Mr Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod("Mr ") == "Mr Test User"
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineMethodWithTwoArgs()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where ("Mr " + employee.FirstName + " " + employee.LastName + " Jr.") == "Mr Test User Jr."
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod("Mr ", " Jr.") == "Mr Test User Jr."
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test, Ignore("Minor differences")]
        public void Issue39()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.NullableDate.HasValue && (employee.NullableInt.HasValue && employee.NullableDate.Value.AddDays(employee.NullableInt.Value) > DateTime.Now))
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.Test
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void Issue58()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = employees.AsQueryable().Where(_ => (_.Id <= 3 ? (_.Id > 3 ? 3 : 2) : 1) == 1);

            var actual = employees.AsQueryable().Where(_ => _.ComplexProperty == 1).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineExtensionMethod()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullName().Computed() == "Test User"
                          select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineExtensionMethodOrderBy()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            orderby (employee.FirstName + " " + employee.LastName)
                            select employee);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.FullName().Computed() == "Test User"
                          orderby employee.FullName().Computed()
                          select employee);

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlineExtensionMethodOrderByThenBy()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            orderby (employee.FirstName + " " + employee.LastName)
                            select employee).ThenBy(x => true);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.FullName().Computed() == "Test User"
                          orderby employee.FullName().Computed()
                          select employee).ThenBy(x => x.IsActive);

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlinePropertyNullableShortColeasce1()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.MyField.HasValue ? employee.MyField.Value : (short)0) > 0
                            select employee);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.TheBad > (short)0
                          select employee);

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void InlinePropertyNullableShortColeasce2()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.MyField.HasValue ? (short)0 : (short)1) > (short)0
                            select employee);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where (employee.MyField.HasValue ? (short)0 : (short)1) > 0
                          select employee);

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test, Ignore("Minor differences")]
        public void Issue78()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = employees.AsQueryable().Select(e => e.TimesheetMode == TimePeriodMode.Duration
                ? e.DurationHours ?? new decimal(100)
                : DbFunctions.DiffMinutes(e.StartTimeOfDay, e.EndTimeOfDay) / new decimal(60) ?? new decimal(100));

            var actual = employees.AsQueryable().Select(e => e.TotalHoursDb).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }

        [Test]
        public void Issue127()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                where ((IEmployeeStatus)new Active()) is Active
                select employee);

            var actual = (from employee in employees.AsQueryable()
                where employee.Status is Active
                select employee).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);
        }
    }
}
