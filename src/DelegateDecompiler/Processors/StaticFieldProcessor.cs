using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace DelegateDecompiler.Processors
{
    class StaticFieldProcessor : IProcessor
    {
        const string cachedAnonymousMethodDelegate = "CS$<>9__CachedAnonymousMethodDelegate";
        const string cachedAnonymousMethodDelegateRoslyn = "<>9__";

        public bool Process(ProcessorState state)
        {
            if (state.Instruction.OpCode == OpCodes.Ldsfld)
            {
                var field = (FieldInfo) state.Instruction.Operand;
                if (IsCachedAnonymousMethodDelegate(field))
                {
                    Address address;
                    if (state.Delegates.TryGetValue(field, out address))
                        state.Stack.Push(address);
                    else
                        state.Stack.Push(Expression.Field(null, field));
                }
                else
                {
                    state.Stack.Push(Expression.Field(null, field));
                }
            }
            else if (state.Instruction.OpCode == OpCodes.Stsfld)
            {
                var field = (FieldInfo) state.Instruction.Operand;
                if (IsCachedAnonymousMethodDelegate(field))
                {
                    state.Delegates[field] = state.Stack.Pop();
                }
                else
                {
                    var pop = state.Stack.Pop();
                    state.Stack.Push(Expression.Assign(Expression.Field(null, field), pop));
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        internal static bool IsCachedAnonymousMethodDelegate(FieldInfo field)
        {
            if (field == null) return false;
            return field.Name.StartsWith(cachedAnonymousMethodDelegate) && Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute), false) ||
                   field.Name.StartsWith(cachedAnonymousMethodDelegateRoslyn) && field.DeclaringType != null && Attribute.IsDefined(field.DeclaringType, typeof(CompilerGeneratedAttribute), false);
        }
    }
}