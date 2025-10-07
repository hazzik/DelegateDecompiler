using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class LdcI4SProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new LdcI4SProcessor(), OpCodes.Ldc_I4_S);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var value = (int)(sbyte)instruction.Operand;
            state.Stack.Push(Expression.Constant(value));
        }
    }
}
