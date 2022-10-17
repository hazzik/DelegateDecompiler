using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ConditionalTests : DecompilerTestsBase
    {
        [Test]
        public void SimpleAndAlso()
        {
            Expression<Func<Employee, bool>> expected = e => e.FirstName != null && e.FirstName.Contains("Test");
            Func<Employee, bool> compiled = e => e.FirstName != null && e.FirstName.Contains("Test");
            Test(expected, compiled);
        }

        [Test]
        public void SimpleIif()
        {
            Expression<Func<Employee, string>> expected = e => e.FirstName != null ? e.FirstName : e.LastName;
            Func<Employee, string> compiled = e => e.FirstName != null ? e.FirstName : e.LastName;
            Test(expected, compiled);
        }

        [Test]
        public void SimpleIif2()
        {
            Expression<Func<Employee, string>> expected = e => e.FirstName == null ? e.LastName : e.FirstName;
            Func<Employee, string> compiled = e => e.FirstName == null ? e.LastName : e.FirstName;
            Test(expected, compiled);
        }

        [Test]
        public void SimpleCoalesce2()
        {
            Expression<Func<Employee, string>> expected = e => e.FirstName ?? e.LastName;
            Func<Employee, string> compiled = e => e.FirstName ?? e.LastName;
            Test(expected, compiled);
        }

        [Test]
        public void SimpleOrElse()
        {
            Expression<Func<Employee, bool>> expected = e => e.FirstName == null || e.FirstName.Contains("Test");
            Func<Employee, bool> compiled = e => e.FirstName == null || e.FirstName.Contains("Test");
            Test(expected, compiled);
        }

        [Test]
        public void SimpleCoalesce()
        {
            Expression<Func<Employee, bool>> expected = e => (e.FirstName ?? string.Empty).Contains("Test");
            Func<Employee, bool> compiled = e => (e.FirstName ?? string.Empty).Contains("Test");
            Test(expected, compiled);
        }

        [Test]
        public void AndLessThanOrEqual()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From <= p && p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From <= p && p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void AndLessThan()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From < p && p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From < p && p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void AndGreaterThanOrEqual()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From >= p && p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From >= p && p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void AndGreaterThan()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From > p && p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From > p && p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void AndEqual()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From == p && p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From == p && p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void AndNotEqual()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From != p && p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From != p && p < e.To;
            Test(expected, compiled);
        }


        [Test]
        public void OrLessThanOrEqual()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From <= p || p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From <= p || p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void OrLessThan()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From < p || p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From < p || p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void OrGreaterThanOrEqual()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From >= p || p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From >= p || p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void OrGreaterThan()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From > p || p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From > p || p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void OrEqual()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From == p || p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From == p || p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void OrNotEqual()
        {
            Expression<Func<Employee, int, bool>> expected = (e, p) => e.From != p || p < e.To;
            Func<Employee, int, bool> compiled = (e, p) => e.From != p || p < e.To;
            Test(expected, compiled);
        }

        [Test]
        public void NullableCoalesce()
        {
            Expression<Func<int?, bool>> expected = e => (e ?? 100) == 100;
            Func<int?, bool> compiled = e => (e ?? 100) == 100;
            Test(expected, compiled);
        }

        [Test]
        public void TwoIifs()
        {
            Expression<Func<Employee, bool>> expected = e => e.FirstName != null && (e.FirstName.Contains("Test") && !e.IsBlocked);
            Func<Employee, bool> compiled = e => e.FirstName != null && e.FirstName.Contains("Test") && !e.IsBlocked;
            Test(expected, compiled);
        }

        [Test, Ignore("Need optimization for Boolean")]
        public void TwoIifs2()
        {
            Expression<Func<Employee, string, bool>> expected = (u, term) => (u.FirstName != null && (u.FirstName.Contains(term) || term.Contains(u.FirstName))) && !u.IsBlocked;
            Func<Employee, string, bool> compiled = (u, term) => (u.FirstName != null && (u.FirstName.Contains(term) || term.Contains(u.FirstName))) && !u.IsBlocked;
            Test(expected, compiled);
        }

        [Test]
        public void SimpleIfStatement()
        {
            Expression<Func<int, int, int>> expected = (a, b) => a >= b ? b : a;
            Func<int, int, int> compiled = (a, b) =>
            {
                var result = b;
                if (a < b)
                    result = a;
                return result;
            };
            Test(expected, compiled);
        }

        [Test]
        public void SimpleIfElseStatement()
        {
            Expression<Func<int, int, int>> expected = (a, b) => a >= b ? b : a;
            Func<int, int, int> compiled = (a, b) =>
            {
                if (a < b)
                    return a;
                else
                    return b;
            };
            Test(expected, compiled);
        }

        [Test]
        public void Issue58()
        {
            Expression<Func<int, bool>> expected = x => (x <= 3 ? (x <= 3 ? 2 : 3) : 1) == 1;
            Func<int, bool> compiled = x => (x <= 3 ? (x <= 3 ? 2 : 3) : 1) == 1;
            Test(expected, compiled);
        }

        [Test]
        public void IfElseStatementWithTwoLocalVariables()
        {
            Expression<Func<int, int, int>> expected = (a, b) => a + b + (a >= b ? 2 : 1) + 10;
            Func<int, int, int> compiled = (a, b) =>
            {
                var c = 0;
                var d = 10;
                if (a < b)
                {
                    c = 1;
                }
                else
                {
                    c = 2;
                }
                return a + b + c + d;
            };
            Test(expected, compiled);
        }
    }
}
