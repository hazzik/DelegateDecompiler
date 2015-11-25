using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class TypeAsTests : DecompilerTestsBase
    {
        class Parent { }
        class Child : Parent { }

        [Test]
        public void TypeAsTest()
        {
            Expression<Func<Parent, Child>> expected = parent => parent as Child;
            Func<Parent, Child> compiled = parent => parent as Child;
            Test(expected, compiled);
        }

        [Test]
        public void TypeIsTest()
        {
            Expression<Func<Parent, bool>> expected = parent => parent is Child;
            Func<Parent, bool> compiled = parent => parent is Child;
            Test(expected, compiled);
        }

        [Test]
        public void TypeIsNotTest()
        {
            Expression<Func<Parent, bool>> expected = parent => !(parent is Child);
            Func<Parent, bool> compiled = parent => !(parent is Child);
            Test(expected, compiled);
        }
    }
}