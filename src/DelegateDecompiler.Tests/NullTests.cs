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
    }
}