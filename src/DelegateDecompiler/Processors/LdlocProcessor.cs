using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdlocProcessor : IProcessor
{
    static readonly HashSet<OpCode> LdLocOpcodes = new()
    {
        OpCodes.Ldloc_0,
        OpCodes.Ldloc_1,
        OpCodes.Ldloc_2,
        OpCodes.Ldloc_3,
        OpCodes.Ldloc,
        OpCodes.Ldloc_S,
        OpCodes.Ldloca,
        OpCodes.Ldloca_S
    };

    public bool Process(ProcessorState state)
    {
        if (LdLocOpcodes.Contains(state.Instruction.OpCode))
        {
            if (state.Instruction.OpCode == OpCodes.Ldloc_0)
            {
                Processor.LdLoc(state, 0);
            }
            else if (state.Instruction.OpCode == OpCodes.Ldloc_1)
            {
                Processor.LdLoc(state, 1);
            }
            else if (state.Instruction.OpCode == OpCodes.Ldloc_2)
            {
                Processor.LdLoc(state, 2);
            }
            else if (state.Instruction.OpCode == OpCodes.Ldloc_3)
            {
                Processor.LdLoc(state, 3);
            }
            else // Ldloc, Ldloc_S, Ldloca, Ldloca_S
            {
                var operand = (LocalVariableInfo)state.Instruction.Operand;
                Processor.LdLoc(state, operand.LocalIndex);
            }
            return true;
        }

        return false;
    }
}