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
        public void MulByInt()
        {
            Expression<Func<decimal, int, decimal>> expected = (x, y) => x * y;
            Func<decimal, int, decimal> compiled = (x, y) => x * y;
            Test(expected, compiled);
        }

        [Test]
        public void Div()
        {
            Expression<Func<decimal, decimal, decimal>> expected = (x, y) => x / y;
            Func<decimal, decimal, decimal> compiled = (x, y) => x / y;
            Test(expected, compiled);
        }

        [Test]
        public void DecimalCast()
        {
            Expression<Func<decimal>> expected1 = () => (decimal)1.21;
            Expression<Func<decimal>> expected2 = () => new decimal(121, 0, 0, false, 2);
            Func<decimal> compiled = () => (decimal)1.21;
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void DecimalConstructor()
        {
            Expression<Func<decimal>> expected1 = () => 1.21M;
            Expression<Func<decimal>> expected2 = () => new decimal(121, 0, 0, false, 2);
            Func<decimal> compiled = () => 1.21M;
            Test(expected1, expected2, compiled);
        }
    }
}