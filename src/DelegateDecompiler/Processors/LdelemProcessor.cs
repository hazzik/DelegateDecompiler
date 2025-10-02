using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdelemProcessor : IProcessor
{
    static readonly HashSet<OpCode> LdElemOpcodes = new()
    {
        OpCodes.Ldelem,
        OpCodes.Ldelem_I,
        OpCodes.Ldelem_I1,
        OpCodes.Ldelem_I2,
        OpCodes.Ldelem_I4,
        OpCodes.Ldelem_I8,
        OpCodes.Ldelem_U1,
        OpCodes.Ldelem_U2,
        OpCodes.Ldelem_U4,
        OpCodes.Ldelem_R4,
        OpCodes.Ldelem_R8,
        OpCodes.Ldelem_Ref
    };

    public bool Process(ProcessorState state)
    {
        if (LdElemOpcodes.Contains(state.Instruction.OpCode))
        {
            var index = state.Stack.Pop();
            var array = state.Stack.Pop();
            state.Stack.Push(Expression.ArrayIndex(array, index));
            return true;
        }

        return false;
    }
}