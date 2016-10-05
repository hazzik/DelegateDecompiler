using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class DbFunctions
    {
        public static DateTime? AddYears(DateTime? dateValue, int? addValue)
        {
            throw new NotImplementedException();
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
    }
}