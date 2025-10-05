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
            processors.Add(OpCodes.Stloc_0, new StlocConstantProcessor(0));
            processors.Add(OpCodes.Stloc_1, new StlocConstantProcessor(1));
            processors.Add(OpCodes.Stloc_2, new StlocConstantProcessor(2));
            processors.Add(OpCodes.Stloc_3, new StlocConstantProcessor(3));
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var info = state.Locals[index];
            var expression = Processor.AdjustType(state.Stack.Pop(), info.Type);
            info.Address = expression.Type == info.Type ? expression : Expression.Convert(expression, info.Type);
        }
    }
}
