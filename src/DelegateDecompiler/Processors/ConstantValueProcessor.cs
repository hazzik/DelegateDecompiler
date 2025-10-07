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
            processors.Register(new ConstantValueProcessor(null), OpCodes.Ldnull);
            processors.Register(new ConstantValueProcessor(-1), OpCodes.Ldc_I4_M1);
            processors.Register(new ConstantValueProcessor(0), OpCodes.Ldc_I4_0);
            processors.Register(new ConstantValueProcessor(1), OpCodes.Ldc_I4_1);
            processors.Register(new ConstantValueProcessor(2), OpCodes.Ldc_I4_2);
            processors.Register(new ConstantValueProcessor(3), OpCodes.Ldc_I4_3);
            processors.Register(new ConstantValueProcessor(4), OpCodes.Ldc_I4_4);
            processors.Register(new ConstantValueProcessor(5), OpCodes.Ldc_I4_5);
            processors.Register(new ConstantValueProcessor(6), OpCodes.Ldc_I4_6);
            processors.Register(new ConstantValueProcessor(7), OpCodes.Ldc_I4_7);
            processors.Register(new ConstantValueProcessor(8), OpCodes.Ldc_I4_8);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            state.Stack.Push(Expression.Constant(value));
        }
    }
}
