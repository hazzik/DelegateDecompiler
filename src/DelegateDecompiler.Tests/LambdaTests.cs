using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class LambdaTest : DecompilerTestsBase
    {
        [Test]
        public void CanUseLambda()
        {
            Expression<Func<bool>> expected = () => Enumerable.Range(1, 10).Any(i => i == 5);
            Func<bool> compiled = () => Enumerable.Range(1, 10).Any(i => i == 5);
            Test(expected, compiled);
        }

        [Test, Ignore("Not supported yet")]
        public void CanUseLambdaWithClosure()
        {
            var j = 5;
            Expression<Func<bool>> expected = () => Enumerable.Range(1, 10).Any(i => i == j);
            Func<bool> compiled = () => Enumerable.Range(1, 10).Any(i => i == j);
            Test(expected, compiled);
        }

        [Test]
        public void CanUseTwoLambda()
        {
            Expression<Func<Dictionary<int, int>>> expected = () => Enumerable.Range(1, 10).ToDictionary(_ => _, _ => _);
            Func<Dictionary<int, int>> compiled = () => Enumerable.Range(1, 10).ToDictionary(_ => _, _ => _);
            Test(expected, compiled);
        }

        [Test]
        public void CanUseConstantAndLambda()
        {
            Expression<Func<int>> expected = () => Enumerable.Range(1, 10).Aggregate(0, (acc, x) => acc + x);
            Func<int> compiled = () => Enumerable.Range(1, 10).Aggregate(0, (acc, x) => acc + x);
            Test(expected, compiled);
        }

        [Test]
        public void CanUseLambdaAndConstant()
        {
            Expression<Func<IOrderedEnumerable<int>>> expected = () => Enumerable.Range(1, 10).OrderBy(x => x, Comparer<int>.Default);
            Func<IOrderedEnumerable<int>> compiled = () => Enumerable.Range(1, 10).OrderBy(x => x, Comparer<int>.Default);
            Test(expected, compiled);
        }

        [Test]
        public void CanUseLambdaInLambda()
        {
            Expression<Func<IEnumerable<int>>> expected =
                () => Enumerable.Range(1, 10).Select(x => Enumerable.Range(0, x).Aggregate(0, (acc, i) => acc + i));
            Func<IEnumerable<int>> compiled =
                () => Enumerable.Range(1, 10).Select(x => Enumerable.Range(0, x).Aggregate(0, (acc, i) => acc + i));
            Test(expected, compiled);
        }

        [Test]
        public void CanUseLambdaAndNullParameter()
        {
            Expression<Func<IOrderedEnumerable<int>>> expected = () => Enumerable.Range(1, 10).OrderBy(x => x, null);
            Func<IOrderedEnumerable<int>> compiled = () => Enumerable.Range(1, 10).OrderBy(x => x, null);
            Test(expected, compiled);
        }
    }
}