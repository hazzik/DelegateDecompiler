using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class AssignmentTests
    {
        public class TestClass
        {
            private static int staticField;
            private int myField;
            private int MyProperty { get; set; }
            private int MyAdditionalProperty { get; set; }

            public void SetStaticField(int value)
            {
                staticField = value;
            }

            public void SetMyField(int value)
            {
                myField = value;
            }

            public void SetMyProperty(int value)
            {
                MyProperty = value;
            }

            public void SetMyPropertyAndMyAdditionalProperty(int value, int additionalValue)
            {
                MyProperty = value;
                MyAdditionalProperty = additionalValue;
            }
        }
        
        [Test]
        public void TestSetStaticField()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.SetStaticField));
            var expression = method.Decompile();

            Assert.That(expression.ToString(), Is.EqualTo("(this, value) => (TestClass.staticField = value)"));
        }

        [Test]
        public void TestSetField()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.SetMyField));
            var expression = method.Decompile();

            Assert.That(expression.ToString(), Is.EqualTo("(this, value) => (this.myField = value)"));
        }

        [Test]
        public void TestSetProperty()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.SetMyProperty));
            var expression = method.Decompile();

            Assert.That(expression.ToString(), Is.EqualTo("(this, value) => (this.MyProperty = value)"));
        }

        [Test]
        public void TestSetPropertyAndAdditionalProperty()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.SetMyPropertyAndMyAdditionalProperty));
            var expression = method.Decompile();

            var block = expression.Body as BlockExpression;
            Assert.NotNull(block);
            Assert.That(block.Expressions[0].ToString(), Is.EqualTo("(this.MyProperty = value)"));
            Assert.That(block.Expressions[1].ToString(), Is.EqualTo("(this.MyAdditionalProperty = additionalValue)"));
        }
    }
}
