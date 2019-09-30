using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ComparisonTests : DecompilerTestsBase
    {
        [Test]
        public void Ceq()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x == y;
            Func<int, int, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void Cgt()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x > y;
            Func<int, int, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Test]
        public void Bgt()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x > y ? x : y;
            Func<int, int, int> compiled = (x, y) => x > y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Cge()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x >= y;
            Func<int, int, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Test]
        public void Bge()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x >= y ? x : y;
            Func<int, int, int> compiled = (x, y) => x >= y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Clt()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x < y;
            Func<int, int, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Test]
        public void Blt()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x < y ? x : y;
            Func<int, int, int> compiled = (x, y) => x < y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Cle()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x <= y;
            Func<int, int, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }

        [Test]
        public void Ble()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x <= y ? x : y;
            Func<int, int, int> compiled = (x, y) => x <= y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Bne()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x != y ? x : y;
            Func<int, int, int> compiled = (x, y) => x != y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Beq()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x == y ? x : y;
            Func<int, int, int> compiled = (x, y) => x == y ? x : y;
            Test(expected, compiled);
        }
    }
}