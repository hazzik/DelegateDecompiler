using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    class StLocInstructionProcessor : IInstructionProcessor
    {
        private readonly Expression[] locals;

        private readonly IDictionary<OpCode, Func<Instruction, int>> indexes = new Dictionary<OpCode, Func<Instruction, int>>()
        {
            { OpCodes.Stloc_0, i => 0 },
            { OpCodes.Stloc_1, i => 1 },
            { OpCodes.Stloc_2, i => 2 },
            { OpCodes.Stloc_3, i => 3 },
            { OpCodes.Stloc_S, i => (byte) i.Operand },
            { OpCodes.Stloc, i => (int) i.Operand },
        };

        public StLocInstructionProcessor(Expression[] locals)
        {
            this.locals = locals;
        }

        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            Func<Instruction, int> index;
            if (!indexes.TryGetValue(instruction.OpCode, out index))
                return false;
            locals[index(instruction)] = stack.Pop();
            return true;
        }
    }
}