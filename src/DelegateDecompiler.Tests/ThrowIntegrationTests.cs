using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ThrowTests : DecompilerTestsBase
    {
        [Test]
        public void SimpleThrow()
        {
            Action compiled = () => throw new ArgumentException("test");
            
            // Should decompile without throwing NotSupportedException
            Assert.DoesNotThrow(() => {
                var decompiled = compiled.Decompile();
                Assert.That(decompiled, Is.Not.Null);
                Assert.That(decompiled.Body, Is.Not.Null);
                Assert.That(decompiled.Body.NodeType, Is.EqualTo(ExpressionType.Throw));
            });
        }

        [Test]
        public void ConditionalThrowInTernaryOperator()
        {
            Func<int, string> compiled = x => x > 0 ? "positive" : throw new ArgumentException("negative");
            
            // Should decompile without throwing NotSupportedException
            Assert.DoesNotThrow(() => {
                var decompiled = compiled.Decompile();
                Assert.That(decompiled, Is.Not.Null);
                Assert.That(decompiled.Body, Is.Not.Null);
            });
        }

        [Test]
        public void IfStatementWithThrow()
        {
            Action<int> compiled = x => { if (x < 0) throw new ArgumentException("negative"); };
            
            // Should decompile without throwing NotSupportedException
            Assert.DoesNotThrow(() => {
                var decompiled = compiled.Decompile();
                Assert.That(decompiled, Is.Not.Null);
                Assert.That(decompiled.Body, Is.Not.Null);
            });
        }

        public class TestClass
        {
            public int Value { get; set; }
            
            [Computed]
            public int PositiveValue => Value > 0 ? Value : throw new ArgumentException("Value must be positive");
        }

        [Test]
        public void ComputedPropertyWithThrow()
        {
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.PositiveValue));
            var getterMethod = propertyInfo.GetGetMethod();
            
            // Should decompile without throwing NotSupportedException
            Assert.DoesNotThrow(() => {
                var decompiled = getterMethod.Decompile();
                Assert.That(decompiled, Is.Not.Null);
                Assert.That(decompiled.Body, Is.Not.Null);
            });
        }
    }
}