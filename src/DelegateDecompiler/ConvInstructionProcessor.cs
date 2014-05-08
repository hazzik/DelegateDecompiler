using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    internal class ConvInstructionProcessor : IInstructionProcessor
    {
        private static readonly IDictionary<OpCode, Func<Instruction, Type>> types = new Dictionary<OpCode, Func<Instruction, Type>>
        {
            { OpCodes.Conv_I, i => typeof (int) },
            { OpCodes.Conv_I1, i => typeof (sbyte) },
            { OpCodes.Conv_I2, i => typeof (short) },
            { OpCodes.Conv_I4, i => typeof (int) },
            { OpCodes.Conv_I8, i => typeof (long) },
            { OpCodes.Conv_U, i => typeof (uint) },
            { OpCodes.Conv_U1, i => typeof (byte) },
            { OpCodes.Conv_U2, i => typeof (ushort) },
            { OpCodes.Conv_U4, i => typeof (uint) },
            { OpCodes.Conv_U8, i => typeof (ulong) },
            { OpCodes.Conv_R4, i => typeof (float) },
            { OpCodes.Conv_R_Un, i => typeof (float) },
            { OpCodes.Conv_R8, i => typeof (double) },
            { OpCodes.Castclass, i => (Type) i.Operand },
        };

        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            Func<Instruction, Type> type;
            if (!types.TryGetValue(instruction.OpCode, out type))
                return false;
            var val = stack.Pop();
            stack.Push(Expression.Convert(val, type(instruction)));
            return true;
        }
    }
}
