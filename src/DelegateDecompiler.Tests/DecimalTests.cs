using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class DecimalTests : DecompilerTestsBase
    {
        [Test]
        public void Ceq()
        {
            Expression<Func<decimal, decimal, bool>> expected = (x, y) => x == y;
            Func<decimal, decimal, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void Cgt()
        {
            Expression<Func<decimal, decimal, bool>> expected = (x, y) => x > y;
            Func<decimal, decimal, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Test]
        public void Bgt()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x > y ? x : y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x > y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Cge()
        {
            Expression<Func<decimal, decimal, bool>> expected = (x, y) => x >= y;
            Func<decimal, decimal, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Test]
        public void Bge()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x >= y ? x : y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x >= y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Clt()
        {
            Expression<Func<decimal, decimal, bool>> expected = (x, y) => x < y;
            Func<decimal, decimal, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Test]
        public void Blt()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x < y ? x : y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x < y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Cle()
        {
            Expression<Func<decimal, decimal, bool>> expected = (x, y) => x <= y;
            Func<decimal, decimal, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }

        [Test]
        public void Ble()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x <= y ? x : y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x <= y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Bne()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x != y ? x : y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x != y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Beq()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x == y ? x : y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x == y ? x : y;
            Test(expected, compiled);
        }

        [Test]
        public void Mul()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x * y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x * y;
            Test(expected, compiled);
        }

        [Test]
        public void Div()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x / y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x / y;
            Test(expected, compiled);
        }
    }
}