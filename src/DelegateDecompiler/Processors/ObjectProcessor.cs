using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class ObjectProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        switch (state.Instruction.OpCode)
        {
            case OpCodes.Initobj:
                var address = state.Stack.Pop();
                var type = (Type)state.Instruction.Operand;
                address.Expression = ExpressionHelper.Default(type);
                return true;
            case OpCodes.Newarr:
                var operand = (Type)state.Instruction.Operand;
                var expression = state.Stack.Pop();
                if (expression.Expression is ConstantExpression size && (int)size.Value == 0) // optimization
                    state.Stack.Push(Expression.NewArrayInit(operand));
                else
                    state.Stack.Push(Expression.NewArrayBounds(operand, expression));
                return true;
            case OpCodes.Box:
                state.Stack.Push(Processor.Box(state.Stack.Pop(), (Type)state.Instruction.Operand));
                return true;
            default:
                return false;
        }
    }
}