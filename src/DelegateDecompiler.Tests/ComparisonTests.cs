using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class ComparisonTests : DecompilerTestsBase
    {
        [Fact]
        public void Ceq()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x == y;
            Func<int, int, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Fact]
        public void Cgt()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x > y;
            Func<int, int, bool> compiled = (x, y) => x > y;
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void Cge()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x >= y;
            Func<int, int, bool> compiled = (x, y) => x >= y;
            Test(expected, compiled);
        }

        [Fact]
        public void Clt()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x < y;
            Func<int, int, bool> compiled = (x, y) => x < y;
            Test(expected, compiled);
        }

        [Fact(Skip = "Need optimization")]
        public void Cle()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x <= y;
            Func<int, int, bool> compiled = (x, y) => x <= y;
            Test(expected, compiled);
        }
    }
}