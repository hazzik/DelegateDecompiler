using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using DelegateDecompiler.Processors;
using NUnit.Framework;
using Mono.Reflection;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ThrowProcessorTests : DecompilerTestsBase
    {
        static readonly ConstructorInfo InstructionCtor = typeof(Instruction)
            .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(int), typeof(OpCode) }, null);

        [Test]
        public void ThrowProcessor_HandlesThrowOpCode()
        {
            var instruction = CreateInstruction(0, OpCodes.Throw);
            var stack = new Stack<Address>();
            
            // Push an exception expression onto the stack
            var exceptionExpression = Expression.Constant(new ArgumentException("test"));
            stack.Push(exceptionExpression); // Implicit conversion to Address
            
            var state = new ProcessorState(true, stack, new VariableInfo[0], new List<Address>(), instruction);
            var processor = new ThrowProcessor();

            var result = processor.Process(state);

            Assert.That(result, Is.True);
            Assert.That(state.Stack.Count, Is.EqualTo(1));
            
            var throwExpression = state.Stack.Pop().Expression;
            Assert.That(throwExpression.NodeType, Is.EqualTo(ExpressionType.Throw));
            Assert.That(throwExpression, Is.TypeOf<UnaryExpression>());
            
            var unaryExpression = (UnaryExpression)throwExpression;
            Assert.That(unaryExpression.Operand, Is.EqualTo(exceptionExpression));
            Assert.That(unaryExpression.Type, Is.EqualTo(typeof(void))); // Throw returns void
        }

        [Test]
        public void ThrowProcessor_HandlesRethrowOpCode()
        {
            var instruction = CreateInstruction(0, OpCodes.Rethrow);
            var stack = new Stack<Address>();
            
            var state = new ProcessorState(true, stack, new VariableInfo[0], new List<Address>(), instruction);
            var processor = new ThrowProcessor();

            var result = processor.Process(state);

            Assert.That(result, Is.True);
            Assert.That(state.Stack.Count, Is.EqualTo(1));
            
            var rethrowExpression = state.Stack.Pop().Expression;
            Assert.That(rethrowExpression.NodeType, Is.EqualTo(ExpressionType.Throw));
            Assert.That(rethrowExpression, Is.TypeOf<UnaryExpression>());
            
            var unaryExpression = (UnaryExpression)rethrowExpression;
            Assert.That(unaryExpression.Operand, Is.Null); // Rethrow has no operand
            Assert.That(unaryExpression.Type, Is.EqualTo(typeof(void))); // Rethrow returns void
        }

        [Test]
        public void ThrowProcessor_IgnoresOtherOpCodes()
        {
            var instruction = CreateInstruction(0, OpCodes.Add);
            var state = new ProcessorState(true, new Stack<Address>(), new VariableInfo[0], new List<Address>(), instruction);
            var processor = new ThrowProcessor();

            var result = processor.Process(state);

            Assert.That(result, Is.False);
        }

        static Instruction CreateInstruction(int offset, OpCode opcode)
        {
            var instruction = (Instruction)InstructionCtor.Invoke(new object[] { offset, opcode });
            Assume.That(instruction, Is.Not.Null);
            return instruction;
        }
    }
}