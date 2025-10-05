using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class ObjectProcessor : IProcessor
{
    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (instruction.OpCode == OpCodes.Initobj)
        {
            var address = state.Stack.Pop();
            var type = (Type)instruction.Operand;
            address.Expression = ExpressionHelper.Default(type);
            return true;
        }

        if (instruction.OpCode == OpCodes.Newarr)
        {
            var operand = (Type)instruction.Operand;
            var expression = state.Stack.Pop();
            if (expression.Expression is ConstantExpression size && (int)size.Value == 0) // optimization
                state.Stack.Push(Expression.NewArrayInit(operand));
            else
                state.Stack.Push(Expression.NewArrayBounds(operand, expression));
            return true;
        }

        if (instruction.OpCode == OpCodes.Box)
        {
            state.Stack.Push(Processor.Box(state.Stack.Pop(), (Type)instruction.Operand));
            return true;
        }

        return false;
    }
}