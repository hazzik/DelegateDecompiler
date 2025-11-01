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
            processors.Register(new ConvertCheckedProcessor(typeof(int)),
                OpCodes.Conv_Ovf_I, OpCodes.Conv_Ovf_I_Un, OpCodes.Conv_Ovf_I4, OpCodes.Conv_Ovf_I4_Un
            );
            processors.Register(new ConvertCheckedProcessor(typeof(sbyte)),
                OpCodes.Conv_Ovf_I1, OpCodes.Conv_Ovf_I1_Un);
            processors.Register(new ConvertCheckedProcessor(typeof(short)),
                OpCodes.Conv_Ovf_I2, OpCodes.Conv_Ovf_I2_Un);
            processors.Register(new ConvertCheckedProcessor(typeof(long)),
                OpCodes.Conv_Ovf_I8, OpCodes.Conv_Ovf_I8_Un);
            processors.Register(new ConvertCheckedProcessor(typeof(uint)),
                OpCodes.Conv_Ovf_U, OpCodes.Conv_Ovf_U_Un, OpCodes.Conv_Ovf_U4, OpCodes.Conv_Ovf_U4_Un);
            processors.Register(new ConvertCheckedProcessor(typeof(byte)),
                OpCodes.Conv_Ovf_U1, OpCodes.Conv_Ovf_U1_Un);
            processors.Register(new ConvertCheckedProcessor(typeof(ushort)),
                OpCodes.Conv_Ovf_U2, OpCodes.Conv_Ovf_U2_Un);
            processors.Register(new ConvertCheckedProcessor(typeof(ulong)),
                OpCodes.Conv_Ovf_U8, OpCodes.Conv_Ovf_U8_Un);
            processors.Register(new ConvertCheckedProcessor(typeof(float)),
                OpCodes.Conv_R4, OpCodes.Conv_R_Un);
            processors.Register(new ConvertCheckedProcessor(typeof(double)), OpCodes.Conv_R8);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var val = state.Stack.Pop();
            state.Stack.Push(Expression.ConvertChecked(val, targetType));
        }
    }
}
