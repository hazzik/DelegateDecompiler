using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class DecimalNullTests : DecompilerTestsBase
    {
        [Test]
        public void ExpressionWithNullable()
        {
            Expression<Func<decimal, decimal?>> expected = x => (decimal?)x;
            Func<decimal, decimal?> compiled = x => (decimal?)x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableEqual()
        {
            Expression<Func<decimal?, decimal?, bool>> expected = (x, y) => x == y;
            Func<decimal?, decimal?, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableNotEqual()
        {
            Expression<Func<decimal?, decimal?, bool>> expected = (x, y) => x != y;
            Func<decimal?, decimal?, bool> compiled = (x, y) => x != y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThan()
        {
            Expression<Func<decimal?, decimal?, bool>> expected = (x, y) => x > y;
            Func<decimal?, decimal?, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThanOrEqual()
        {
            Expression<Func<decimal?, decimal?, bool>> expected = (x, y) => x >= y;
            Func<decimal?, decimal?, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThan()
        {
            Expression<Func<decimal?, decimal?, bool>> expected = (x, y) => x < y;
            Func<decimal?, decimal?, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThanOrEqual()
        {
            Expression<Func<decimal?, decimal?, bool>> expected = (x, y) => x <= y;
            Func<decimal?, decimal?, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableMul()
        {
            Expression<Func<decimal?, decimal?, decimal?>> expected = (x, y) => x * y;
            Func<decimal?, decimal?, decimal?> compiled = (x, y) => x * y;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullablePlus()
        {
            Expression<Func<decimal?, decimal?, decimal?>> expected = (x, y) => x + y;
            Func<decimal?, decimal?, decimal?> compiled = (x, y) => x + y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableDiv()
        {
            Expression<Func<decimal?, decimal?, decimal?>> expected = (x, y) => x / y;
            Func<decimal?, decimal?, decimal?> compiled = (x, y) => x / y;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullableEqual2()
        {
            Expression<Func<decimal?, decimal, bool>> expected = (x, y) => x == y;
            Func<decimal?, decimal, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableNotEqual2()
        {
            Expression<Func<decimal?, decimal, bool>> expected = (x, y) => x != y;
            Func<decimal?, decimal, bool> compiled = (x, y) => x != y;

            Func<decimal?, decimal, bool> c = (x, y) => (x.GetValueOrDefault() == y) ? (x.HasValue == false) : true;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThan2()
        {
            Expression<Func<decimal?, decimal, bool>> expected = (x, y) => x > y;
            Func<decimal?, decimal, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThanOrEqual2()
        {
            Expression<Func<decimal?, decimal, bool>> expected = (x, y) => x >= y;
            Func<decimal?, decimal, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThan2()
        {
            Expression<Func<decimal?, decimal, bool>> expected = (x, y) => x < y;
            Func<decimal?, decimal, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThanOrEqual2()
        {
            Expression<Func<decimal?, decimal, bool>> expected = (x, y) => x <= y;
            Func<decimal?, decimal, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableMul2()
        {
            Expression<Func<decimal?, decimal, decimal?>> expected = (x, y) => x * y;
            Func<decimal?, decimal, decimal?> compiled = (x, y) => x * y;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullablePlus2()
        {
            Expression<Func<decimal?, decimal, decimal?>> expected = (x, y) => x + y;
            Func<decimal?, decimal, decimal?> compiled = (x, y) => x + y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableDiv2()
        {
            Expression<Func<decimal?, decimal, decimal?>> expected = (x, y) => x / y;
            Func<decimal?, decimal, decimal?> compiled = (x, y) => x / y;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullableEqual3()
        {
            Expression<Func<decimal, decimal?, bool>> expected = (x, y) => x == y;
            Func<decimal, decimal?, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableNotEqual3()
        {
            Expression<Func<decimal, decimal?, bool>> expected = (x, y) => x != y;
            Func<decimal, decimal?, bool> compiled = (x, y) => x != y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThan3()
        {
            Expression<Func<decimal, decimal?, bool>> expected = (x, y) => x > y;
            Func<decimal, decimal?, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThanOrEqual3()
        {
            Expression<Func<decimal, decimal?, bool>> expected = (x, y) => x >= y;
            Func<decimal, decimal?, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThan3()
        {
            Expression<Func<decimal, decimal?, bool>> expected = (x, y) => x < y;
            Func<decimal, decimal?, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThanOrEqual3()
        {
            Expression<Func<decimal, decimal?, bool>> expected = (x, y) => x <= y;
            Func<decimal, decimal?, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableMul3()
        {
            Expression<Func<decimal, decimal?, decimal?>> expected = (x, y) => x * y;
            Func<decimal, decimal?, decimal?> compiled = (x, y) => x * y;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullablePlus3()
        {
            Expression<Func<decimal, decimal?, decimal?>> expected = (x, y) => x + y;
            Func<decimal, decimal?, decimal?> compiled = (x, y) => x + y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableDiv3()
        {
            Expression<Func<decimal, decimal?, decimal?>> expected = (x, y) => x / y;
            Func<decimal, decimal?, decimal?> compiled = (x, y) => x / y;
            Test(expected, compiled);
        }
        
        [Test, Ignore("Needs optimization")]
        public void ExpressionWithNullableSum3()
        {
            Expression<Func<decimal?, decimal?, decimal?, decimal?>> expected = (x, y, z) => x + y + z;
            Func<decimal?, decimal?, decimal?, decimal?> compiled = (x, y, z) => x + y + z;
            Test(expected, compiled);
        }
    }
}