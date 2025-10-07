using System.Collections.Generic;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors
{
    internal static class RegistrationHelpers
    {
        public static void Register(this Dictionary<OpCode, IProcessor> processors, IProcessor processor, params OpCode[] opCodes)
        {
            foreach (var opCode in opCodes)
                processors.Register(processor, opCode);
        }

        public static void Register(this Dictionary<OpCode, IProcessor> processors, IProcessor processor, OpCode opCode)
        {
            processors.Add(opCode, processor);
        }
    }
}
