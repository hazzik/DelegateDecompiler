using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class NestedExpressionsTests : DecompilerTestsBase
    {
        readonly int f1 = 0;
        static readonly int f2 = 0;
        int p1 => 0;
        static int p2 => 0;
        int M1() => 0;
        static int M2() => 0;
       
        [Test]
        public void TestNestedExpression()
        {
            Test<Func<IQueryable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == 0),
                ints => ints.SingleOrDefault(i => i == 0)
            );
        }
       
        [Test]
        public void TestExpressionWithSimpleClosure()
        {
            var v = 0;
            Test<Func<IQueryable<int>, IQueryable<int>>>(
                ints => ints.Select(_ => v),
                ints => ints.Select(_ => v)
            );
        }

        [TestCase(1)]
        public void TestExpressionWithParameterClosure(int p)
        {
            Test<Func<IQueryable<int>, IQueryable<int>>>(
                ints => ints.Select(_ => p),
                ints => ints.Select(_ => p)
            );
        }

        [Test]
        public void TestExpressionWithClosure()
        {
            var v = 0;
            Test<Func<IQueryable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == v),
                ints => ints.SingleOrDefault(i => i == v)
            );
        }
       
        [Test]
        public void TestExpressionWithFieldClosure()
        {
            Test<Func<IQueryable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == f1),
                ints => ints.SingleOrDefault(i => i == f1)
            );
        }
       
        [Test]
        public void TestExpressionWithStaticFieldClosure()
        {
            Test<Func<IQueryable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == f2),
                ints => ints.SingleOrDefault(i => i == f2)
            );
        }
       
        [Test]
        public void TestExpressionWithPropertyClosure()
        {
            Test<Func<IQueryable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == p1),
                ints => ints.SingleOrDefault(i => i == p1)
            );
        }
       
        [Test]
        public void TestExpressionWithStaticPropertyClosure()
        {
            Test<Func<IQueryable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == p2),
                ints => ints.SingleOrDefault(i => i == p2)
            );
        }
       
        [Test]
        public void TestExpressionWithMethodClosure()
        {
            Test<Func<IQueryable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == M1()),
                ints => ints.SingleOrDefault(i => i == M1())
            );
        }
       
        [Test]
        public void TestExpressionWithStaticMethodClosure()
        {
            Test<Func<IQueryable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == M2()),
                ints => ints.SingleOrDefault(i => i == M2())
            );
        }
       
        [Test]
        public void TestNestedFunc()
        {
            Test<Func<IEnumerable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == 0),
                ints => ints.SingleOrDefault(i => i == 0)
            );
        }
       
        [Test]
        public void TestFuncWithClosure()
        {
            var v = 0;
            Test<Func<IEnumerable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == v),
                ints => ints.SingleOrDefault(i => i == v)
            );
        }
       
        [Test]
        public void TestFuncWithFieldClosure()
        {
            Test<Func<IEnumerable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == f1),
                ints => ints.SingleOrDefault(i => i == f1)
            );
        }
       
        [Test]
        public void TestFuncWithStaticFieldClosure()
        {
            Test<Func<IEnumerable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == f2),
                ints => ints.SingleOrDefault(i => i == f2)
            );
        }
       
        [Test]
        public void TestFuncWithPropertyClosure()
        {
            Test<Func<IEnumerable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == p1),
                ints => ints.SingleOrDefault(i => i == p1)
            );
        }
       
        [Test]
        public void TestFuncWithStaticPropertyClosure()
        {
            Test<Func<IEnumerable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == p2),
                ints => ints.SingleOrDefault(i => i == p2)
            );
        }
       
        [Test]
        public void TestFuncWithMethodClosure()
        {
            Test<Func<IEnumerable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == M1()),
                ints => ints.SingleOrDefault(i => i == M1())
            );
        }
       
        [Test]
        public void TestFuncWithStaticMethodClosure()
        {
            Test<Func<IEnumerable<int>, int>>(
                ints => ints.SingleOrDefault(i => i == M2()),
                ints => ints.SingleOrDefault(i => i == M2())
            );
        }
    }
}
