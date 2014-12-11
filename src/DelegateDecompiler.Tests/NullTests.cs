using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class NullTests : DecompilerTestsBase
    {
        [Test]
        public void ExpressionWithNull()
        {
            Expression<Func<string>> expected = () => null;
            Func<string> compiled = () => null;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionStringEqualsNull()
        {
            Expression<Func<string, bool>> expected = x => x.Equals(null);
            Func<string, bool> compiled = x => x.Equals(null);
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullable()
        {
            Expression<Func<int, int?>> expected = x => (int?)x;
            Func<int, int?> compiled = x => (int?)x;
            Test(expected, compiled);
        }
        [Test]
        public void ExpressionWithNullableCoalesce()
        {
            Expression<Func<int?, int>> expected = x => x ?? 0;
            Func<int?, int> compiled = x => x ?? 0;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableEqual()
        {
            Expression<Func<int?, int?, bool>> expected = (x, y) => x == y;
            Func<int?, int?, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableNotEqual()
        {
            Expression<Func<int?, int?, bool>> expected = (x, y) => x != y;
            Func<int?, int?, bool> compiled = (x, y) => x != y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThan()
        {
            Expression<Func<int?, int?, bool>> expected = (x, y) => x > y;
            Func<int?, int?, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThanOrEqual()
        {
            Expression<Func<int?, int?, bool>> expected = (x, y) => x >= y;
            Func<int?, int?, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThan()
        {
            Expression<Func<int?, int?, bool>> expected = (x, y) => x < y;
            Func<int?, int?, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThanOrEqual()
        {
            Expression<Func<int?, int?, bool>> expected = (x, y) => x <= y;
            Func<int?, int?, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableXor()
        {
            Expression<Func<int?, int?, int?>> expected = (x, y) => x ^ y;
            Func<int?, int?, int?> compiled = (x, y) => y ^ x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableAnd()
        {
            Expression<Func<int?, int?, int?>> expected = (x, y) => x & y;
            Func<int?, int?, int?> compiled = (x, y) => y & x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableOr()
        {
            Expression<Func<int?, int?, int?>> expected = (x, y) => x | y;
            Func<int?, int?, int?> compiled = (x, y) => y | x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableMul()
        {
            Expression<Func<int?, int?, int?>> expected = (x, y) => x * y;
            Func<int?, int?, int?> compiled = (x, y) => y * x;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullablePlus()
        {
            Expression<Func<int?, int?, int?>> expected = (x, y) => x + y;
            Func<int?, int?, int?> compiled = (x, y) => y + x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableDiv()
        {
            Expression<Func<int?, int?, int?>> expected = (x, y) => x / y;
            Func<int?, int?, int?> compiled = (x, y) => y / x;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullableEqual2()
        {
            Expression<Func<int?, int, bool>> expected = (x, y) => x == y;
            Func<int?, int, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableNotEqual2()
        {
            Expression<Func<int?, int, bool>> expected = (x, y) => x != y;
            Func<int?, int, bool> compiled = (x, y) => x != y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThan2()
        {
            Expression<Func<int?, int, bool>> expected = (x, y) => x > y;
            Func<int?, int, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThanOrEqual2()
        {
            Expression<Func<int?, int, bool>> expected = (x, y) => x >= y;
            Func<int?, int, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThan2()
        {
            Expression<Func<int?, int, bool>> expected = (x, y) => x < y;
            Func<int?, int, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThanOrEqual2()
        {
            Expression<Func<int?, int, bool>> expected = (x, y) => x <= y;
            Func<int?, int, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableXor2()
        {
            Expression<Func<int?, int, int?>> expected = (x, y) => x ^ y;
            Func<int?, int, int?> compiled = (x, y) => y ^ x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableAnd2()
        {
            Expression<Func<int?, int, int?>> expected = (x, y) => x & y;
            Func<int?, int, int?> compiled = (x, y) => y & x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableOr2()
        {
            Expression<Func<int?, int, int?>> expected = (x, y) => x | y;
            Func<int?, int, int?> compiled = (x, y) => y | x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableMul2()
        {
            Expression<Func<int?, int, int?>> expected = (x, y) => x * y;
            Func<int?, int, int?> compiled = (x, y) => y * x;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullablePlus2()
        {
            Expression<Func<int?, int, int?>> expected = (x, y) => x + y;
            Func<int?, int, int?> compiled = (x, y) => y + x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableDiv2()
        {
            Expression<Func<int?, int, int?>> expected = (x, y) => x / y;
            Func<int?, int, int?> compiled = (x, y) => y / x;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullableEqual3()
        {
            Expression<Func<int, int?, bool>> expected = (x, y) => x == y;
            Func<int, int?, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableNotEqual3()
        {
            Expression<Func<int, int?, bool>> expected = (x, y) => x != y;
            Func<int, int?, bool> compiled = (x, y) => x != y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThan3()
        {
            Expression<Func<int, int?, bool>> expected = (x, y) => x > y;
            Func<int, int?, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableGreaterThanOrEqual3()
        {
            Expression<Func<int, int?, bool>> expected = (x, y) => x >= y;
            Func<int, int?, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThan3()
        {
            Expression<Func<int, int?, bool>> expected = (x, y) => x < y;
            Func<int, int?, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableLessThanOrEqual3()
        {
            Expression<Func<int, int?, bool>> expected = (x, y) => x <= y;
            Func<int, int?, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableXor3()
        {
            Expression<Func<int, int?, int?>> expected = (x, y) => x ^ y;
            Func<int, int?, int?> compiled = (x, y) => y ^ x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableAnd3()
        {
            Expression<Func<int, int?, int?>> expected = (x, y) => x & y;
            Func<int, int?, int?> compiled = (x, y) => y & x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableOr3()
        {
            Expression<Func<int, int?, int?>> expected = (x, y) => x | y;
            Func<int, int?, int?> compiled = (x, y) => y | x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableMul3()
        {
            Expression<Func<int, int?, int?>> expected = (x, y) => x * y;
            Func<int, int?, int?> compiled = (x, y) => y * x;
            Test(expected, compiled);
        }
        
        [Test]
        public void ExpressionWithNullablePlus3()
        {
            Expression<Func<int, int?, int?>> expected = (x, y) => x + y;
            Func<int, int?, int?> compiled = (x, y) => y + x;
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithNullableDiv3()
        {
            Expression<Func<int, int?, int?>> expected = (x, y) => x / y;
            Func<int, int?, int?> compiled = (x, y) => y / x;
            Test(expected, compiled);
        }
        
        [Test, Ignore("Needs optimization")]
        public void ExpressionWithNullableSum3()
        {
            Expression<Func<int?, int?, int?, int?>> expected = (x, y, z) => x + y + z;
            Func<int?, int?, int?, int?> compiled = (x, y, z) => x + y + z;
            Test(expected, compiled);
        }
    }
}