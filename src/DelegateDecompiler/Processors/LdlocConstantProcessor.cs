using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class LdlocConstantProcessor(int index) : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new LdlocConstantProcessor(0), OpCodes.Ldloc_0);
            processors.Register(new LdlocConstantProcessor(1), OpCodes.Ldloc_1);
            processors.Register(new LdlocConstantProcessor(2), OpCodes.Ldloc_2);
            processors.Register(new LdlocConstantProcessor(3), OpCodes.Ldloc_3);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var local = state.Locals[index];
            state.Stack.Push(local.Address);
        }
    }
}
