using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class OutParametersTests : DecompilerTestsBase
    {
        [Test]
        public void TestOutParameter()
        {
            var mi = typeof(OutParametersTests).GetMethod(nameof(GetInt));

            var expression = (Expression<Func<string, int>>) MethodBodyDecompiler.Decompile(mi);

            Assert.That(expression.ToString(), Is.EqualTo("s => {var Param_0; ... }"));
            Assert.That(DebugView(expression), Is.EqualTo(@".Lambda #Lambda1<System.Func`2[System.String,System.Int32]>(System.String $s) {
    .Block(System.Int32 $var1) {
        .If (
            .Call System.Int32.TryParse(
                $s,
                $var1)
        ) {
            $var1
        } .Else {
            -1
        }
    }
}"));

            var compiledMethod = expression.Compile();

            Assert.That(compiledMethod("123"), Is.EqualTo(123));
            Assert.That(compiledMethod("999"), Is.EqualTo(999));
        }

        public static int GetInt(string s) => int.TryParse(s, out var i) ? i : -1;
    }
}