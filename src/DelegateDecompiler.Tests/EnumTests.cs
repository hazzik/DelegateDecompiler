using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class EnumTests : DecompilerTestsBase
    {
        public enum TestEnum
        {
            Foo,
            Bar
        }

        [Flags]
        public enum TestFlagEnum
        {
            None = 0,
            Foo = 1,
            Bar = 2,
        }

        [Test]
        public void TestEnumParameterEqualsEnumConstant()
        {
            Expression<Func<TestEnum, bool>> expected1 = x => x == TestEnum.Bar;
            Expression<Func<TestEnum, bool>> expected2 = x => (int) x == (int) TestEnum.Bar;
            Func<TestEnum, bool> compiled = x => x == TestEnum.Bar;
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void TestEnumConstantEqualsEnumParameter()
        {
            Expression<Func<TestEnum, bool>> expected1 = x => TestEnum.Bar == x;
            Expression<Func<TestEnum, bool>> expected2 = x => (int) TestEnum.Bar == (int) x;
            Func<TestEnum, bool> compiled = x => TestEnum.Bar == x;
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void TestEnumPropertyNotEqualsFooOrElseEnumPropertyEqualsBar()
        {
            Expression<Func<TestEnum, bool>> expected1 = x => (x != TestEnum.Bar) || (x == TestEnum.Foo);
            Expression<Func<TestEnum, bool>> expected2 = x => ((int)x != (int)TestEnum.Bar) || ((int)x == (int)TestEnum.Foo);
            Func<TestEnum, bool> compiled = x => (x != TestEnum.Bar) || (x == TestEnum.Foo);
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void TestEnumPropertyEqualsFooOrElseEnumPropertyEqualsBar()
        {
            Expression<Func<TestEnum, bool>> expected1 = x => (x == TestEnum.Bar) || (x == TestEnum.Foo);
            Expression<Func<TestEnum, bool>> expected2 = x => ((int)x == (int)TestEnum.Bar) || ((int)x == (int)TestEnum.Foo);
            Func<TestEnum, bool> compiled = x => (x == TestEnum.Bar) || (x == TestEnum.Foo);
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void TestEnumParametersEqual()
        {
            Expression<Func<TestEnum, TestEnum, bool>> expected = (x, y) => x == y;
            Func<TestEnum, TestEnum, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterNotEqualsEnumConstant()
        {
            Expression<Func<TestEnum, bool>> expected1 = x => x != TestEnum.Bar;
            Expression<Func<TestEnum, bool>> expected2 = x => (int) x != (int) TestEnum.Bar;
            Func<TestEnum, bool> compiled = x => x != TestEnum.Bar;
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void TestEnumConstantNotEqualsEnumParameter()
        {
            Expression<Func<TestEnum, bool>> expected1 = x => TestEnum.Bar != x;
            Expression<Func<TestEnum, bool>> expected2 = x => (int) TestEnum.Bar != (int) x;
            Func<TestEnum, bool> compiled = x => TestEnum.Bar != x;
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void TestEnumParametersNotEqual()
        {
            Expression<Func<TestEnum, TestEnum, bool>> expected = (x, y) => x != y;
            Func<TestEnum, TestEnum, bool> compiled = (x, y) => x != y;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumConstantHasFlagEnumParameter()
        {
            Expression<Func<TestFlagEnum, bool>> expected = x => TestFlagEnum.Bar.HasFlag(x);
            Func<TestFlagEnum, bool> compiled = x => TestFlagEnum.Bar.HasFlag(x);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterHasFlagEnumConstant()
        {
            Expression<Func<TestFlagEnum, bool>> expected = x => x.HasFlag(TestFlagEnum.Bar);
            Func<TestFlagEnum, bool> compiled = x => x.HasFlag(TestFlagEnum.Bar);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterHasFlagEnumParameter()
        {
            Expression<Func<TestFlagEnum, bool>> expected = x => x.HasFlag(x);
            Func<TestFlagEnum, bool> compiled = x => x.HasFlag(x);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumConstantAndEnumParameter()
        {
            Expression<Func<TestFlagEnum, TestFlagEnum>> expected = x => TestFlagEnum.Bar & x;
            Func<TestFlagEnum, TestFlagEnum> compiled = x => TestFlagEnum.Bar & x;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAndEnumConstant()
        {
            Expression<Func<TestFlagEnum, TestFlagEnum>> expected = x => x & TestFlagEnum.Bar ;
            Func<TestFlagEnum, TestFlagEnum> compiled = x => x & TestFlagEnum.Bar ;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAndEnumParameter()
        {
            Expression<Func<TestFlagEnum, TestFlagEnum>> expected = x => x & x;
            Func<TestFlagEnum, TestFlagEnum> compiled = x => x & x ;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumConstantOrEnumParameter()
        {
            Expression<Func<TestFlagEnum, TestFlagEnum>> expected = x => TestFlagEnum.Bar | x;
            Func<TestFlagEnum, TestFlagEnum> compiled = x => TestFlagEnum.Bar | x;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterOrEnumConstant()
        {
            Expression<Func<TestFlagEnum, TestFlagEnum>> expected = x => x | TestFlagEnum.Bar ;
            Func<TestFlagEnum, TestFlagEnum> compiled = x => x | TestFlagEnum.Bar ;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterOrEnumParameter()
        {
            Expression<Func<TestFlagEnum, TestFlagEnum>> expected = x => x | x;
            Func<TestFlagEnum, TestFlagEnum> compiled = x => x | x ;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAsMethodParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => TestEnumMethod(x);
            Func<TestEnum, bool> compiled = x => TestEnumMethod(x);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAsMethodWithEnumParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => EnumMethod(x);
            Func<TestEnum, bool> compiled = x => EnumMethod(x);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAsMethodWithObjectParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => ObjectMethod(x);
            Func<TestEnum, bool> compiled = x => ObjectMethod(x);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAsMethodWithInt16Parameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => Int16Method((short) x);
            Func<TestEnum, bool> compiled = x => Int16Method((short) x);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAsMethodWithInt32Parameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => Int32Method((int) x);
            Func<TestEnum, bool> compiled = x => Int32Method((int) x);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAsMethodWithInt64Parameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => Int64Method((long) x);
            Func<TestEnum, bool> compiled = x => Int64Method((long) x);
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumParameterAsGenericMethodParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => GenericMethod(x);
            Func<TestEnum, bool> compiled = x => GenericMethod(x);
            Test(expected, compiled);
        }

        // The following tests check for the insertion of Expression.Convert in the expression tree for compatible types

        [Test]
        public void TestEnumCastSubtraction()
        {
            Expression<Func<TestEnum, int>> expected = x => (int)x - 10;
            Func<TestEnum, int> compiled = x => (int)x - 10;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumCastMod()
        {
            Expression<Func<TestEnum, int>> expected = x => (int)x % 10;
            Func<TestEnum, int> compiled = x => (int)x % 10;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumCastEquals()
        {
            Expression<Func<TestEnum, bool>> expected = x => (int)x == 10;
            Func<TestEnum, bool> compiled = x => (int)x == 10;
            Test(expected, compiled);
        }

        [Test]
        public void TestEnumCastGreaterThan()
        {
            Expression<Func<TestEnum, bool>> expected = x => (int)x > 10;
            Func<TestEnum, bool> compiled = x => (int)x > 10;
            Test(expected, compiled);
        }

        [Test]
        public void Issue61()
        {
            Expression<Func<decimal, decimal>> expected = x => Decimal.Round(x, 3, MidpointRounding.AwayFromZero);
            Func<decimal, decimal> compiled = x => Decimal.Round(x, 3, MidpointRounding.AwayFromZero);
            Test(expected, compiled);
        }

        [Test]
        public void Issue98A()
        {
	        Expression<Func<TestEnum?, TestEnum, bool>> expected = (x, y) => x == y;
	        Func<TestEnum?, TestEnum, bool> compiled = (x, y) => x == y;
			Test(expected, compiled);
        }

        [Test]
        public void Issue98B()
        {
	        Expression<Func<TestEnum?, bool>> expected = x => x == TestEnum.Foo;
	        Func<TestEnum?, bool> compiled = x => x == TestEnum.Foo;
			Test(expected, compiled);
        }

        private static bool TestEnumMethod(TestEnum p0)
        {
            throw new NotImplementedException();
        }

        private static bool EnumMethod(Enum p0)
        {
            throw new NotImplementedException();
        }

        private static bool ObjectMethod(object p0)
        {
            throw new NotImplementedException();
        }

        private static bool Int16Method(short p0)
        {
            throw new NotImplementedException();
        }

        private static bool Int32Method(int p0)
        {
            throw new NotImplementedException();
        }

        private static bool Int64Method(long p0)
        {
            throw new NotImplementedException();
        }

        private static bool GenericMethod<T>(T x)
        {
            throw new NotImplementedException();
        }
    }
}
