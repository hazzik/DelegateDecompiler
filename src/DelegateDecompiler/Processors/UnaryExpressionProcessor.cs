using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class UnaryExpressionProcessor(ExpressionType expressionType) : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new UnaryExpressionProcessor(ExpressionType.Negate), OpCodes.Neg);
        processors.Register(new UnaryExpressionProcessor(ExpressionType.Not), OpCodes.Not);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var val = state.Stack.Pop();
        state.Stack.Push(MakeUnaryExpression(val, expressionType));
    }

    static UnaryExpression MakeUnaryExpression(Expression operand, ExpressionType expressionType)
    {
        operand = Processor.ConvertEnumExpressionToUnderlyingType(operand);

        return Expression.MakeUnary(expressionType, operand, operand.Type);
    }
}