using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public static class DbFunctions
    {
        public static DateTime? AddYears(DateTime? dateValue, int? addValue)
        {
            throw new NotSupportedException();
        }

        public static int? DiffMinutes(TimeSpan? timeValue1, TimeSpan? timeValue2)
        {
            throw new NotSupportedException();
        }
    }

    public class DateTimeTests : DecompilerTestsBase
    {
        [Test]
        public void TestAddYears()
        {
            Func<DateTime, bool> actual = dt => dt < DbFunctions.AddYears(DateTime.Now, -10);
            Expression< Func<DateTime, bool>> expected = dt => dt < DbFunctions.AddYears(DateTime.Now, -10);

            Test(expected, actual);
        }

        [Test]
        public void TestAddYearsNullable()
        {
            Func<DateTime, bool?> actual = dt => dt < DbFunctions.AddYears(DateTime.Now, -10);
            Expression< Func<DateTime, bool?>> expected = dt => dt < DbFunctions.AddYears(DateTime.Now, -10);

            Test(expected, actual);
        }
    }
}
