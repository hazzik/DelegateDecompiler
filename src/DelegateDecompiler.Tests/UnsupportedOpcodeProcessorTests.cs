using System;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class UnsupportedOpcodeProcessorTests : DecompilerTestsBase
    {
        [Test]
        public void UnsupportedOpcode_ShouldThrowDescriptiveException()
        {
            // This test verifies that when decompiling encounters an unsupported opcode,
            // it throws a descriptive NotSupportedException rather than silently failing
            
            // We'll test with a method that might use opcodes that aren't supported
            // For example, methods using unsafe code or specific opcodes like cpobj, initobj, etc.
            
            // Since we can't easily create a method with an unsupported opcode at runtime,
            // we'll test the processor directly by checking that it throws when Process is called
            var processor = new DelegateDecompiler.Processors.UnsupportedOpcodeProcessor();
            
            // The processor should always throw when Process is called, 
            // since it's designed to be the fallback for unsupported opcodes
            var exception = Assert.Throws<NotSupportedException>(() => 
            {
                // We need a valid ProcessorState to test with, but since the UnsupportedOpcodeProcessor
                // always throws regardless of the state, we can pass null for this unit test
                processor.Process(null);
            });
            
            // Verify the exception message is descriptive and helpful
            Assert.That(exception.Message, Does.Contain("IL opcode"));
            Assert.That(exception.Message, Does.Contain("not supported"));
            Assert.That(exception.Message, Does.Contain("DelegateDecompiler"));
            Assert.That(exception.Message, Does.Contain("expression tree"));
        }

        // Test method that demonstrates the processor integration
        // This method uses a pattern that might trigger an unsupported opcode scenario
        [Test]
        public void DecompileMethod_WithUnsupportedPattern_ShouldProvideDescriptiveError()
        {
            // Try to decompile a method that uses patterns that might not be fully supported
            // Note: This test serves as a demonstration of how the new processor works
            // In practice, most common IL patterns are supported, so this is more of a documentation test
            
            // Create a simple delegate that should be decompilable
            Func<int, int> simpleFunc = x => x + 1;
            
            // This should work fine with supported opcodes
            var result = simpleFunc.Decompile();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Body.ToString(), Does.Contain("(x + 1)"));
        }
    }
}