using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DelegateDecompiler.ControlFlow;
using DelegateDecompiler.Processors;
using NUnit.Framework;
using Mono.Reflection;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class UnsupportedOpcodeProcessorTests : DecompilerTestsBase
    {
        static readonly ConstructorInfo InstructionCtor = typeof(Instruction)
            .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(int), typeof(OpCode) }, null);

        [Test]
        public void UnsupportedOpcode_ShouldThrowDescriptiveException()
        {
            Func<int, int> test = x => x;

            var cfg = test.Method.BuildControlFlowGraph();
            var instructionEntry = cfg.Entry;
            var instruction = instructionEntry.Instructions.First();
            var state = new ProcessorState(true, new Stack<Address>(), new VariableInfo[0], new List<Address>());

            var processor = new UnsupportedOpcodeProcessor();

            var exception = Assert.Throws<NotSupportedException>(() => processor.Process(state, instruction));
            Assert.That(exception.Message, Does.Contain(instruction.OpCode.ToString()));
        }

        [Test]
        public void UnsupportedOpcodeProcessor_AlwaysThrows()
        {
            var instruction = CreateInstruction(0, OpCodes.Cpobj);
            var state = new ProcessorState(true, new Stack<Address>(), new VariableInfo[0], new List<Address>());

            var processor = new UnsupportedOpcodeProcessor();

            var exception = Assert.Throws<NotSupportedException>(() => processor.Process(state, instruction));
            Assert.That(exception.Message, Does.Contain("cpobj"));
        }

        static Instruction CreateInstruction(int offset, OpCode opcode)
        {
            var instruction = (Instruction)InstructionCtor.Invoke(new object[] { offset, opcode });
            Assume.That(instruction, Is.Not.Null);
            return instruction;
        }
    }
}