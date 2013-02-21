using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class DecompilerTestsBase
    {
        private static readonly Func<Expression, string> debugView = BuildDebugView();

        private static Func<Expression, string> BuildDebugView()
        {
            var parameter = Expression.Parameter(typeof (Expression), "e");
            return Expression.Lambda<Func<Expression, string>>(Expression.Property(parameter, "DebugView"), parameter).Compile();
        }

        protected static void Test<T>(Expression<T> expected, T compiled)
        {
            //Double cast required as we can not convert T to Delegate directly
            var decompiled = ((Delegate) ((object) compiled)).Decompile();

            Assert.Equal(expected.ToString(), decompiled.ToString());
            Assert.Equal(debugView(expected), debugView(decompiled));
        }
    }
}
