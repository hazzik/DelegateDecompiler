using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class DecompilerTestsBase
    {
        protected static readonly Func<Expression, string> DebugView = BuildDebugView();

        static Func<Expression, string> BuildDebugView()
        {
            var parameter = Expression.Parameter(typeof(Expression), "e");
            return Expression.Lambda<Func<Expression, string>>(Expression.Property(parameter, "DebugView"), parameter).Compile();
        }
        
        
        protected static Expression<T> Test<T>(MethodInfo method, params Expression<T>[] expected)
        {
            var decompiled = (Expression<T>)method.Decompile();

            var decompiledBody = decompiled.Body.ToString();
            Console.WriteLine(decompiledBody);

            var expectedBodies = Array.ConvertAll(expected, object (e) => e.Body.ToString());
            Array.ForEach(expectedBodies, Console.WriteLine);
            Assert.That(decompiledBody, Is.AnyOf(expectedBodies));

            var expectedDebugViews = Array.ConvertAll(expected, object (e) => DebugView(e.Body));
            Assert.That(DebugView(decompiled.Body), Is.AnyOf(expectedDebugViews));
            return decompiled;
        }

        protected static Expression<T> Test<T>(T compiled, params Expression<T>[] expected)
        {
            var decompiled = TestNoDebugView(compiled, expected);

            var expectedDebugViews = Array.ConvertAll(expected, object (e) => DebugView(e.Body));
            Assert.That(DebugView(decompiled.Body), Is.AnyOf(expectedDebugViews));
            return decompiled;
        }

        protected static Expression<T> TestNoDebugView<T>(T compiled, params Expression<T>[] expected)
        {
            //Double cast required as we can not convert T to Delegate directly
            var decompiled = ((Delegate)((object)compiled)).Decompile();

            var decompiledBody = decompiled.Body.ToString();
            Console.WriteLine(decompiledBody);

            var expectedBodies = Array.ConvertAll(expected, object (e) => e.Body.ToString());
            Array.ForEach(expectedBodies, Console.WriteLine);
            Assert.That(decompiledBody, Is.AnyOf(expectedBodies));
            return (Expression<T>)decompiled;
        }

        protected static void AssertAreEqual(Expression actual, Expression expected, bool compareDebugView = true)
        {
            Assert.That(actual.ToString(), Is.EqualTo(expected.ToString()));
            if (compareDebugView)
                Assert.That(DebugView(actual), Is.EqualTo(DebugView(expected)));
        }

        protected static void AssertAreEqual(Expression actual, Expression expected1, Expression expected2, bool compareDebugView = true)
        {
            Assert.That(actual.ToString(), Is.AnyOf(expected1.ToString(), expected2.ToString()));
            if (compareDebugView)
                Assert.That(DebugView(actual), Is.AnyOf(DebugView(expected1), DebugView(expected2)));
        }
    }
}
