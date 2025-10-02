using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors
{
    internal class StsfldProcessor : IProcessor
    {
        public bool Process(ProcessorState state)
        {
            if (state.Instruction.OpCode != OpCodes.Stsfld)
                return false;

            var value = state.Stack.Pop();
            var field = (FieldInfo)state.Instruction.Operand;
            if (Processor.IsCachedAnonymousMethodDelegate(field))
            {
                state.Delegates[Tuple.Create(default(Address), field)] = value;
            }
            else
            {
                state.Stack.Push(Expression.Assign(Expression.Field(null, field), value));
            }

            return true;
        }
    }
}
