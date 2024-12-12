using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue220And243
    {
        [Test]
        public void ShouldOptimizeSimpleExpressionQuickly()
        {
            // Somewhat artificial example, as Contains could be used instead. But the problem exists for every combination of binary operators.
            Expression<Func<Employee, bool>> expr = e => e.Id == 1 || e.Id == 2 || e.Id == 3 || e.Id == 4 || e.Id == 5 || e.Id == 6 || e.Id == 7 || e.Id == 8
                || e.Id == 9 || e.Id == 10 || e.Id == 11 || e.Id == 12 || e.Id == 13 || e.Id == 14 || e.Id == 15 || e.Id == 16 || e.Id == 17 || e.Id == 18
                || e.Id == 19 || e.Id == 20 || e.Id == 21 || e.Id == 22 || e.Id == 23 || e.Id == 24 || e.Id == 25 || e.Id == 26 || e.Id == 27;

            var sw = Stopwatch.StartNew();
            OptimizeExpressionVisitor.Optimize(expr);

            // Not so good to test runtime performance, but it's a working indicator. On my machine without fix > 20 s, with fix < 10 ms.
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(1000));
        }
    }
}
