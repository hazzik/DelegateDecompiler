using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ArrayTests : DecompilerTestsBase
    {
        [Test]
        public void TestNewArray()
        {
            Expression<Func<int[]>> expected = () => new int[] { };
            Func<int[]> compiled = () => new int[] { };
            Test(compiled, expected);
        }

        [Test]
        public void TestNewArray0()
        {
            Expression<Func<int[]>> expected1 = () => new int[0];
            Expression<Func<int[]>> expected2 = () => new int[] { };
            Func<int[]> compiled = () => new int[0];
            Test(compiled, expected1, expected2);
        }

        [Test]
        public void TestNewArrayX()
        {
            Expression<Func<int, int[]>> expected = x => new int[x];
            Func<int, int[]> compiled = x => new int[x];
            Test(compiled, expected);
        }

        [Test]
        public void TestNewArray1()
        {
            Expression<Func<int[]>> expected = () => new int[1];
            Func<int[]> compiled = () => new int[1];
            Test(compiled, expected);
        }

        [Test]
        public void DecompileArrayWithBounds()
        {
            Expression<Func<int[]>> expected = () => new int[10];
            Func<int[]> compiled = () => new int[10];
            Test(compiled, expected);
        }

        [Test]
        public void DecompileArrayWithInit()
        {
            Expression<Func<int[]>> expected = () => new[] { 1 };
            Func<int[]> compiled = () => new[] { 1 };
            Test(compiled, expected);
        }

        [Test]
        public void DecompileArrayWithInit2()
        {
            Expression<Func<int[]>> expected = () => new[] { 1, 2 };
            Func<int[]> compiled = () => new[] { 1, 2 };
            Test(compiled, expected);
        }

        [Test]
        public void DecompileArrayWithInit3()
        {
            Expression<Func<int[]>> expected = () => new[] { 1, 2, 3 };
            Func<int[]> compiled = () => new[] { 1, 2, 3 };
            Test(compiled, expected);
        }

        [Test]
        public void DecompileArrayOfIntWithInitFromParam()
        {
            Expression<Func<int, int[]>> expected = x => new[] { x, 2 };
            Func<int, int[]> compiled = x => new[] { x, 2 };
            Test(compiled, expected);
        }

        [Test]
        public void DecompileArrayOfIntWithInitFromParam4()
        {
            Expression<Func<int, int[]>> expected = x => new[] { x, 2, 3, 4 };
            Func<int, int[]> compiled = x => new[] { x, 2, 3, 4 };
            Test(compiled, expected);
        }

        [Test]
        public void DecompileArrayOfIntWithInitFromParam4A()
        {
            Expression<Func<int, int[]>> expected = x => new[] { 1, 2, 3, 4, x };
            Func<int, int[]> compiled = x => new[] { 1, 2, 3, 4, x };
            Test(compiled, expected);
        }

        [Test]
        public void DecompileArrayOfClassWithInitFromParam()
        {
            Expression<Func<TestClass, TestClass[]>> expected = x => new[] { new TestClass(), x };
            Func<TestClass, TestClass[]> compiled = x => new[] { new TestClass(), x };
            Test(compiled, expected);
        }
    }
}
