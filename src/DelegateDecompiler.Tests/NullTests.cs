using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class NullTests : DecompilerTestsBase
    {
        [Fact]
        public void ExpressionWithNull()
        {
            Expression<Func<string>> expected = () => null;
            Func<string> compiled = () => null;
            Test(expected, compiled);
        }
        
        [Fact]
        public void ExpressionStringEqualsNull()
        {
            Expression<Func<string, bool>> expected = x => x.Equals(null);
            Func<string, bool> compiled = x => x.Equals(null);
            Test(expected, compiled);
        }

        [Fact]
        public void ExpressionWithNullable()
        {
            Expression<Func<int, int?>> expected = x => (int?)x;
            Func<int, int?> compiled = x => (int?)x;
            Test(expected, compiled);
        }
        [Fact]
        public void ExpressionWithNullable2()
        {
            Expression<Func<int?, int>> expected = x => x ?? 0;
            Func<int?, int> compiled = x => x ?? 0;
            Test(expected, compiled);
        }
    }
}