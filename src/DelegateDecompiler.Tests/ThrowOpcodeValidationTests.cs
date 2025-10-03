using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ThrowOpcodeValidationTests : DecompilerTestsBase
    {
        [Test]
        public void OpCodes_Throw_Is_Now_Supported()
        {
            // Before the fix, this would have thrown a NotSupportedException about OpCodes.Throw being unsupported
            // Now it should work correctly
            Action method = () => throw new InvalidOperationException("Test exception");
            
            Assert.DoesNotThrow(() => {
                var decompiled = method.Decompile();
                Assert.That(decompiled, Is.Not.Null);
                Assert.That(decompiled.Body, Is.Not.Null);
                Assert.That(decompiled.Body.NodeType, Is.EqualTo(ExpressionType.Throw));
            });
        }

        [Test]
        public void OpCodes_Rethrow_Is_Now_Supported()
        {
            // Before the fix, this would have thrown a NotSupportedException about OpCodes.Rethrow being unsupported
            // Now it should work correctly
            Action method = () => 
            {
                try
                {
                    throw new InvalidOperationException("Original exception");
                }
                catch (InvalidOperationException)
                {
                    throw; // This generates OpCodes.Rethrow
                }
            };
            
            Assert.DoesNotThrow(() => {
                var decompiled = method.Decompile();
                Assert.That(decompiled, Is.Not.Null);
                Assert.That(decompiled.Body, Is.Not.Null);
            });
        }

        [Test]
        public void Conditional_Expression_With_Throw_Is_Supported()
        {
            // This tests the fixed stack merge logic for conditional branches with throw
            Func<int, string> method = x => x > 0 ? "positive" : throw new ArgumentException("negative");
            
            Assert.DoesNotThrow(() => {
                var decompiled = method.Decompile();
                Assert.That(decompiled, Is.Not.Null);
                Assert.That(decompiled.Body, Is.Not.Null);
                
                // The decompiled expression should contain both a conditional and throw
                var visitor = new ThrowExpressionVisitor();
                visitor.Visit(decompiled.Body);
                Assert.That(visitor.HasThrowExpression, Is.True, "Should contain throw expressions");
            });
        }
        
        [Test]
        [TestCase("Should handle simple throw")]
        [TestCase("Should handle throw in conditional")]
        [TestCase("Should handle rethrow")]
        public void ThrowProcessor_Integration_Test(string scenario)
        {
            Action testAction = scenario switch
            {
                "Should handle simple throw" => () => throw new Exception("test"),
                "Should handle throw in conditional" => () => { if (true) throw new Exception("conditional"); },
                "Should handle rethrow" => () => { try { throw new Exception("original"); } catch { throw; } },
                _ => throw new ArgumentException($"Unknown scenario: {scenario}")
            };
            
            // None of these should throw a NotSupportedException anymore
            Assert.DoesNotThrow(() => {
                var decompiled = testAction.Decompile();
                Assert.That(decompiled, Is.Not.Null);
            });
        }
    }
}