using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class LdargConstantProcessor(int index) : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Add(OpCodes.Ldarg_0, new LdargConstantProcessor(0));
            processors.Add(OpCodes.Ldarg_1, new LdargConstantProcessor(1));
            processors.Add(OpCodes.Ldarg_2, new LdargConstantProcessor(2));
            processors.Add(OpCodes.Ldarg_3, new LdargConstantProcessor(3));
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            state.Stack.Push(state.Args[index]);
        }
    }
}
