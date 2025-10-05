using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class InitObjProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Add(OpCodes.Initobj, new InitObjProcessor());
        }
    
        public void Process(ProcessorState state, Instruction instruction)
        {
            var address = state.Stack.Pop();
            var type = (Type)instruction.Operand;
            address.Expression = ExpressionHelper.Default(type);
        }
    }
}
