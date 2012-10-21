using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class ArrayTests : DecompilerTestsBase
    {
        [Fact]
        public void DecompileArrayWithBounds()
        {
            Expression<Func<int[]>> expected = () => new int[10];
            Func<int[]> compiled = () => new int[10];
            Test(expected, compiled);
        }
    }
}
