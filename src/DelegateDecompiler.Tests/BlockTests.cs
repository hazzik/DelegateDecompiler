using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class BlockTests : DecompilerTestsBase
    {
        [Fact]
        public void CanCallVoidMethod()
        {
            Expression<Action> e1 = () => Method1();
            Expression<Func<int>> e2 = () => 1;
            var expected = Expression.Lambda<Func<int>>(Expression.Block(e1.Body, e2.Body));
            Func<int> compiled = () =>
            {
                Method1();
                return 1;
            };
            Test(expected, compiled);
        }

        [Fact]
        public void CanCallNonVoidMethod()
        {
            Expression<Action> e1 = () => Method2();
            Expression<Func<int>> e2 = () => 1;
            var expected = Expression.Lambda<Func<int>>(Expression.Block(e1.Body, e2.Body));
            Func<int> compiled = () =>
            {
                Method2();
                return 1;
            };
            Test(expected, compiled);
        }

        private static void Method1()
        {
        }

        private static int Method2()
        {
            return 0;
        }
    }
}
