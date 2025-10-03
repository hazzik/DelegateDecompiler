using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using DelegateDecompiler.ControlFlow;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ControlFlowGraphTests
    {
        private class Sample
        {
            public int SumLoop(int n)
            {
                var sum = 0;
                for (int i = 0; i < n; i++)
                {
                    sum += i;
                }
                return sum;
            }

            public int TryCatchFinally(int x)
            {
                try
                {
                    if (x < 0) throw new ArgumentException();
                    return x;
                }
                catch (ArgumentException)
                {
                    return -1;
                }
                finally
                {
                    _ = x + 1; // side effect
                }
            }
        }

        [Test]
        public void DetectsExceptionEdges()
        {
            Func<int, int> method = x =>
            {
                try
                {
                    return x < 0 ? throw new ArgumentException() : x;
                }
                catch (ArgumentException)
                {
                    return -1;
                }
                finally
                {
                    _ = x + 1; // side effect
                }
            };

            // var lambdaExpression = method.Decompile();

            var cfg = method.Method.BuildControlFlowGraph();
            var hasExceptionEdge = cfg.Blocks.Any(b => b.Successors.Any(e => e.IsException));
            Assert.That(hasExceptionEdge, Is.True, "Expected at least one exception edge");
        }
    }
}
