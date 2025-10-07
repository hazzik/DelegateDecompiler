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
        if (constructor.DeclaringType.IsNullableType() && constructor.GetParameters().Length == 1)
        {
            state.Stack.Push(Expression.Convert(arguments[0], constructor.DeclaringType));
        }
        else
        {
            state.Stack.Push(Expression.New(constructor, arguments));
        }
    }
}