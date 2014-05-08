using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    internal class LdArgInstructionProcessor : IInstructionProcessor
    {
        private readonly IList<ParameterExpression> args;

        private static readonly IDictionary<OpCode, Func<IList<ParameterExpression>, Instruction, ParameterExpression>> indexes = new Dictionary<OpCode, Func<IList<ParameterExpression>, Instruction, ParameterExpression>>
        {
            { OpCodes.Ldarg_0, (args, i) => args[0] },
            { OpCodes.Ldarg_1, (args, i) => args[1] },
            { OpCodes.Ldarg_2, (args, i) => args[2] },
            { OpCodes.Ldarg_3, (args, i) => args[3] },
            { OpCodes.Ldarg_S, (args, i) => args[(byte) i.Operand] },
            { OpCodes.Ldarg, (args, i) => args[(int) i.Operand] },
            { OpCodes.Ldarga_S, (args, i) => args.Single(x => x.Name == ((ParameterInfo) i.Operand).Name) },
            { OpCodes.Ldarga, (args, i) => args.Single(x => x.Name == ((ParameterInfo) i.Operand).Name) },
        };

        public LdArgInstructionProcessor(IList<ParameterExpression> args)
        {
            this.args = args;
        }

        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            Func<IList<ParameterExpression>, Instruction, ParameterExpression> index;
            if (!indexes.TryGetValue(instruction.OpCode, out index))
                return false;
            stack.Push(index(args, instruction));
            return true;
        }
    }
}
