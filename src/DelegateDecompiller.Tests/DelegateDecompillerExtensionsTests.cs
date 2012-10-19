using System;
using System.Globalization;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiller.Tests
{
    public class DelegateDecompillerExtensionsTests
    {
        [Fact]
        public void ShouldBeAbleToDecompileExpression()
        {
            Expression<Func<object, object>> expression = o => o;

            var compiled = GetType().GetMethod("Id");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithSum()
        {
            Expression<Func<int, int, int>> expression = (x, y) => x + y;

            var compiled = GetType().GetMethod("Sum");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithSubstract()
        {
            Expression<Func<int, int, int>> expression = (x, y) => x - y;

            var compiled = GetType().GetMethod("Substract");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithMul()
        {
            Expression<Func<int, int, int>> expression = (x, y) => x * y;

            var compiled = GetType().GetMethod("Multiply");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithDiv()
        {
            Expression<Func<int, int, int>> expression = (x, y) => x / y;

            var compiled = GetType().GetMethod("Divade");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithBoxing()
        {
            Expression<Func<int, object>> expression = x => x;

            var compiled = GetType().GetMethod("Boxing");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithMethodCall()
        {
            Expression<Func<int, int, int>> expression = (x, y) => Sum(x, y);

            var compiled = GetType().GetMethod("MehtodCall");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }
        
        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCall()
        {
            Expression<Func<int, string>> expression = x => x.ToString();

            var compiled = GetType().GetMethod("ToString1");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCallWithArguments()
        {
            Expression<Func<int, CultureInfo, string>> expression = (x, culture) => x.ToString(culture);

            var compiled = GetType().GetMethod("ToString2");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        [Fact]
        public void ShouldBeAbleToDecompileExpressionWithInstanceMethodCallWithArguments2()
        {
            Expression<Func<int, string>> expression = x => x.ToString(CultureInfo.InvariantCulture);

            var compiled = GetType().GetMethod("ToString3");

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }

        public static object Id(object o)
        {
            return o;
        }

        public static int Sum(int x, int y)
        {
            return x + y;
        }

        public static int Substract(int x, int y)
        {
            return x - y;
        }

        public static int Multiply(int x, int y)
        {
            return x * y;
        }

        public static int Divade(int x, int y)
        {
            return x / y;
        }

        public static int MehtodCall(int x, int y)
        {
            return Sum(x, y);
        }

        public static string ToString1(int x)
        {
            return x.ToString();
        }

        public static string ToString2(int x, CultureInfo culture)
        {
            return x.ToString(culture);
        }

        public static string ToString3(int x)
        {
            return x.ToString(CultureInfo.InvariantCulture);
        }

        public static object Boxing(int x)
        {
            return x;
        }
    }
}
