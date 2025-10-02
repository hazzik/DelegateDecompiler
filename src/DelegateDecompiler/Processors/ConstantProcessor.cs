using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class ConstantProcessor : IProcessor
{
    static readonly HashSet<OpCode> SimpleIntegerConstants = new HashSet<OpCode>
    {
        OpCodes.Ldc_I4_0,
        OpCodes.Ldc_I4_1,
        OpCodes.Ldc_I4_2,
        OpCodes.Ldc_I4_3,
        OpCodes.Ldc_I4_4,
        OpCodes.Ldc_I4_5,
        OpCodes.Ldc_I4_6,
        OpCodes.Ldc_I4_7,
        OpCodes.Ldc_I4_8,
        OpCodes.Ldc_I4_M1
    };

    static readonly Dictionary<OpCode, int> SimpleIntegerValues = new Dictionary<OpCode, int>
    {
        {OpCodes.Ldc_I4_0, 0},
        {OpCodes.Ldc_I4_1, 1},
        {OpCodes.Ldc_I4_2, 2},
        {OpCodes.Ldc_I4_3, 3},
        {OpCodes.Ldc_I4_4, 4},
        {OpCodes.Ldc_I4_5, 5},
        {OpCodes.Ldc_I4_6, 6},
        {OpCodes.Ldc_I4_7, 7},
        {OpCodes.Ldc_I4_8, 8},
        {OpCodes.Ldc_I4_M1, -1}
    };

    public bool Process(ProcessorState state)
    {
        // Handle simple integer constants (0-8, -1)
        if (SimpleIntegerValues.TryGetValue(state.Instruction.OpCode, out int value))
        {
            Processor.LdC(state, value);
            return true;
        }

        // Handle other constant load operations
        if (state.Instruction.OpCode == OpCodes.Ldc_I4_S)
        {
            Processor.LdC(state, (sbyte)state.Instruction.Operand);
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Ldc_I4)
        {
            Processor.LdC(state, (int)state.Instruction.Operand);
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Ldc_I8)
        {
            Processor.LdC(state, (long)state.Instruction.Operand);
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Ldc_R4)
        {
            Processor.LdC(state, (float)state.Instruction.Operand);
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Ldc_R8)
        {
            Processor.LdC(state, (double)state.Instruction.Operand);
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Ldstr)
        {
            state.Stack.Push(Expression.Constant((string)state.Instruction.Operand));
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Ldnull)
        {
            state.Stack.Push(Expression.Constant(null));
            return true;
        }

        return false;
    }
}