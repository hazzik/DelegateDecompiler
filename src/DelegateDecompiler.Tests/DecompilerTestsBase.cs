using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

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

            var x = expected.Body.ToString();
            Console.WriteLine(x);
            var y = decompiled.Body.ToString();
            Console.WriteLine(y);
            Assert.AreEqual(x, y);
            Assert.AreEqual(debugView(expected.Body), debugView(decompiled.Body));
        }

        protected static void Test<T>(Expression<T> expected, MethodInfo compiled)
        {
            //Double cast required as we can not convert T to Delegate directly
            var decompiled = compiled.Decompile();

            var x = expected.Body.ToString();
            Console.WriteLine(x);
            var y = decompiled.Body.ToString();
            Console.WriteLine(y);
            Assert.AreEqual(x, y);
            Assert.AreEqual(debugView(expected.Body), debugView(decompiled.Body));
        }

        protected static void Test<T>(Expression<T> expected1, Expression<T> expected2, T compiled)
        {
            //Double cast required as we can not convert T to Delegate directly
            var decompiled = ((Delegate) ((object) compiled)).Decompile();

            var x1 = expected1.Body.ToString();
            Console.WriteLine(x1);
            var x2 = expected2.Body.ToString();
            Console.WriteLine(x2);
            var y = decompiled.Body.ToString();
            Console.WriteLine(y);
            Assert.That(y, Is.EqualTo(x1).Or.EqualTo(x2));
            Assert.That(debugView(decompiled.Body), Is.EqualTo(debugView(expected1.Body)).Or.EqualTo(debugView(expected2.Body)));
        }
    }
}
