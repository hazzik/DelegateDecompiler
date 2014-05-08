using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    class LdArgInstructionProcessor : IInstructionProcessor
    {
        private readonly IList<ParameterExpression> args;

        private readonly IDictionary<OpCode, Func<Instruction, int>> argumentsMap = new Dictionary<OpCode, Func<Instruction, int>>()
        {
            { OpCodes.Ldarg_0, i => 0 },
            { OpCodes.Ldarg_1, i => 1 },
            { OpCodes.Ldarg_2, i => 2 },
            { OpCodes.Ldarg_3, i => 3 },
            { OpCodes.Ldarg_S, i => (short) i.Operand },
            { OpCodes.Ldarg, i => (int) i.Operand },
        };

        public LdArgInstructionProcessor(IList<ParameterExpression> args)
        {
            this.args = args;
        }

        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            Func<Instruction, int> argument;
            if (!argumentsMap.TryGetValue(instruction.OpCode, out argument))
                return false;
            stack.Push(args[argument(instruction)]);
            return true;
        }
    }
}