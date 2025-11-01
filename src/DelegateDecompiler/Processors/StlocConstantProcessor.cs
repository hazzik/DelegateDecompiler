using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class StlocConstantProcessor(int index) : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new StlocConstantProcessor(0), OpCodes.Stloc_0);
            processors.Register(new StlocConstantProcessor(1), OpCodes.Stloc_1);
            processors.Register(new StlocConstantProcessor(2), OpCodes.Stloc_2);
            processors.Register(new StlocConstantProcessor(3), OpCodes.Stloc_3);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var info = state.Locals[index];
            var expression = Processor.AdjustType(state.Stack.Pop(), info.Type);
            info.Address = expression.Type == info.Type ? expression : Expression.Convert(expression, info.Type);
        }
    }
}
