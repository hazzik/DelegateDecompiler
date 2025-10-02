using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ProcessorDecompositionTests
    {
        [Test]
        public void ConvertProcessor_ShouldHandleBasicConversions()
        {
            var processor = new ConvertProcessor();
            var state = CreateMockState(OpCodes.Conv_I4);
            
            Assert.IsTrue(processor.Process(state));
        }

        [Test]
        public void ArithmeticProcessor_ShouldHandleAddition()
        {
            var processor = new ArithmeticProcessor();
            var state = CreateMockState(OpCodes.Add);
            
            Assert.IsTrue(processor.Process(state));
        }

        [Test]
        public void BitwiseProcessor_ShouldHandleAnd()
        {
            var processor = new BitwiseProcessor();
            var state = CreateMockState(OpCodes.And);
            
            Assert.IsTrue(processor.Process(state));
        }

        [Test]
        public void UnaryProcessor_ShouldHandleNegation()
        {
            var processor = new UnaryProcessor();
            var state = CreateMockState(OpCodes.Neg);
            
            Assert.IsTrue(processor.Process(state));
        }

        [Test]
        public void Processors_ShouldReturnFalseForUnsupportedOpcodes()
        {
            var processors = new IProcessor[]
            {
                new ConvertProcessor(),
                new ArithmeticProcessor(),
                new BitwiseProcessor(),
                new UnaryProcessor()
            };

            var state = CreateMockState(OpCodes.Nop); // Unsupported by all processors

            foreach (var processor in processors)
            {
                Assert.IsFalse(processor.Process(state));
            }
        }

        private ProcessorState CreateMockState(OpCode opCode)
        {
            var instruction = new MockInstruction { OpCode = opCode };
            var stack = new Stack<Address>();
            
            // Add some mock addresses to the stack for operations that need them
            stack.Push(Expression.Constant(1));
            stack.Push(Expression.Constant(2));
            
            return new ProcessorState(stack, new VariableInfo[0], new Address[0], instruction);
        }

        private class MockInstruction : Instruction
        {
            public override OpCode OpCode { get; set; }
            public override object Operand { get; set; }
            public override Instruction Next { get; set; }
            public override Instruction Previous { get; set; }
            public override int Offset { get; set; }
        }
    }
}