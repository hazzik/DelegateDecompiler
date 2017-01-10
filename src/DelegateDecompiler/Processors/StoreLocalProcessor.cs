using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    class StoreLocalProcessor : IProcessor
    {
        public bool Process(ProcessorState state)
        {
            int index;
            if (!TryGetLocalIndex(state.Instruction, out index)) return false;

            var info = state.Locals[index];
            info.Address = Processor.AdjustType(state.Stack.Pop(), info.Type);
            return true;
        }

        static bool TryGetLocalIndex(Instruction opcode, out int index)
        {
            if (opcode.OpCode == OpCodes.Stloc_0)
            {
                index = 0;
            return true;
            }
            if (opcode.OpCode == OpCodes.Stloc_1)
            {
                index = 1;
                return true;
            }
            if (opcode.OpCode == OpCodes.Stloc_2)
            {
                index = 2;
                return true;
            }
            if (opcode.OpCode == OpCodes.Stloc_3)
            {
                index = 3;
                return true;
            }
            if (opcode.OpCode == OpCodes.Stloc_S ||
                opcode.OpCode == OpCodes.Stloc)
            {
                var operand = (LocalVariableInfo)opcode.Operand;
                index = operand.LocalIndex;
                return true;
            }
            index = -1;
            return false;
        }
    }
}