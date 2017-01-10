using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors
{
    class ConvertProcessor
    {
        static readonly Dictionary<OpCode, Type> Types = new Dictionary<OpCode, Type>
        {
            {OpCodes.Conv_I, typeof(int)},
            {OpCodes.Conv_I1, typeof(sbyte)},
            {OpCodes.Conv_I2, typeof(short)},
            {OpCodes.Conv_I4, typeof(int)},
            {OpCodes.Conv_I8, typeof(long)},
            {OpCodes.Conv_U, typeof(uint)},
            {OpCodes.Conv_U1, typeof(byte)},
            {OpCodes.Conv_U2, typeof(ushort)},
            {OpCodes.Conv_U4, typeof(uint)},
            {OpCodes.Conv_U8, typeof(ulong)},
            {OpCodes.Conv_R4, typeof(float)},
            {OpCodes.Conv_R_Un, typeof(float)},
            {OpCodes.Conv_R8, typeof(double)},
        };

        public bool Process(ProcessorState state)
        {
            Type type;
            if (Types.TryGetValue(state.Instruction.OpCode, out type))
            {
                var val = state.Stack.Pop();
                state.Stack.Push(Expression.Convert(val, type));
                return true;
            }
            return false;
        }
    }
}