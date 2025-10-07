using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class StringConcatTests : DecompilerTestsBase
    {
        [Test]
        public void StringConcat()
        {
            Expression<Func<string, string, string, string, string>> expected = (w, x, y, z) => w + " " + x + " " + y + " " + z;
            Func<string, string, string, string, string> compiled = (w, x, y, z) => w + " " + x + " " + y + " " + z;
            Test(compiled, expected);
        }

        [Test]
        public void StringConcat5()
        {
            Expression<Func<string, string, string, string, string, string>> expected = (u, w, x, y, z) => u + " " + w + " " + x + " " + y + " " + z;
            Func<string, string, string, string, string, string> compiled = (u, w, x, y, z) => u + " " + w + " " + x + " " + y + " " + z;
            Test(compiled, expected);
        }

        [Test]
        public void StringConcatSingleArg()
        {
            Expression<Func<object, string>> expected = x => string.Concat(x);
            Func<object, string> compiled = x => string.Concat(x);
            Test(compiled, expected);
        }

        [Test]
        public void StringConcatSingleArgParams()
        {
            Expression<Func<string, string>> expected = x => string.Concat(x);
            Func<string, string> compiled = x => string.Concat(x);
            Test(compiled, expected);
        }

        [Test]
        public void StringConcatObjects()
        {
            Expression<Func<int, string, int, string>> expected1 = (x, y, z) => x + y + z;
            Expression<Func<int, string, int, string>> expected2 = (x, y, z) => x.ToString() + y + z.ToString();
            Func<int, string, int, string> compiled = (x, y, z) => x + y + z;
            Test(compiled, expected1, expected2);
        }

        [Test]
        public void StringConcatObjects2()
        {
            Expression<Func<int, string, double, string>> expected1 = (x, y, z) => x + y + z;
            Expression<Func<int, string, double, string>> expected2 = (x, y, z) => x.ToString() + y + z.ToString();
            Func<int, string, double, string> compiled = (x, y, z) => x + y + z;
            Test(compiled, expected1, expected2);
        }
    }
}