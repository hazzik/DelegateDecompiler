using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    internal class LdCInstructionProcessor : IInstructionProcessor
    {
        private static readonly IDictionary<OpCode, Func<Instruction, object>> factories = new Dictionary<OpCode, Func<Instruction, object>>
        {
            { OpCodes.Ldc_I4_0, i => 0 },
            { OpCodes.Ldc_I4_1, i => 1 },
            { OpCodes.Ldc_I4_2, i => 2 },
            { OpCodes.Ldc_I4_3, i => 3 },
            { OpCodes.Ldc_I4_4, i => 4 },
            { OpCodes.Ldc_I4_5, i => 5 },
            { OpCodes.Ldc_I4_6, i => 6 },
            { OpCodes.Ldc_I4_7, i => 7 },
            { OpCodes.Ldc_I4_8, i => 8 },
            { OpCodes.Ldc_I4_M1, i => -1 },
            { OpCodes.Ldc_I4_S, i => (int) (sbyte) i.Operand },
            { OpCodes.Ldc_I4, i => (int) i.Operand },
            { OpCodes.Ldc_I8, i => (long) i.Operand },
            { OpCodes.Ldc_R4, i => (float) i.Operand },
            { OpCodes.Ldc_R8, i => (double) i.Operand },
            { OpCodes.Ldstr, i => (string) i.Operand },
            { OpCodes.Ldnull, i => null },
        };

        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            Func<Instruction, object> constant;
            if (!factories.TryGetValue(instruction.OpCode, out constant))
                return false;

            stack.Push(Expression.Constant(constant(instruction)));
            return true;
        }
    }
}
