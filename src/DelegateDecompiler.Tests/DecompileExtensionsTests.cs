using System;
using System.Globalization;
using System.Linq.Expressions;
using NUnit.Framework;
using Xunit;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class DecompileExtensionsTests : DecompilerTestsBase
    {
        [Test]
        public void ShouldBeAbleToDecompileExpression()
        {
            Expression<Func<object, object>> expected = o => o;
            Func<object, object> compiled = o => o;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithAdd()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x + y;
            Func<int, int, int> compiled = (x, y) => x + y;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithRem()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x % y;
            Func<int, int, int> compiled = (x, y) => x % y;
            Test(expected, compiled);
        }
        
        [Test]
        public void ShouldBeAbleToDecompileExpressionWithXor()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x ^ y;
            Func<int, int, int> compiled = (x, y) => x ^ y;
            Test(expected, compiled);
        }
        
        [Test]
        public void ShouldBeAbleToDecompileExpressionWithShl()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x << (y & 31);
            Func<int, int, int> compiled = (x, y) => x << y;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithShr()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x >> (y & 31);
            Func<int, int, int> compiled = (x, y) => x >> y;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithNeg()
        {
            Expression<Func<int, int>> expected = x => -x;
            Func<int, int> compiled = x => -x;
            Test(expected, compiled);
        }

        [Test, Ignore("Code is optimized")]
        public void ShouldBeAbleToDecompileExpressionWithNot()
        {
            Expression<Func<bool, bool>> expected = x => !x;
            Func<bool, bool> compiled = x => !x;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithCastClass()
        {
            Expression<Func<object, DecompileExtensionsTests>> expected = x => (DecompileExtensionsTests) x;
            Func<object, DecompileExtensionsTests> compiled = x => (DecompileExtensionsTests)x;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithLdLen()
        {
            Expression<Func<int[], int>> expected = x => (int) x.Length;
            Func<int[], int> compiled = x => (int) x.Length;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithLdElemI4()
        {
            Expression<Func<int[], int>> expected = x => x[0];
            Func<int[], int> compiled = x => x[0];
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithLdElemU1()
        {
            Expression<Func<byte[], int>> expected = x => x[0];
            Func<byte[], int> compiled = x => x[0];
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithLdElemRef()
        {
            Expression<Func<object[], object>> expected = x => x[0];
            Func<object[], object> compiled = x => x[0];
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithAddConstant()
        {
            Expression<Func<int, int>> expected = x => x + 1;
            Func<int, int> compiled = x => x + 1;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithAddConstant127()
        {
            Expression<Func<int, int>> expected = x => x + 127;
            Func<int, int> compiled = x => x + 127;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithAddConstant128()
        {
            Expression<Func<int, int>> expected = x => x + 128;
            Func<int, int> compiled = x => x + 128;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithAddConstant65535()
        {
            Expression<Func<int, int>> expected = x => x + 65535;
            Func<int, int> compiled = x => x + 65535;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithSubstract()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x - y;
            Func<int, int, int> compiled = (x, y) => x - y;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithMul()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x * y;
            Func<int, int, int> compiled = (x, y) => x * y;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithDiv()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x / y;
            Func<int, int, int> compiled = (x, y) => x / y;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithBitwiseAnd()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x & y;
            Func<int, int, int> compiled = (x, y) => x & y;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithBitwiseOr()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x | y;
            Func<int, int, int> compiled = (x, y) => x | y;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithBoxing()
        {
            Expression<Func<int, object>> expected = x => x;
            Func<int, object> compiled = x => x;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithMethodCall()
        {
            Expression<Func<int, int, int>> expected = (x, y) => Add(x, y);
            Func<int, int, int> compiled = (x, y) => Add(x, y);
            Test(expected, compiled);
        }
        
        [Test]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCall()
        {
            Expression<Func<int, string>> expected = x => x.ToString();
            Func<int, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCallWithArguments()
        {
            Expression<Func<int, CultureInfo, string>> expected = (x, culture) => x.ToString(culture);
            Func<int, CultureInfo, string> compiled = (x, culture) => x.ToString(culture);
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCallWithArguments2()
        {
            Expression<Func<int, string>> expected = x => x.ToString(CultureInfo.InvariantCulture);
            Func<int, string> compiled = x => x.ToString(CultureInfo.InvariantCulture);
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithInstancePropertyCall()
        {
            Expression<Func<int?, bool>> expected = x => x.HasValue;
            Func<int?, bool> compiled = x => x.HasValue;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithCastIntToLong()
        {
            Expression<Func<int, long>> expected = x => x;
            Func<int, long> compiled = x => x;
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileExpressionWithCastIntToSbyte()
        {
            Expression<Func<sbyte, int>> expected = x => x;
            Func<sbyte, int> compiled = x => x;
            Test(expected, compiled);
        }

        [Test]
        public void DateGeDateTimeNow()
        {
            Expression<Func<TestClass, bool>> expected = x => x.StartDate >= DateTime.Now;
            Func<TestClass, bool> compiled = x => x.StartDate >= DateTime.Now;
            Test(expected, compiled);
        }

        [Test]
        public void DateLeDateTimeNow()
        {
            Expression<Func<TestClass, bool>> expected = x => x.EndDate.GetValueOrDefault() <= DateTime.Now;
            Func<TestClass, bool> compiled = x => x.EndDate.GetValueOrDefault() <= DateTime.Now;
            Test(expected, compiled);
        }

        [Test]
        public void NewObject()
        {
            Expression<Func<TestClass>> expected = () => new TestClass();
            Func<TestClass> compiled = () => new TestClass();
            Test(expected, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileStringConcat2()
        {
            Expression<Func<string, string, string>> expression = (x, y) => x + y;
            Func<string, string, string> compiled = (x, y) => x + y;
            Test(expression, compiled);
        }

        [Test]
        public void ShouldBeAbleToDecompileStringConcat3()
        {
            Expression<Func<string, string, string, string>> expression = (x, y, z) => x + y + z;
            Func<string, string, string, string> compiled = (x, y, z) => x + y + z;
            Test(expression, compiled);
        }

        public static int Add(int x, int y)
        {
            return x + y;
        }
    }
}
