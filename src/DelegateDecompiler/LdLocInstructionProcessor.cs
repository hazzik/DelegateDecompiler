using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    class LdLocInstructionProcessor : IInstructionProcessor
    {
        private readonly Expression[] locals;

        private static readonly IDictionary<OpCode, Func<Instruction, int>> indexes = new Dictionary<OpCode, Func<Instruction, int>>
        {
            { OpCodes.Ldloc_0,  i => 0 },
            { OpCodes.Ldloc_1,  i => 1 },
            { OpCodes.Ldloc_2,  i => 2 },
            { OpCodes.Ldloc_3,  i => 3 },
            { OpCodes.Ldloc_S,  i => (byte) i.Operand },
            { OpCodes.Ldloc,    i => (int) i.Operand },
            { OpCodes.Ldloca_S, i => ((LocalVariableInfo) i.Operand).LocalIndex },
            { OpCodes.Ldloca,   i => ((LocalVariableInfo) i.Operand).LocalIndex },
        };

        public LdLocInstructionProcessor(Expression[] locals)
        {
            this.locals = locals;
        }

        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            Func<Instruction, int> index;
            if (!indexes.TryGetValue(instruction.OpCode, out index))
                return false;

            stack.Push(locals[index(instruction)]);
            return true;
        }
    }
}