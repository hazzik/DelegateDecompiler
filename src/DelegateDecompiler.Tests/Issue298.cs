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
        static int Actual<T>(T value) where T : ITestInterface => ((ITestInterface)value).Field;
        
        Test(Actual<TestClass>,
            value => value.Field,
            value => ((ITestInterface)value).Field
        );
    }

    [Test]
    public void TestConstrained()
    {
        static int Actual<T>(T value) where T : ITestInterface => value.Field;

        Test(Actual<ITestInterface>, value => value.Field);
        Test(Actual<TestClass>, value => value.Field);
    }

#if NET6_0_OR_GREATER
    [Test]
    public void TestConstrainedDefaultImplementation()
    {
        static int Actual<T>(T value, int i) where T : ITestInterface => value.Method(i);
        static Expression<Func<T, int, int>> Expected<T>() where T : ITestInterface => (value, i) => value.Method(i);

        Test(Actual, Expected<ITestInterface>());
        Test(Actual, Expected<TestClass>());
    }
#endif

    interface ITestInterface
    {
        public int Field { get; }

#if NET6_0_OR_GREATER
        public int Method(int i) => i;
#endif
    }

    class TestClass : ITestInterface
    {
        public int Field { get; set; }
    }
}
