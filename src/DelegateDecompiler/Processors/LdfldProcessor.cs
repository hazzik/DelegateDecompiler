using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdfldProcessor : IProcessor
{
    static readonly HashSet<OpCode> LdfldOpcodes = new()
    {
        OpCodes.Ldfld,
        OpCodes.Ldflda,
        OpCodes.Ldsfld
    };

    public bool Process(ProcessorState state)
    {
        if (LdfldOpcodes.Contains(state.Instruction.OpCode))
        {
            var field = (FieldInfo)state.Instruction.Operand;
            
            if (state.Instruction.OpCode == OpCodes.Ldsfld)
            {
                // Static field
                state.Stack.Push(Expression.Field(null, field));
            }
            else
            {
                // Instance field
                var instance = state.Stack.Pop();
                var result = LdFld(instance, field);
                state.Stack.Push(result);
            }
            return true;
        }

        return false;
    }

    static Expression LdFld(Expression instance, FieldInfo field)
    {
        if (instance is Address address)
        {
            return address[field];
        }

        if (IsCachedAnonymousMethodDelegate(instance, field))
        {
            return ((ConstantExpression)instance).Value;
        }

        return Expression.Field(instance, field);
    }

    static bool IsCachedAnonymousMethodDelegate(Expression instance, FieldInfo field)
    {
        return instance is ConstantExpression &&
               field.Name.Contains("CachedAnonymousMethodDelegate");
    }
}