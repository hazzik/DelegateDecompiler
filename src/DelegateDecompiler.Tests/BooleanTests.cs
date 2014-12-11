using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class BooleanTests : DecompilerTestsBase
    {
        [Test]
        public void ExpressionWithBoolean()
        {
            Expression<Func<string>> expected = () => Method1(true);
            Func<string> compiled = () => Method1(true);
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithBoolean2()
        {
            Expression<Func<string>> expected = () => true.ToString();
            Func<string> compiled = () => true.ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ExpressionWithBoolean3()
        {
            Expression<Func<bool,string>> expected = x => x.ToString();
            Func<bool, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method1(bool arg)
        {
            return arg.ToString();
        }
    }
}