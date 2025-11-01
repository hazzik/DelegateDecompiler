using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class NewobjProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new NewobjProcessor(), OpCodes.Newobj);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var constructor = (ConstructorInfo)instruction.Operand;
        var arguments = Processor.GetArguments(state, constructor);
        
        // No special handling needed here - OptimizeExpressionVisitor converts
        // new Nullable<T>(value) to Convert(value, Nullable<T>)
        state.Stack.Push(Expression.New(constructor, arguments));
    }
}