using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class ListInitTests:DecompilerTestsBase
    {
        [Fact]
        public void NewListOfIntWithInitializationTest()
        {
            Expression<Func<List<int>>> expression = () => new List<int> { 1 };
            Func<List<int>> compiled = () => new List<int> { 1 };
            Test(expression, compiled);
        }

        [Fact]
        public void NewListOfIntWithInitialization2Test()
        {
            Expression<Func<List<int>>> expression = () => new List<int> { 1, 2 };
            Func<List<int>> compiled = () => new List<int> { 1, 2 };
            Test(expression, compiled);
        }

        [Fact]
        public void NewListOfIntWithInitialization3Test()
        {
            Expression<Func<List<int>>> expression = () => new List<int> { 1, 2, 3 };
            Func<List<int>> compiled = () => new List<int> { 1, 2, 3 };
            Test(expression, compiled);
        }

        [Fact]
        public void NewDictionaryWithInitializationTest()
        {
            Expression<Func<Dictionary<int, int>>> expression = () => new Dictionary<int, int> { { 1, 2 }, { 3, 4 } };
            Func<Dictionary<int, int>> compiled = () => new Dictionary<int, int> { { 1, 2 }, { 3, 4 } };
            Test(expression, compiled);
        }

        [Fact]
        public void NewTestClassWithInitializationTest()
        {
            Expression<Func<TestClass>> expression = () => new TestClass { { DateTime.Now, DateTime.Now } };
            Func<TestClass> compiled = () => new TestClass { { DateTime.Now, DateTime.Now } };
            Test(expression, compiled);
        }
    }
}