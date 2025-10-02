using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class StargTests : DecompilerTestsBase
    {
        public class TestClass
        {
            public static int ModifyParameter(int value)
            {
                value = value + 1;
                return value;
            }

            public static int ModifyParameterWithCondition(int value)
            {
                if (value > 0)
                    value = value * 2;
                else
                    value = -value;
                return value;
            }

            public static string ModifyStringParameter(string text)
            {
                if (text != null)
                    text = text + " modified";
                return text;
            }

            // Non-static method for testing instance method parameter modification
            public int ModifyInstanceParameter(int value)
            {
                value = value + 1;
                return value;
            }
        }

        [Test]
        public void StargProcessor_ShouldHandleSimpleParameterModification()
        {
            Expression<Func<int, int>> expected = value => value + 1;
            Func<int, int> compiled = TestClass.ModifyParameter;
            Test(expected, compiled);
        }

        [Test]
        public void StargProcessor_ShouldHandleConditionalParameterModification()
        {
            Expression<Func<int, int>> expected = value => value > 0 ? value * 2 : -value;
            Func<int, int> compiled = TestClass.ModifyParameterWithCondition;
            Test(expected, compiled);
        }

        [Test]
        public void StargProcessor_ShouldHandleStringParameterModification()
        {
            Expression<Func<string, string>> expected = text => text + " modified";
            Func<string, string> compiled = TestClass.ModifyStringParameter;
            Test(expected, compiled);
        }

        [Test]
        public void StargProcessor_ShouldHandleInstanceMethodParameterModification()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.ModifyInstanceParameter));
            var expression = method.Decompile();

            Assert.That(expression.ToString(), Is.EqualTo("(this, value) => (value + 1)"));
        }
    }
}