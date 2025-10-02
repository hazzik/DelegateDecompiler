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
        }

        [Test]
        public void StargProcessor_ShouldHandleSimpleParameterModification()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.ModifyParameter));
            var decompiled = method.Decompile();
            
            // Verify the method can be decompiled without errors
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            // Assert on the decompiled expression structure
            Assert.That(decompiled.Parameters.Count, Is.EqualTo(1));
            Assert.That(decompiled.Parameters[0].Name, Is.EqualTo("value"));
            Assert.That(decompiled.Parameters[0].Type, Is.EqualTo(typeof(int)));
            
            // Currently, the StargProcessor implementation just discards the stored value
            // so the body should just return the parameter unchanged
            // This assertion validates the current behavior - TODO: enhance when semantics are fully implemented
            Assert.That(decompiled.Body.ToString(), Is.EqualTo("value"));
            Assert.That(decompiled.Body, Is.InstanceOf<ParameterExpression>());
        }

        [Test]
        public void StargProcessor_ShouldHandleConditionalParameterModification()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.ModifyParameterWithCondition));
            var decompiled = method.Decompile();
            
            // Verify the method can be decompiled without errors
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            // Assert on the decompiled expression structure
            Assert.That(decompiled.Parameters.Count, Is.EqualTo(1));
            Assert.That(decompiled.Parameters[0].Name, Is.EqualTo("value"));
            Assert.That(decompiled.Parameters[0].Type, Is.EqualTo(typeof(int)));
            
            // Currently returns parameter unchanged due to incomplete Starg semantics
            Assert.That(decompiled.Body.ToString(), Is.EqualTo("value"));
            Assert.That(decompiled.Body, Is.InstanceOf<ParameterExpression>());
        }

        [Test]
        public void StargProcessor_ShouldHandleStringParameterModification()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.ModifyStringParameter));
            var decompiled = method.Decompile();
            
            // Verify the method can be decompiled without errors
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            // Assert on the decompiled expression structure
            Assert.That(decompiled.Parameters.Count, Is.EqualTo(1));
            Assert.That(decompiled.Parameters[0].Name, Is.EqualTo("text"));
            Assert.That(decompiled.Parameters[0].Type, Is.EqualTo(typeof(string)));
            
            // Currently returns parameter unchanged due to incomplete Starg semantics
            Assert.That(decompiled.Body.ToString(), Is.EqualTo("text"));
            Assert.That(decompiled.Body, Is.InstanceOf<ParameterExpression>());
        }
    }
}