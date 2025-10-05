using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class LdtokenProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Add(OpCodes.Ldtoken, new LdtokenProcessor());
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var value = GetRuntimeHandle(instruction);
            state.Stack.Push(Expression.Constant(value));
        }

        static object GetRuntimeHandle(Instruction instruction) => instruction.Operand switch
        {
            FieldInfo field => field.FieldHandle,
            MethodBase method => method.MethodHandle,
            Type type => type.TypeHandle,
            _ => null
        };
    }
}
