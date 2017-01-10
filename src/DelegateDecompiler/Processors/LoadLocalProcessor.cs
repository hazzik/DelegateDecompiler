using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    class LoadLocalProcessor : IProcessor
    {
        public bool Process(ProcessorState state)
        {
            int index;
            if (!TryGetLocalIndex(state.Instruction, out index)) return false;

            state.Stack.Push(state.Locals[index].Address);

            return true;
        }

        static bool TryGetLocalIndex(Instruction instruction, out int index)
        {
            if (instruction.OpCode == OpCodes.Ldloc_0)
            {
                index = 0;
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldloc_1)
            {
                index = 1;
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldloc_2)
            {
                index = 2;
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldloc_3)
            {
                index = 3;
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldloc ||
                instruction.OpCode == OpCodes.Ldloc_S ||
                instruction.OpCode == OpCodes.Ldloca ||
                instruction.OpCode == OpCodes.Ldloca_S)
            {
                var operand = (LocalVariableInfo) instruction.Operand;
                index = operand.LocalIndex;
                return true;
            }
            index = -1;
            return false;
        }
    }
}