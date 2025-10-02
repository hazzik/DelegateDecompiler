using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class StelemProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Type> StelemTypes = new()
    {
        { OpCodes.Stelem_I, typeof(IntPtr) },
        { OpCodes.Stelem_I1, typeof(sbyte) },
        { OpCodes.Stelem_I2, typeof(short) },
        { OpCodes.Stelem_I4, typeof(int) },
        { OpCodes.Stelem_I8, typeof(long) },
        { OpCodes.Stelem_R4, typeof(float) },
        { OpCodes.Stelem_R8, typeof(double) },
        { OpCodes.Stelem_Ref, typeof(object) }
    };

    public bool Process(ProcessorState state)
    {
        if (state.Instruction.OpCode == OpCodes.Stelem)
        {
            var type = (Type)state.Instruction.Operand;
            StElem(state, type);
            return true;
        }

        if (StelemTypes.TryGetValue(state.Instruction.OpCode, out var stelemType))
        {
            StElem(state, stelemType);
            return true;
        }

        return false;
    }

    static void StElem(ProcessorState state, Type type)
    {
        var value = state.Stack.Pop();
        var index = state.Stack.Pop();
        var array = state.Stack.Pop();

        var arrayAccess = Expression.ArrayAccess(array, index);
        var assignment = Expression.Assign(arrayAccess, Expression.Convert(value, type));
        state.Stack.Push(assignment);
    }
}