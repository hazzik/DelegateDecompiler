using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class BoxProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new BoxProcessor(), OpCodes.Box);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        state.Stack.Push(Box(state.Stack.Pop(), (Type)instruction.Operand));
    }

    static Expression Box(Expression expression, Type type)
    {
        if (expression.Type == type)
            return expression;

        if (expression is ConstantExpression constantExpression)
        {
            if (type.IsEnum)
                return Expression.Constant(Enum.ToObject(type, constantExpression.Value));
        }

        if (expression.Type.IsEnum)
            return Expression.Convert(expression, type);

        // Optimize Convert(Convert(int, byte/short), enum) -> Convert(int, enum)
        // This happens when operations like NOT return int, IL converts to byte, then boxes to enum
        if (type.IsEnum && expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            var operand = unary.Operand;
            // Skip intermediate conversions from int to byte/short when boxing to enum
            if (operand.Type == typeof(int) &&
                (unary.Type == typeof(byte) || unary.Type == typeof(sbyte) ||
                 unary.Type == typeof(short) || unary.Type == typeof(ushort)))
            {
                // Box the int directly to the enum
                return Expression.Convert(operand, type);
            }
        }

        return expression;
    }
}