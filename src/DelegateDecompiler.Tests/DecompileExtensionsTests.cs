using System;
using System.Globalization;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class DecompileExtensionsTests : DecompilerTestsBase
    {
        [Fact]
        public void ShouldBeAbleToDecompileExpression()
        {
            Expression<Func<object, object>> expected = o => o;
            Func<object, object> compiled = o => o;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithAdd()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x + y;
            Func<int, int, int> compiled = (x, y) => x + y;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithRem()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x % y;
            Func<int, int, int> compiled = (x, y) => x % y;
            Test(expected, compiled);
        }
        
        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithXor()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x ^ y;
            Func<int, int, int> compiled = (x, y) => x ^ y;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithAddConstant()
        {
            Expression<Func<int, int>> expected = x => x + 1;
            Func<int, int> compiled = x => x + 1;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithAddConstant127()
        {
            Expression<Func<int, int>> expected = x => x + 127;
            Func<int, int> compiled = x => x + 127;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithAddConstant128()
        {
            Expression<Func<int, int>> expected = x => x + 128;
            Func<int, int> compiled = x => x + 128;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithAddConstant65535()
        {
            Expression<Func<int, int>> expected = x => x + 65535;
            Func<int, int> compiled = x => x + 65535;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithSubstract()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x - y;
            Func<int, int, int> compiled = (x, y) => x - y;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithMul()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x * y;
            Func<int, int, int> compiled = (x, y) => x * y;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithDiv()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x / y;
            Func<int, int, int> compiled = (x, y) => x / y;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithBitwiseAnd()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x & y;
            Func<int, int, int> compiled = (x, y) => x & y;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithBitwiseOr()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x | y;
            Func<int, int, int> compiled = (x, y) => x | y;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithBoxing()
        {
            Expression<Func<int, object>> expected = x => x;
            Func<int, object> compiled = x => x;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithMethodCall()
        {
            Expression<Func<int, int, int>> expected = (x, y) => Add(x, y);
            Func<int, int, int> compiled = (x, y) => Add(x, y);
            Test(expected, compiled);
        }
        
        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCall()
        {
            Expression<Func<int, string>> expected = x => x.ToString();
            Func<int, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCallWithArguments()
        {
            Expression<Func<int, CultureInfo, string>> expected = (x, culture) => x.ToString(culture);
            Func<int, CultureInfo, string> compiled = (x, culture) => x.ToString(culture);
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCallWithArguments2()
        {
            Expression<Func<int, string>> expected = x => x.ToString(CultureInfo.InvariantCulture);
            Func<int, string> compiled = x => x.ToString(CultureInfo.InvariantCulture);
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithInstancePropertyCall()
        {
            Expression<Func<int?, bool>> expected = x => x.HasValue;
            Func<int?, bool> compiled = x => x.HasValue;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithCastIntToLong()
        {
            Expression<Func<int, long>> expected = x => x;
            Func<int, long> compiled = x => x;
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithCastIntToSbyte()
        {
            Expression<Func<sbyte, int>> expected = x => x;
            Func<sbyte, int> compiled = x => x;
            Test(expected, compiled);
        }

        [Fact]
        public void DateGeDateTimeNow()
        {
            Expression<Func<TestClass, bool>> expected = x => x.StartDate >= DateTime.Now;
            Func<TestClass, bool> compiled = x => x.StartDate >= DateTime.Now;
            Test(expected, compiled);
        }

        [Fact]
        public void DateLeDateTimeNow()
        {
            Expression<Func<TestClass, bool>> expected = x => x.EndDate.GetValueOrDefault() <= DateTime.Now;
            Func<TestClass, bool> compiled = x => x.EndDate.GetValueOrDefault() <= DateTime.Now;
            Test(expected, compiled);
        }

        [Fact]
        public void NewObject()
        {
            Expression<Func<TestClass>> expected = () => new TestClass();
            Func<TestClass> compiled = () => new TestClass();
            Test(expected, compiled);
        }

        [Fact]
        public void ShouldBeAbleToDecompileStringConcat2()
        {
            Expression<Func<string, string, string>> expression = (x, y) => x + y;
            Func<string, string, string> compiled = (x, y) => x + y;
            Test(expression, compiled);
        }

        [Fact]
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
