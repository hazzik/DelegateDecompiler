using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class ConvertProcessor(Type targetType) : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Add(OpCodes.Conv_I, new ConvertProcessor(typeof(int)));
        processors.Add(OpCodes.Conv_I1, new ConvertProcessor(typeof(sbyte)));
        processors.Add(OpCodes.Conv_I2, new ConvertProcessor(typeof(short)));
        processors.Add(OpCodes.Conv_I4, new ConvertProcessor(typeof(int)));
        processors.Add(OpCodes.Conv_I8, new ConvertProcessor(typeof(long)));
        processors.Add(OpCodes.Conv_U, new ConvertProcessor(typeof(uint)));
        processors.Add(OpCodes.Conv_U1, new ConvertProcessor(typeof(byte)));
        processors.Add(OpCodes.Conv_U2, new ConvertProcessor(typeof(ushort)));
        processors.Add(OpCodes.Conv_U4, new ConvertProcessor(typeof(uint)));
        processors.Add(OpCodes.Conv_U8, new ConvertProcessor(typeof(ulong)));
    }
    
    public void Process(ProcessorState state, Instruction instruction)
    {
        var val = state.Stack.Pop();
        state.Stack.Push(Expression.Convert(val, targetType));
    }
}