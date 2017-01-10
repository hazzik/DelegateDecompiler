using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors
{
    class ConstantProcessor : IProcessor
    {
        static readonly Dictionary<OpCode, object> Values = new Dictionary<OpCode, object>
        {
            {OpCodes.Ldc_I4_M1, -1},
            {OpCodes.Ldc_I4_0, 0},
            {OpCodes.Ldc_I4_1, 1},
            {OpCodes.Ldc_I4_2, 2},
            {OpCodes.Ldc_I4_3, 3},
            {OpCodes.Ldc_I4_4, 4},
            {OpCodes.Ldc_I4_5, 5},
            {OpCodes.Ldc_I4_6, 6},
            {OpCodes.Ldc_I4_7, 7},
            {OpCodes.Ldc_I4_8, 8},
            {OpCodes.Ldnull, null}
        };

        public bool Process(ProcessorState state)
        {
            object i;
            if (state.Instruction.OpCode == OpCodes.Ldc_I4 ||
                state.Instruction.OpCode == OpCodes.Ldc_I8 ||
                state.Instruction.OpCode == OpCodes.Ldc_R4 ||
                state.Instruction.OpCode == OpCodes.Ldc_R8 ||
                state.Instruction.OpCode == OpCodes.Ldstr)
            {
                state.Stack.Push(Expression.Constant(state.Instruction.Operand));
            }
            else if (state.Instruction.OpCode == OpCodes.Ldc_I4_S)
            {
                state.Stack.Push(Expression.Constant(Convert.ToInt32(state.Instruction.Operand)));
            }
            else if (Values.TryGetValue(state.Instruction.OpCode, out i))
            {
                state.Stack.Push(Expression.Constant(i));
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}