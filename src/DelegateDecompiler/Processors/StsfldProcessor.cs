using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class StsfldProcessor : IProcessor
    {
        public bool Process(ProcessorState state, Instruction instruction)
        {
            if (instruction.OpCode != OpCodes.Stsfld)
                return false;

            var value = state.Stack.Pop();
            var field = (FieldInfo)instruction.Operand;
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
