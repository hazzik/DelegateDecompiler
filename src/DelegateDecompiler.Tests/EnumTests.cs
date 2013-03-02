using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
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

        [Fact]
        public void TestEnumParameterEqualsEnumConstant()
        {
            Expression<Func<TestEnum, bool>> expected = x => x == TestEnum.Bar;
            Func<TestEnum, bool> compiled = x => x == TestEnum.Bar;
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumConstantEqualsEnumParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => TestEnum.Bar == x;
            Func<TestEnum, bool> compiled = x => TestEnum.Bar == x;
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void TestEnumPropertyNotEqualsFooOrElseEnumPropertyEqualsBar()
        {
            // Original expected expression:
            // Expression<Func<TestEnum, bool>> expected = x => (x != TestEnum.Bar) || (x == TestEnum.Foo);
            // Expected expression before optimization:
            Expression<Func<TestEnum, bool>> expected = x => (x != TestEnum.Bar) ? true : (x == TestEnum.Foo);
            Func<TestEnum, bool> compiled = x => (x != TestEnum.Bar) || (x == TestEnum.Foo);
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumPropertyEqualsFooOrElseEnumPropertyEqualsBar()
        {
            Expression<Func<TestEnum, bool>> expected = x => (x == TestEnum.Bar) || (x == TestEnum.Foo);
            Func<TestEnum, bool> compiled = x => (x == TestEnum.Bar) || (x == TestEnum.Foo);
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumParametersEqual()
        {
            Expression<Func<TestEnum, TestEnum, bool>> expected = (x, y) => x == y;
            Func<TestEnum, TestEnum, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void TestEnumParameterNotEqualsEnumConstant()
        {
            Expression<Func<TestEnum, bool>> expected = x => x != TestEnum.Bar;
            Func<TestEnum, bool> compiled = x => x != TestEnum.Bar;
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void TestEnumConstantNotEqualsEnumParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => TestEnum.Bar != x;
            Func<TestEnum, bool> compiled = x => TestEnum.Bar != x;
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void TestEnumParametersNotEqual()
        {
            Expression<Func<TestEnum, TestEnum, bool>> expected = (x, y) => x != y;
            Func<TestEnum, TestEnum, bool> compiled = (x, y) => x != y;
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void TestEnumConstantHasFlagEnumParameter()
        {
            Expression<Func<TestFlagEnum, bool>> expected = x => TestFlagEnum.Bar.HasFlag(x);
            Func<TestFlagEnum, bool> compiled = x => TestFlagEnum.Bar.HasFlag(x);
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void TestEnumParameterHasFlagEnumConstant()
        {
            Expression<Func<TestFlagEnum, bool>> expected = x => x.HasFlag(TestFlagEnum.Bar);
            Func<TestFlagEnum, bool> compiled = x => x.HasFlag(TestFlagEnum.Bar);
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void TestEnumParameterHasFlagEnumParameter()
        {
            Expression<Func<TestFlagEnum, bool>> expected = x => x.HasFlag(TestFlagEnum.Bar);
            Func<TestFlagEnum, bool> compiled = x => x.HasFlag(TestFlagEnum.Bar);
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumParameterAsMethodParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => TestEnumMethod(x);
            Func<TestEnum, bool> compiled = x => TestEnumMethod(x);
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumParameterAsMethodWithEnumParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => EnumMethod(x);
            Func<TestEnum, bool> compiled = x => EnumMethod(x);
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void TestEnumParameterAsMethodWithObjectParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => ObjectMethod(x);
            Func<TestEnum, bool> compiled = x => ObjectMethod(x);
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumParameterAsMethodWithInt16Parameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => Int16Method((short) x);
            Func<TestEnum, bool> compiled = x => Int16Method((short) x);
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumParameterAsMethodWithInt32Parameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => Int32Method((int) x);
            Func<TestEnum, bool> compiled = x => Int32Method((int) x);
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumParameterAsMethodWithInt64Parameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => Int64Method((long) x);
            Func<TestEnum, bool> compiled = x => Int64Method((long) x);
            Test(expected, compiled);
        }

        [Fact]
        public void TestEnumParameterAsGenericMethodParameter()
        {
            Expression<Func<TestEnum, bool>> expected = x => GenericMethod(x);
            Func<TestEnum, bool> compiled = x => GenericMethod(x);
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
