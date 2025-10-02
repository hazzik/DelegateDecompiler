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
            
            // Should represent parameter modification operation
            var bodyString = decompiled.Body.ToString();
            Console.WriteLine($"Decompiled body: {bodyString}");
        }

        [Test]
        public void StargProcessor_ShouldHandleConditionalParameterModification()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.ModifyParameterWithCondition));
            var decompiled = method.Decompile();
            
            // Verify the method can be decompiled without errors
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            var bodyString = decompiled.Body.ToString();
            Console.WriteLine($"Decompiled body: {bodyString}");
        }

        [Test]
        public void StargProcessor_ShouldHandleStringParameterModification()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.ModifyStringParameter));
            var decompiled = method.Decompile();
            
            // Verify the method can be decompiled without errors
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            var bodyString = decompiled.Body.ToString();
            Console.WriteLine($"Decompiled body: {bodyString}");
        }
    }
}