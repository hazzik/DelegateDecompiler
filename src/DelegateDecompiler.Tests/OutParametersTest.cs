using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class OutParametersTests
    {
        [Test]
        public void TestMethod()
        {
            var mi = typeof(OutParametersTests).GetMethod(nameof(GetInt));

            var expression = (Expression<Func<string, int>>) MethodBodyDecompiler.Decompile(mi);

            Assert.That(expression.ToString(), Is.EqualTo("s => {var var0; ... }"));

            var compiledMethod = expression.Compile();

            Assert.That(compiledMethod("123"), Is.EqualTo(123));
            Assert.That(compiledMethod("999"), Is.EqualTo(999));
        }

        public static int GetInt(string s) => int.TryParse(s, out var i) ? i : -1;
    }
}