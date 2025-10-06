using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture, Ignore("Not supported yet")]
    public class TryCatchTests : DecompilerTestsBase
    {
        [Test]
        public void TryCatchSimple()
        {
            Func<int, int> compiled = x =>
            {
                try
                {
                    return x * 2;
                }
                catch (ArgumentException)
                {
                    return -1;
                }
            };

            var x = Expression.Parameter(typeof(int), "x");
            var expected = Expression.Lambda<Func<int, int>>(
                Expression.TryCatch(
                    Expression.Multiply(x, Expression.Constant(2)),
                    Expression.Catch(typeof(ArgumentException), Expression.Constant(-1))
                ), x);

            Test(compiled, expected);
        }

        [Test]
        public void TryCatchWithSpecificException()
        {
            Func<int, int> compiled = x =>
            {
                try
                {
                    return x * 2;
                }
                catch (FormatException)
                {
                    return 0;
                }
            };

            var x = Expression.Parameter(typeof(int), "x");
            var expected = Expression.Lambda<Func<int, int>>(
                Expression.TryCatch(
                    Expression.Multiply(x, Expression.Constant(2)),
                    Expression.Catch(typeof(FormatException), Expression.Constant(0))
                ), x);

            Test(compiled, expected);
        }

        [Test]
        public void TryCatchFinally()
        {
            Func<int, int> compiled = x =>
            {
                try
                {
                    return x * 2;
                }
                catch (ArgumentException)
                {
                    return -1;
                }
                finally
                {
                    Console.WriteLine("Processing");
                }
            };

            var param = Expression.Parameter(typeof(int), "x");
            var expected = Expression.Lambda<Func<int, int>>(
                Expression.TryCatchFinally(
                    Expression.Multiply(param, Expression.Constant(2)),
                    Expression.Block(
                        Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) })!,
                            Expression.Constant("Processing"))
                    ),
                    Expression.Catch(typeof(ArgumentException), Expression.Constant(-1))
                ),
                param);

            Test(compiled, expected);
        }

        [Test]
        public void TryFinallyOnly()
        {
            Func<int, int> compiled = x =>
            {
                try
                {
                    return x * 2;
                }
                finally
                {
                    Console.WriteLine($"Finally executed for {x}");
                }
            };

            var param = Expression.Parameter(typeof(int), "x");
            var expected = Expression.Lambda<Func<int, int>>(
                Expression.TryFinally(
                    Expression.Multiply(param, Expression.Constant(2)),
                    Expression.Block(
                        Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }),
                                       Expression.Call(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object) }),
                                                     Expression.Constant("Finally executed for {0}"),
                                                     Expression.Convert(param, typeof(object))))
                    )
                ),
                param);

            Test(compiled, expected);
        }

        [Test]
        public void TryCatchWithExceptionVariable()
        {
            Func<string, string> compiled = s =>
            {
                try
                {
                    return s.ToUpper();
                }
                catch (ArgumentNullException ex)
                {
                    return ex.Message;
                }
            };

            var s = Expression.Parameter(typeof(string), "s");
            var ex = Expression.Parameter(typeof(ArgumentNullException), "ex");
            var expected = Expression.Lambda<Func<string, string>>(
                Expression.TryCatch(
                    Expression.Call(s, typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!),
                    Expression.Catch(
                        ex,
                        Expression.Property(ex, nameof(Exception.Message))
                    )
                ), s);

            Test(compiled, expected);
        }
    }
}