using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class StoreProcessor : IProcessor
{
    static readonly HashSet<OpCode> StLocOpcodes = new HashSet<OpCode>
    {
        OpCodes.Stloc_0,
        OpCodes.Stloc_1,
        OpCodes.Stloc_2,
        OpCodes.Stloc_3,
        OpCodes.Stloc_S,
        OpCodes.Stloc
    };

    public bool Process(ProcessorState state)
    {
        // Handle Stloc operations
        if (StLocOpcodes.Contains(state.Instruction.OpCode))
        {
            if (state.Instruction.OpCode == OpCodes.Stloc_0)
            {
                Processor.StLoc(state, 0);
            }
            else if (state.Instruction.OpCode == OpCodes.Stloc_1)
            {
                Processor.StLoc(state, 1);
            }
            else if (state.Instruction.OpCode == OpCodes.Stloc_2)
            {
                Processor.StLoc(state, 2);
            }
            else if (state.Instruction.OpCode == OpCodes.Stloc_3)
            {
                Processor.StLoc(state, 3);
            }
            else // Stloc_S, Stloc
            {
                var operand = (LocalVariableInfo)state.Instruction.Operand;
                Processor.StLoc(state, operand.LocalIndex);
            }
            return true;
        }

        return false;
    }
}