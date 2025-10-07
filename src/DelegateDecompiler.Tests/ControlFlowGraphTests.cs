using System;
using System.IO;
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

            var cfg = method.Method.BuildControlFlowGraph();
            var hasExceptionEdge = cfg.Blocks.Any(b => b.Successors.Any(e => e.IsException));
            Assert.That(hasExceptionEdge, Is.True, "Expected at least one exception edge");
        }

        [Test, Explicit]
        public void TestToString()
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

            var cfg = method.Method.BuildControlFlowGraph();

            Assert.That(cfg.ToString().NormalizeLineEndings(), Is.EqualTo(@"Control Flow Graph: DelegateDecompiler.Tests.ControlFlowGraphTests+<>c.<TestToString>b__2_0
Block 0 (0000-0000) [Entry]
  IL_0000: nop
    -> FallThrough B1
Block 1 (0001-0004)
  IL_0001: nop
  IL_0002: ldarg.1
  IL_0003: ldc.i4.0
  IL_0004: blt.s IL_0009
    -> ConditionalTrue B3
    -> ConditionalFalse B2
    -> Exception B5
    -> Finally B6
Block 2 (0006-0007)
  IL_0006: ldarg.1
  IL_0007: br.s IL_000f
    -> UnconditionalBranch B4
    -> Exception B5
    -> Finally B6
Block 3 (0009-000e)
  IL_0009: newobj Void .ctor()
  IL_000e: throw
    -> UnconditionalBranch B8
    -> Exception B5
    -> Finally B6
Block 4 (000f-0010)
  IL_000f: stloc.0
  IL_0010: leave.s IL_001c
    -> UnconditionalBranch B7
    -> Exception B5
    -> Finally B6
Block 5 (0012-0016)
  IL_0012: pop
  IL_0013: nop
  IL_0014: ldc.i4.m1
  IL_0015: stloc.0
  IL_0016: leave.s IL_001c
    -> UnconditionalBranch B7
    -> Finally B6
Block 6 (0018-001b)
  IL_0018: nop
  IL_0019: nop
  IL_001a: nop
  IL_001b: endfinally
    -> UnconditionalBranch B8
Block 7 (001c-001d)
  IL_001c: ldloc.0
  IL_001d: ret
    -> UnconditionalBranch B8
Block 8 (-) [Exit]
".NormalizeLineEndings()));
        }
    }
}
