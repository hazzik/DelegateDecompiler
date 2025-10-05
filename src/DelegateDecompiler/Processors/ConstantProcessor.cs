using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class ConstantProcessor(Func<Instruction, object> getValue) : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Add(OpCodes.Ldnull, new ConstantProcessor(_ => null));
        processors.Add(OpCodes.Ldc_I4_0, new ConstantProcessor(_ => 0));
        processors.Add(OpCodes.Ldc_I4_1, new ConstantProcessor(_ => 1));
        processors.Add(OpCodes.Ldc_I4_2, new ConstantProcessor(_ => 2));
        processors.Add(OpCodes.Ldc_I4_3, new ConstantProcessor(_ => 3));
        processors.Add(OpCodes.Ldc_I4_4, new ConstantProcessor(_ => 4));
        processors.Add(OpCodes.Ldc_I4_5, new ConstantProcessor(_ => 5));
        processors.Add(OpCodes.Ldc_I4_6, new ConstantProcessor(_ => 6));
        processors.Add(OpCodes.Ldc_I4_7, new ConstantProcessor(_ => 7));
        processors.Add(OpCodes.Ldc_I4_8, new ConstantProcessor(_ => 8));
        processors.Add(OpCodes.Ldc_I4_M1, new ConstantProcessor(_ => -1));
        processors.Add(OpCodes.Ldc_I4_S, new ConstantProcessor(i => (int)(sbyte)i.Operand));
        processors.Add(OpCodes.Ldc_I4, new ConstantProcessor(FromOperand));
        processors.Add(OpCodes.Ldc_I8, new ConstantProcessor(FromOperand));
        processors.Add(OpCodes.Ldc_R4, new ConstantProcessor(FromOperand));
        processors.Add(OpCodes.Ldc_R8, new ConstantProcessor(FromOperand));
        processors.Add(OpCodes.Ldstr, new ConstantProcessor(FromOperand));
        processors.Add(OpCodes.Ldtoken, new ConstantProcessor(GetRuntimeHandle));
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        state.Stack.Push(Expression.Constant(getValue(instruction)));
    }

    static object FromOperand(Instruction i) => i.Operand;
    
    static object GetRuntimeHandle(Instruction i) => i.Operand switch
    {
        FieldInfo field => field.FieldHandle,
        MethodBase method => method.MethodHandle,
        Type type => type.TypeHandle,
        _ => null
    };
}