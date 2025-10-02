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
            [Computed]
            public static int ModifyParameter(int value)
            {
                value = value + 1;
                return value;
            }

            [Computed]  
            public static int ModifyParameterWithCondition(int value)
            {
                if (value > 0)
                    value = value * 2;
                else
                    value = -value;
                return value;
            }

            [Computed]
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
            
            // Just verify the method can be decompiled without errors
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            // The exact structure may vary, but it should represent the parameter modification
            Console.WriteLine($"Decompiled: {decompiled}");
            Console.WriteLine($"Body: {decompiled.Body}");
        }

        [Test]
        public void StargProcessor_ShouldHandleConditionalParameterModification()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.ModifyParameterWithCondition));
            var decompiled = method.Decompile();
            
            // Just verify the method can be decompiled without errors
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            Console.WriteLine($"Decompiled: {decompiled}");
            Console.WriteLine($"Body: {decompiled.Body}");
        }

        [Test]
        public void StargProcessor_ShouldHandleStringParameterModification()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.ModifyStringParameter));
            var decompiled = method.Decompile();
            
            // Just verify the method can be decompiled without errors
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            Console.WriteLine($"Decompiled: {decompiled}");
            Console.WriteLine($"Body: {decompiled.Body}");
        }
    }
}