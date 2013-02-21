﻿using System;
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
    }
}