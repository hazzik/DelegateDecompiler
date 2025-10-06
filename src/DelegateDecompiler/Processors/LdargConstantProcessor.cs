using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class LdargConstantProcessor(int index) : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new LdargConstantProcessor(0), OpCodes.Ldarg_0);
            processors.Register(new LdargConstantProcessor(1), OpCodes.Ldarg_1);
            processors.Register(new LdargConstantProcessor(2), OpCodes.Ldarg_2);
            processors.Register(new LdargConstantProcessor(3), OpCodes.Ldarg_3);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            state.Stack.Push(state.Args[index]);
        }
    }
}
