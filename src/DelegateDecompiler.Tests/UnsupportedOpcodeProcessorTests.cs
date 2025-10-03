using System;
using System.Linq;
using System.Reflection.Emit;
using NUnit.Framework;
using Mono.Reflection;

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
            
            var processor = new DelegateDecompiler.Processors.UnsupportedOpcodeProcessor();
            
            // Create a ProcessorState with a minimal setup and an instruction
            // We'll use a direct approach by creating the state from existing patterns
            var stack = new System.Collections.Generic.Stack<Address>();
            var locals = new VariableInfo[0];
            var args = new System.Collections.Generic.List<Address>();
            
            // Create a simple delegate to get a real instruction, then simulate 
            // it being unhandled by the other processors
            Func<int, int> testFunc = x => x + 1;
            var method = testFunc.Method;
            var instructions = method.GetInstructions();
            
            // Get the first instruction - the UnsupportedOpcodeProcessor should always throw
            var testInstruction = instructions.First();
            
            var mockState = new ProcessorState(true, stack, locals, args, testInstruction);
            
            // The processor should always throw when Process is called, 
            // since it's designed to be the fallback for unsupported opcodes
            var exception = Assert.Throws<NotSupportedException>(() => 
            {
                processor.Process(mockState);
            });
            
            // Verify the exception message is descriptive and helpful
            Assert.That(exception.Message, Does.Contain("IL opcode"));
            Assert.That(exception.Message, Does.Contain("not supported"));
            Assert.That(exception.Message, Does.Contain("DelegateDecompiler"));
            Assert.That(exception.Message, Does.Contain("expression tree"));
            
            // Should contain the actual opcode name
            Assert.That(exception.Message, Does.Contain(testInstruction.OpCode.ToString()));
        }

        [Test]
        public void UnsupportedOpcodeProcessor_AlwaysThrows()
        {
            // Simple test to verify the processor behavior without complex setup
            var processor = new DelegateDecompiler.Processors.UnsupportedOpcodeProcessor();
            
            // Create a minimal state with a simple instruction
            var stack = new System.Collections.Generic.Stack<Address>();
            var locals = new VariableInfo[0]; 
            var args = new System.Collections.Generic.List<Address>();
            
            // Create a simple instruction manually using reflection
            var instructionType = typeof(Mono.Reflection.Instruction);
            var constructors = instructionType.GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Mono.Reflection.Instruction instruction = null;
            foreach (var ctor in constructors)
            {
                try
                {
                    var parameters = ctor.GetParameters();
                    if (parameters.Length == 2) // offset, opcode
                    {
                        instruction = (Mono.Reflection.Instruction)ctor.Invoke(new object[] { 0, OpCodes.Cpobj });
                        break;
                    }
                }
                catch
                {
                    // Try next constructor
                }
            }
            
            if (instruction != null)
            {
                var state = new ProcessorState(true, stack, locals, args, instruction);
                var exception = Assert.Throws<NotSupportedException>(() => processor.Process(state));
                Assert.That(exception.Message, Does.Contain("cpobj"));
            }
            else
            {
                Assert.Fail("Could not create instruction for testing UnsupportedOpcodeProcessor");
            }
        }
    }
}