using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiller.Tests
{
    public class DelegateDecompillerExtensionsTests
    {
        [Fact]
        public void ShouldBeAbleToDecompileExpression()
        {
            Expression<Func<object, object>> expression = o => o;

            var compiled = expression.Compile();

            var decompilled = compiled.Decompile();

            Assert.Equal(expression.ToString(), decompilled.ToString());
        }
    }
}
