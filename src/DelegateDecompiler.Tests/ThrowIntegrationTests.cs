using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ThrowIntegrationTests : DecompilerTestsBase
    {
        [Test]
        public void Should_decompile_simple_throw()
        {
            Action<int> method = x => { if (x < 0) throw new ArgumentException("negative"); };
            
            // Get the decompiled expression
            var decompiled = method.Decompile();
            
            // Verify it contains a throw expression
            var decompiledString = decompiled.Body.ToString();
            Console.WriteLine($"Decompiled: {decompiledString}");
            
            // The decompiled expression should contain the throw
            Assert.That(decompiledString, Does.Contain("throw"));
        }

        [Test]
        public void Should_decompile_throw_expression()
        {
            Func<int, int> method = x => x > 0 ? x : throw new ArgumentException("negative");
            
            // Get the decompiled expression  
            var decompiled = method.Decompile();
            
            // Verify it contains a throw expression
            var decompiledString = decompiled.Body.ToString();
            Console.WriteLine($"Decompiled: {decompiledString}");
            
            // The decompiled expression should contain the throw
            Assert.That(decompiledString, Does.Contain("throw"));
        }

        [Test]
        public void Should_decompile_simple_rethrow()
        {
            Action method = () => 
            {
                try
                {
                    throw new ArgumentException("original");
                }
                catch (ArgumentException)
                {
                    throw; // This should generate a rethrow opcode
                }
            };
            
            // Get the decompiled expression
            var decompiled = method.Decompile();
            
            // Verify it contains expressions (even if rethrow isn't directly visible in string representation)
            var decompiledString = decompiled.Body.ToString();
            Console.WriteLine($"Decompiled: {decompiledString}");
            
            // The method should decompile without throwing an exception
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
        }

        // Test method with computed attribute that includes throw
        public class TestClass
        {
            public int Value { get; set; }
            
            [Computed]
            public int PositiveValue => Value > 0 ? Value : throw new ArgumentException("Value must be positive");
        }

        [Test]
        public void Should_decompile_computed_property_with_throw()
        {
            TestClass testClass = new TestClass();
            
            // This should work now that throw is supported
            var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.PositiveValue));
            var getterMethod = propertyInfo.GetGetMethod();
            
            var decompiled = getterMethod.Decompile();
            Console.WriteLine($"Decompiled computed property: {decompiled.Body}");
            
            // Should decompile without exception
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body.ToString(), Does.Contain("throw"));
        }
    }
}