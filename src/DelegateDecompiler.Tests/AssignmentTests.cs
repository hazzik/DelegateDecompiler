using System;
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
    }
}
