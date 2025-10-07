using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ListInitTests : DecompilerTestsBase
    {
        [Test]
        public void NewListOfIntWithInitializationTest()
        {
            Expression<Func<List<int>>> expression = () => new List<int> { 1 };
            Func<List<int>> compiled = () => new List<int> { 1 };
            Test(compiled, expression);
        }

        [Test]
        public void NewListOfIntWithInitialization2Test()
        {
            Expression<Func<List<int>>> expression = () => new List<int> { 1, 2 };
            Func<List<int>> compiled = () => new List<int> { 1, 2 };
            Test(compiled, expression);
        }

        [Test]
        public void NewListOfIntWithInitialization3Test()
        {
            Expression<Func<List<int>>> expression = () => new List<int> { 1, 2, 3 };
            Func<List<int>> compiled = () => new List<int> { 1, 2, 3 };
            Test(compiled, expression);
        }

        [Test]
        public void NewDictionaryWithInitializationTest()
        {
            Expression<Func<Dictionary<int, int>>> expression = () => new Dictionary<int, int> { { 1, 2 }, { 3, 4 } };
            Func<Dictionary<int, int>> compiled = () => new Dictionary<int, int> { { 1, 2 }, { 3, 4 } };
            Test(compiled, expression);
        }

        [Test]
        public void NewTestClassWithInitializationTest()
        {
            Expression<Func<TestClass>> expression = () => new TestClass { { DateTime.Now, DateTime.Now } };
            Func<TestClass> compiled = () => new TestClass { { DateTime.Now, DateTime.Now } };
            Test(compiled, expression);
        }
    }
}