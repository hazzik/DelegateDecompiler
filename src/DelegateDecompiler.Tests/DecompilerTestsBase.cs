using System;
using System.Linq.Expressions;
using NUnit.Framework;
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

            var x = expected.ToString();
            Console.WriteLine(x);
            var y = decompiled.ToString();
            Console.WriteLine(y);
            Assert.AreEqual(x, y);
            Assert.AreEqual(debugView(expected), debugView(decompiled));
        }
    }
}
