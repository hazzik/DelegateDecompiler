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

            public void SetMultiple(int value)
            {
                staticField = value;
                myField = value;
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
 
        [Test]
        public void TestSetMultiple()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.SetMultiple));
            var expression = method.Decompile();

            Assert.That(expression.ToString(), Is.EqualTo("(this, value) => { ... }"));
            Assert.That(expression.Body, Is.InstanceOf<BlockExpression>());
            var block = (BlockExpression)expression.Body;
            Assert.That(block.Expressions[0].ToString(), Is.EqualTo("(TestClass.staticField = value)"));
            Assert.That(block.Expressions[1].ToString(), Is.EqualTo("(this.myField = value)"));
            Assert.That(block.Expressions[2].ToString(), Is.EqualTo("(this.MyProperty = value)"));
        }
    }
}
