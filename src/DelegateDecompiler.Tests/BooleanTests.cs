using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class BooleanTests : DecompilerTestsBase
    {
        [Test]
        public void TestParameterAndFalse()
        {
            Expression<Func<bool, bool>> expected = b => b & false;
            Expression<Func<bool, bool>> expected2 = b => false;
            Func<bool, bool> compiled = b => b & false;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterAndTrue()
        {
            Expression<Func<bool, bool>> expected = b => b & true;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => b & true;
            Test(expected, expected2, compiled);
        }

        [Test, Ignore("Not fixed yet.")]
        public void TestFuncAndFalse()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() & false;
            Func<Func<bool>, bool> compiled = b => b.Invoke() & false;
            Test(expected, compiled);
        }

        [Test]
        public void TestFuncAndTrue()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() & true;
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => b.Invoke() & true;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFalseAndParameter()
        {
            Expression<Func<bool, bool>> expected = b => false & b;
            Expression<Func<bool, bool>> expected2 = b => false;
            Func<bool, bool> compiled = b => false & b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueAndParameter()
        {
            Expression<Func<bool, bool>> expected = b => true & b;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => true & b;
            Test(expected, expected2, compiled);
        }

        [Test, Ignore("Not fixed yet.")]
        public void TestFalseAndFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => false & b.Invoke();
            Func<Func<bool>, bool> compiled = b => false & b.Invoke();
            Test(expected, compiled);
        }

        [Test]
        public void TestTrueAndFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => true & b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => true & b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterAndParameter()
        {
            Expression<Func<bool, bool, bool>> expected = (x, y) => x & y;
            Func<bool, bool, bool> compiled = (x, y) => x & y;
            Test(expected, compiled);
        }

        [Test]
        public void TestFuncAndFunc()
        {
            Expression<Func<Func<bool>, Func<bool>, bool>> expected = (x, y) => x.Invoke() & y.Invoke();
            Func<Func<bool>, Func<bool>, bool> compiled = (x, y) => x.Invoke() & y.Invoke();
            Test(expected, compiled);
        }

        [Test, Ignore("Needs optimization")]
        public void TestParameterAndAlsoFalse()
        {
            Expression<Func<bool, bool>> expected = b => b && false;
            Expression<Func<bool, bool>> expected2 = b => false;
            Func<bool, bool> compiled = b => b && false;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterAndAlsoTrue()
        {
            Expression<Func<bool, bool>> expected = b => b && true;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => b && true;
            Test(expected, expected2, compiled);
        }

        [Test, Ignore("Needs optimization")]
        public void TestFuncAndAlsoFalse()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() && false;
            Func<Func<bool>, bool> compiled = b => b.Invoke() && false;
            Test(expected, compiled);
        }

        [Test]
        public void TestFuncAndAlsoTrue()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() && true;
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => b.Invoke() && true;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFalseAndAlsoParameter()
        {
            Expression<Func<bool, bool>> expected = b => false && b;
            Expression<Func<bool, bool>> expected2 = b => false;
            Func<bool, bool> compiled = b => false && b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueAndAlsoParameter()
        {
            Expression<Func<bool, bool>> expected = b => true && b;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => true && b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFalseAndAlsoFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => false && b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => false;
            Func<Func<bool>, bool> compiled = b => false && b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueAndAlsoFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => true && b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => true && b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterAndAlsoParameter()
        {
            Expression<Func<bool, bool, bool>> expected = (x, y) => x && y;
            Expression<Func<bool, bool, bool>> expected2 = (x, y) => x & y;
            Func<bool, bool, bool> compiled = (x, y) => x && y;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFuncAndAlsoFunc()
        {
            Expression<Func<Func<bool>, Func<bool>, bool>> expected = (x, y) => x.Invoke() && y.Invoke();
            Func<Func<bool>, Func<bool>, bool> compiled = (x, y) => x.Invoke() && y.Invoke();
            Test(expected, compiled);
        }

        [Test]
        public void TestParameterOrFalse()
        {
            Expression<Func<bool, bool>> expected = b => b | false;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => b | false;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterOrTrue()
        {
            Expression<Func<bool, bool>> expected = b => b | true;
            Expression<Func<bool, bool>> expected2 = b => true;
            Func<bool, bool> compiled = b => b | true;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFuncOrFalse()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() | false;
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => b.Invoke() | false;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFuncOrTrue()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() | true;
            Expression<Func<Func<bool>, bool>> expected2 = b => true;
            Func<Func<bool>, bool> compiled = b => b.Invoke() | true;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFalseOrParameter()
        {
            Expression<Func<bool, bool>> expected = b => false | b;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => false | b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueOrParameter()
        {
            Expression<Func<bool, bool>> expected = b => true | b;
            Expression<Func<bool, bool>> expected2 = b => true;
            Func<bool, bool> compiled = b => true | b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFalseOrFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => false | b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => false | b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueOrFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => true | b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => true;
            Func<Func<bool>, bool> compiled = b => true | b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterOrParameter()
        {
            Expression<Func<bool, bool, bool>> expected = (x, y) => x | y;
            Func<bool, bool, bool> compiled = (x, y) => x | y;
            Test(expected, compiled);
        }


        [Test]
        public void TestFuncOrFunc()
        {
            Expression<Func<Func<bool>, Func<bool>, bool>> expected = (x, y) => x.Invoke() | y.Invoke();
            Func<Func<bool>, Func<bool>, bool> compiled = (x, y) => x.Invoke() | y.Invoke();
            Test(expected, compiled);
        }

        [Test]
        public void TestParameterOrElseFalse()
        {
            Expression<Func<bool, bool>> expected = b => b || false;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => b || false;
            Test(expected, expected2, compiled);
        }

        [Test, Ignore("Needs optimization")]
        public void TestParameterOrElseTrue()
        {
            Expression<Func<bool, bool>> expected = b => b || true;
            Expression<Func<bool, bool>> expected2 = b => true;
            Func<bool, bool> compiled = b => b || true;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFuncOrElseFalse()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() || false;
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => b.Invoke() || false;
            Test(expected, expected2, compiled);
        }

        [Test, Ignore("Needs optimization")]
        public void TestFuncOrElseTrue()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() || true;
            Func<Func<bool>, bool> compiled = b => b.Invoke() || true;
            Test(expected, compiled);
        }

        [Test]
        public void TestFalseOrElseParameter()
        {
            Expression<Func<bool, bool>> expected = b => false || b;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => false || b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueOrElseParameter()
        {
            Expression<Func<bool, bool>> expected = b => true || b;
            Expression<Func<bool, bool>> expected2 = b => true;
            Func<bool, bool> compiled = b => true || b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFalseOrElseFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => false || b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => false || b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueOrElseFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => true || b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => true;
            Func<Func<bool>, bool> compiled = b => true || b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterOrElseParameter()
        {
            Expression<Func<bool, bool, bool>> expected = (x, y) => x || y;
            Expression<Func<bool, bool, bool>> expected2 = (x, y) => x | y;
            Func<bool, bool, bool> compiled = (x, y) => x || y;
            Test(expected, expected2, compiled);
        }

        [Test, Ignore("Needs optimization")]
        public void TestFuncOrElseFunc()
        {
            Expression<Func<Func<bool>, Func<bool>, bool>> expected = (x, y) => x.Invoke() || y.Invoke();
            Func<Func<bool>, Func<bool>, bool> compiled = (x, y) => x.Invoke() || y.Invoke();
            Test(expected, compiled);
        }

        [Test]
        public void TestParameterEqualFalse()
        {
            Expression<Func<bool, bool>> expected = b => b == false;
            Expression<Func<bool, bool>> expected2 = b => !b;
            Func<bool, bool> compiled = b => b == false;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterEqualTrue()
        {
            Expression<Func<bool, bool>> expected = b => b == true;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => b == true;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFuncEqualFalse()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() == false;
            Expression<Func<Func<bool>, bool>> expected2 = b => !b.Invoke();
            Func<Func<bool>, bool> compiled = b => b.Invoke() == false;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFuncEqualTrue()
        {
            Expression<Func<Func<bool>, bool>> expected = b => b.Invoke() == true;
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => b.Invoke() == true;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFalseEqualParameter()
        {
            Expression<Func<bool, bool>> expected = b => false == b;
            Expression<Func<bool, bool>> expected2 = b => !b;
            Func<bool, bool> compiled = b => false == b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueEqualParameter()
        {
            Expression<Func<bool, bool>> expected = b => true == b;
            Expression<Func<bool, bool>> expected2 = b => b;
            Func<bool, bool> compiled = b => true == b;
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestFalseEqualFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => false == b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => !b.Invoke();
            Func<Func<bool>, bool> compiled = b => false == b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestTrueEqualFunc()
        {
            Expression<Func<Func<bool>, bool>> expected = b => true == b.Invoke();
            Expression<Func<Func<bool>, bool>> expected2 = b => b.Invoke();
            Func<Func<bool>, bool> compiled = b => true == b.Invoke();
            Test(expected, expected2, compiled);
        }

        [Test]
        public void TestParameterEqualParameter()
        {
            Expression<Func<bool, bool, bool>> expected = (x, y) => x == y;
            Func<bool, bool, bool> compiled = (x, y) => x == y;
            Test(expected, compiled);
        }

        [Test]
        public void TestFuncEqualFunc()
        {
            Expression<Func<Func<bool>, Func<bool>, bool>> expected = (x, y) => x.Invoke() == y.Invoke();
            Func<Func<bool>, Func<bool>, bool> compiled = (x, y) => x.Invoke() == y.Invoke();
            Test(expected, compiled);
        }
    }
}
