using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class ConvertCheckedProcessor(Type targetType) : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Add(OpCodes.Conv_Ovf_I, new ConvertCheckedProcessor(typeof(int)));
            processors.Add(OpCodes.Conv_Ovf_I_Un, new ConvertCheckedProcessor(typeof(int)));
            processors.Add(OpCodes.Conv_Ovf_I1, new ConvertCheckedProcessor(typeof(sbyte)));
            processors.Add(OpCodes.Conv_Ovf_I1_Un, new ConvertCheckedProcessor(typeof(sbyte)));
            processors.Add(OpCodes.Conv_Ovf_I2, new ConvertCheckedProcessor(typeof(short)));
            processors.Add(OpCodes.Conv_Ovf_I2_Un, new ConvertCheckedProcessor(typeof(short)));
            processors.Add(OpCodes.Conv_Ovf_I4, new ConvertCheckedProcessor(typeof(int)));
            processors.Add(OpCodes.Conv_Ovf_I4_Un, new ConvertCheckedProcessor(typeof(int)));
            processors.Add(OpCodes.Conv_Ovf_I8, new ConvertCheckedProcessor(typeof(long)));
            processors.Add(OpCodes.Conv_Ovf_I8_Un, new ConvertCheckedProcessor(typeof(long)));
            processors.Add(OpCodes.Conv_Ovf_U, new ConvertCheckedProcessor(typeof(uint)));
            processors.Add(OpCodes.Conv_Ovf_U_Un, new ConvertCheckedProcessor(typeof(uint)));
            processors.Add(OpCodes.Conv_Ovf_U1, new ConvertCheckedProcessor(typeof(byte)));
            processors.Add(OpCodes.Conv_Ovf_U1_Un, new ConvertCheckedProcessor(typeof(byte)));
            processors.Add(OpCodes.Conv_Ovf_U2, new ConvertCheckedProcessor(typeof(ushort)));
            processors.Add(OpCodes.Conv_Ovf_U2_Un, new ConvertCheckedProcessor(typeof(ushort)));
            processors.Add(OpCodes.Conv_Ovf_U4, new ConvertCheckedProcessor(typeof(uint)));
            processors.Add(OpCodes.Conv_Ovf_U4_Un, new ConvertCheckedProcessor(typeof(uint)));
            processors.Add(OpCodes.Conv_Ovf_U8, new ConvertCheckedProcessor(typeof(ulong)));
            processors.Add(OpCodes.Conv_Ovf_U8_Un, new ConvertCheckedProcessor(typeof(ulong)));
            processors.Add(OpCodes.Conv_R4, new ConvertCheckedProcessor(typeof(float)));
            processors.Add(OpCodes.Conv_R_Un, new ConvertCheckedProcessor(typeof(float)));
            processors.Add(OpCodes.Conv_R8, new ConvertCheckedProcessor(typeof(double)));
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var val = state.Stack.Pop();
            state.Stack.Push(Expression.ConvertChecked(val, targetType));
        }
    }
}
