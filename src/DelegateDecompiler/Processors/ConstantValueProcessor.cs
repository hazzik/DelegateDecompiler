using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class ConstantValueProcessor(object value) : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Add(OpCodes.Ldnull, new ConstantValueProcessor(null));
            processors.Add(OpCodes.Ldc_I4_0, new ConstantValueProcessor(0));
            processors.Add(OpCodes.Ldc_I4_1, new ConstantValueProcessor(1));
            processors.Add(OpCodes.Ldc_I4_2, new ConstantValueProcessor(2));
            processors.Add(OpCodes.Ldc_I4_3, new ConstantValueProcessor(3));
            processors.Add(OpCodes.Ldc_I4_4, new ConstantValueProcessor(4));
            processors.Add(OpCodes.Ldc_I4_5, new ConstantValueProcessor(5));
            processors.Add(OpCodes.Ldc_I4_6, new ConstantValueProcessor(6));
            processors.Add(OpCodes.Ldc_I4_7, new ConstantValueProcessor(7));
            processors.Add(OpCodes.Ldc_I4_8, new ConstantValueProcessor(8));
            processors.Add(OpCodes.Ldc_I4_M1, new ConstantValueProcessor(-1));
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            state.Stack.Push(Expression.Constant(value));
        }
    }
}
