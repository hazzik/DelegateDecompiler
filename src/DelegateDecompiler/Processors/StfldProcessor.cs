using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class StfldProcessor : IProcessor
{
    static readonly HashSet<OpCode> StfldOpcodes = new()
    {
        OpCodes.Stfld,
        OpCodes.Stsfld
    };

    public bool Process(ProcessorState state)
    {
        if (StfldOpcodes.Contains(state.Instruction.OpCode))
        {
            var field = (FieldInfo)state.Instruction.Operand;
            var value = state.Stack.Pop();

            if (state.Instruction.OpCode == OpCodes.Stsfld)
            {
                // Static field assignment
                var assignment = Expression.Assign(Expression.Field(null, field), value);
                state.Stack.Push(assignment);
            }
            else
            {
                // Instance field assignment
                var instance = state.Stack.Pop();
                var assignment = BuildAssignment(instance, field, value);
                state.Stack.Push(assignment);
            }
            return true;
        }

        return false;
    }

    static Expression BuildAssignment(Expression instance, FieldInfo field, Expression value)
    {
        var target = instance is Address address ? address[field] : Expression.Field(instance, field);
        return Expression.Assign(target, value);
    }
}