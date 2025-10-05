using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class ConvertProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Type> ConvertTypes = new()
    {
        { OpCodes.Conv_I, typeof(int) },
        { OpCodes.Conv_I1, typeof(sbyte) },
        { OpCodes.Conv_I2, typeof(short) },
        { OpCodes.Conv_I4, typeof(int) },
        { OpCodes.Conv_I8, typeof(long) },
        { OpCodes.Conv_U, typeof(uint) },
        { OpCodes.Conv_U1, typeof(byte) },
        { OpCodes.Conv_U2, typeof(ushort) },
        { OpCodes.Conv_U4, typeof(uint) },
        { OpCodes.Conv_U8, typeof(ulong) },
    };

    static readonly Dictionary<OpCode, Type> ConvertCheckedTypes = new()
    {
        { OpCodes.Conv_Ovf_I, typeof(int) },
        { OpCodes.Conv_Ovf_I_Un, typeof(int) },
        { OpCodes.Conv_Ovf_I1, typeof(sbyte) },
        { OpCodes.Conv_Ovf_I1_Un, typeof(sbyte) },
        { OpCodes.Conv_Ovf_I2, typeof(short) },
        { OpCodes.Conv_Ovf_I2_Un, typeof(short) },
        { OpCodes.Conv_Ovf_I4, typeof(int) },
        { OpCodes.Conv_Ovf_I4_Un, typeof(int) },
        { OpCodes.Conv_Ovf_I8, typeof(long) },
        { OpCodes.Conv_Ovf_I8_Un, typeof(long) },
        { OpCodes.Conv_Ovf_U, typeof(uint) },
        { OpCodes.Conv_Ovf_U_Un, typeof(uint) },
        { OpCodes.Conv_Ovf_U1, typeof(byte) },
        { OpCodes.Conv_Ovf_U1_Un, typeof(byte) },
        { OpCodes.Conv_Ovf_U2, typeof(ushort) },
        { OpCodes.Conv_Ovf_U2_Un, typeof(ushort) },
        { OpCodes.Conv_Ovf_U4, typeof(uint) },
        { OpCodes.Conv_Ovf_U4_Un, typeof(uint) },
        { OpCodes.Conv_Ovf_U8, typeof(ulong) },
        { OpCodes.Conv_Ovf_U8_Un, typeof(ulong) },
        { OpCodes.Conv_R4, typeof(float) },
        { OpCodes.Conv_R_Un, typeof(float) },
        { OpCodes.Conv_R8, typeof(double) },
    };

    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (ConvertTypes.TryGetValue(instruction.OpCode, out var type))
        {
            var val = state.Stack.Pop();
            state.Stack.Push(Expression.Convert(val, type));
            return true;
        }

        if (ConvertCheckedTypes.TryGetValue(instruction.OpCode, out type))
        {
            var val = state.Stack.Pop();
            state.Stack.Push(Expression.ConvertChecked(val, type));
            return true;
        }

        if (instruction.OpCode == OpCodes.Castclass)
        {
            var val = state.Stack.Pop();
            state.Stack.Push(Expression.Convert(val, (Type)instruction.Operand));
            return true;
        }

        if (instruction.OpCode == OpCodes.Unbox)
        {
            var expression = state.Stack.Pop();
            var targetType = (Type)instruction.Operand;
            // Unbox returns a pointer to the value, but in expression trees we represent this as a conversion
            state.Stack.Push(Expression.Convert(expression, targetType));
            return true;
        }

        if (instruction.OpCode == OpCodes.Unbox_Any)
        {
            var expression = state.Stack.Pop();
            var targetType = (Type)instruction.Operand;
            // Unbox_Any converts boxed value types to their unboxed form or leaves reference types unchanged
            state.Stack.Push(Expression.Convert(expression, targetType));
            return true;
        }

        return false;
    }
}