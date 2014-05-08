using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    internal class ConvOvfInstructionProcessor : IInstructionProcessor
    {
        private static readonly IDictionary<OpCode, Type> types = new Dictionary<OpCode, Type>
        {
            { OpCodes.Conv_Ovf_I, typeof (int) },
            { OpCodes.Conv_Ovf_I_Un, typeof (int) },
            { OpCodes.Conv_Ovf_I1, typeof (sbyte) },
            { OpCodes.Conv_Ovf_I1_Un, typeof (sbyte) },
            { OpCodes.Conv_Ovf_I2, typeof (short) },
            { OpCodes.Conv_Ovf_I2_Un, typeof (short) },
            { OpCodes.Conv_Ovf_I4, typeof (int) },
            { OpCodes.Conv_Ovf_I4_Un, typeof (int) },
            { OpCodes.Conv_Ovf_I8, typeof (long) },
            { OpCodes.Conv_Ovf_I8_Un, typeof (long) },
            { OpCodes.Conv_Ovf_U, typeof (uint) },
            { OpCodes.Conv_Ovf_U_Un, typeof (uint) },
            { OpCodes.Conv_Ovf_U1, typeof (byte) },
            { OpCodes.Conv_Ovf_U1_Un, typeof (byte) },
            { OpCodes.Conv_Ovf_U2, typeof (ushort) },
            { OpCodes.Conv_Ovf_U2_Un, typeof (ushort) },
            { OpCodes.Conv_Ovf_U4, typeof (uint) },
            { OpCodes.Conv_Ovf_U4_Un, typeof (uint) },
            { OpCodes.Conv_Ovf_U8, typeof (ulong) },
            { OpCodes.Conv_Ovf_U8_Un, typeof (ulong) },
        };

        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            Type type;
            if (!types.TryGetValue(instruction.OpCode, out type))
                return false;
            var val = stack.Pop();
            stack.Push(Expression.ConvertChecked(val, type));
            return true;
        }
    }
}
