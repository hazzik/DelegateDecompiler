using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests;

[TestFixture]
public class Issue298 : DecompilerTestsBase
{
    [Test]
    public void TestNotConstrained()
    {
        static int NotConstrained<T>(T value) where T : ITestInterface => ((ITestInterface)value).Value;

        Expression<Func<TestClass, int>> expected = value => value.Value;
        Test(NotConstrained, expected);
    }

    [Test]
    public void TestConstrained()
    {
        static int Constrained<T>(T value) where T : ITestInterface => value.Value;

        Expression<Func<TestClass, int>> expected = value => value.Value;
        Test(Constrained, expected);
    }

    interface ITestInterface
    {
        public int Value { get; }
    }

    class TestClass : ITestInterface
    {
        public int Value { get; set; }
    }
}
