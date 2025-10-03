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
    }
}