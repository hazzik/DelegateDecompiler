using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdargProcessor : IProcessor
{
    static readonly HashSet<OpCode> LdArgOpcodes = new()
    {
        OpCodes.Ldarg_0,
        OpCodes.Ldarg_1,
        OpCodes.Ldarg_2,
        OpCodes.Ldarg_3,
        OpCodes.Ldarg_S,
        OpCodes.Ldarg,
        OpCodes.Ldarga,
        OpCodes.Ldarga_S
    };

    public bool Process(ProcessorState state)
    {
        if (LdArgOpcodes.Contains(state.Instruction.OpCode))
        {
            if (state.Instruction.OpCode == OpCodes.Ldarg_0)
            {
                Processor.LdArg(state, 0);
            }
            else if (state.Instruction.OpCode == OpCodes.Ldarg_1)
            {
                Processor.LdArg(state, 1);
            }
            else if (state.Instruction.OpCode == OpCodes.Ldarg_2)
            {
                Processor.LdArg(state, 2);
            }
            else if (state.Instruction.OpCode == OpCodes.Ldarg_3)
            {
                Processor.LdArg(state, 3);
            }
            else // Ldarg_S, Ldarg, Ldarga, Ldarga_S
            {
                var operand = (ParameterInfo)state.Instruction.Operand;
                state.Stack.Push(state.Args.Single(x => ((ParameterExpression)x.Expression).Name == operand.Name));
            }
            return true;
        }

        return false;
    }
}