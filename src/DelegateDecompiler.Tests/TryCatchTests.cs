using System;
using System.Linq.Expressions;
using DelegateDecompiler.ControlFlow;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture, Explicit]
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

            var p = Expression.Parameter(typeof(int), "x");
            var expected = Expression.Lambda<Func<int, int>>(
                Expression.TryCatch(
                    Expression.Multiply(p, Expression.Constant(2)),
                    Expression.Catch(typeof(ArgumentException), Expression.Constant(-1))
                ), p);

            Test(compiled, expected, x => x * 2);
        }
        
        [Test]
        public void TryCatchWithCondition()
        {
            Func<int, int> compiled = x =>
            {
                try
                {
                    return x > 0 ? x * 2 : x / 2;
                }
                catch (ArgumentException)
                {
                    return -1;
                }
            };

            var p = Expression.Parameter(typeof(int), "x");
            var expected = Expression.Lambda<Func<int, int>>(
                Expression.TryCatch(
                    Expression.Multiply(p, Expression.Constant(2)),
                    Expression.Catch(typeof(ArgumentException), Expression.Constant(-1))
                ), p);

            Test(compiled, expected, x => x > 0 ? x * 2 : x / 2);
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

            var p = Expression.Parameter(typeof(int), "x");
            var expected = Expression.Lambda<Func<int, int>>(
                Expression.TryCatch(
                    Expression.Multiply(p, Expression.Constant(2)),
                    Expression.Catch(typeof(FormatException), Expression.Constant(0))
                ), p);

            Test(compiled, expected, x => x * 2);
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
            
            var p = Expression.Parameter(typeof(int), "x");
            var expected = Expression.Lambda<Func<int, int>>(
                Expression.TryCatchFinally(
                    Expression.Multiply(p, Expression.Constant(2)),
                    Expression.Block(
                        Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) })!,
                            Expression.Constant("Processing"))
                    ),
                    Expression.Catch(typeof(ArgumentException), Expression.Constant(-1))
                ),
                p);

            Test(compiled, expected, x => x * 2);
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

            Test(compiled, expected, x => x * 2);
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

            var p = Expression.Parameter(typeof(string), "s");
            var ex = Expression.Parameter(typeof(ArgumentNullException), "ex");
            var expected = Expression.Lambda<Func<string, string>>(
                Expression.TryCatch(
                    Expression.Call(p, typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!),
                    Expression.Catch(
                        ex,
                        Expression.Property(ex, nameof(Exception.Message))
                    )
                ), p);

            Test(compiled, expected, s => s.ToUpper());
        }
    }
}