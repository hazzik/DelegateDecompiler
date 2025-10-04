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

        [Test]
        public void ConditionalThrowInTernaryOperator()
        {
            Func<int, string> compiled = x => x > 0 ? "positive" : throw new ArgumentException("negative");
            
            // Create expected expression programmatically for conditional with throw
            var parameter = Expression.Parameter(typeof(int), "x");
            var positiveConstant = Expression.Constant("positive");
            var constructor = typeof(ArgumentException).GetConstructor(new[] { typeof(string) });
            var messageConstant = Expression.Constant("negative");
            var newExpression = Expression.New(constructor, messageConstant);
            var throwExpression = Expression.Throw(newExpression, typeof(string));
            var condition = Expression.GreaterThan(parameter, Expression.Constant(0));
            var conditionalExpression = Expression.Condition(condition, positiveConstant, throwExpression);
            var expectedExpression = Expression.Lambda<Func<int, string>>(conditionalExpression, parameter);
            
            Test(compiled, expectedExpression);
        }
    }
}