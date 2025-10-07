using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class DupProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new DupProcessor(), OpCodes.Dup);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            state.Stack.Push(state.Stack.Peek());
        }
    }
}
