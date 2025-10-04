using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ThrowTests : DecompilerTestsBase
    {
        [Test]
        public void SimpleThrow()
        {
            Action compiled = () => throw new ArgumentException("test");
            
            // Create expected expression programmatically to match what decompiler produces
            var constructor = typeof(ArgumentException).GetConstructor(new[] { typeof(string) });
            var messageConstant = Expression.Constant("test");
            var newExpression = Expression.New(constructor, messageConstant);
            var throwExpression = Expression.Throw(newExpression);
            var expectedExpression = Expression.Lambda<Action>(throwExpression);
            
            Test(compiled, expectedExpression);
        }
    }
}