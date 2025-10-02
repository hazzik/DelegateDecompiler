using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class StelemProcessor : IProcessor
{
    static readonly HashSet<OpCode> Operations = new()
    {
        OpCodes.Stelem,
        OpCodes.Stelem_I,
        OpCodes.Stelem_I1,
        OpCodes.Stelem_I2,
        OpCodes.Stelem_I4,
        OpCodes.Stelem_I8,
        OpCodes.Stelem_R4,
        OpCodes.Stelem_R8,
        OpCodes.Stelem_Ref
    };

    public bool Process(ProcessorState state)
    {
        if (!Operations.Contains(state.Instruction.OpCode))
            return false;

        StElem(state);
        return true;
    }

    static void StElem(ProcessorState state)
    {
        var value = state.Stack.Pop();
        var index = state.Stack.Pop();
        var array = state.Stack.Pop();

        if (array.Expression is NewArrayExpression newArray)
        {
            var elementType = array.Type.GetElementType();
            var expressions = CreateArrayInitExpressions(elementType, newArray, value, index);
            array.Expression = Expression.NewArrayInit(elementType, expressions);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    static IEnumerable<Expression> CreateArrayInitExpressions(
        Type elementType, NewArrayExpression newArray, Expression valueExpression, Expression indexExpression)
    {
        var indexGetter = (Func<int>)Expression.Lambda(indexExpression).Compile();
        var index = indexGetter();

        Expression[] expressions;
        if (newArray.NodeType == ExpressionType.NewArrayInit)
        {
            expressions = newArray.Expressions.ToArray();
            if (index >= newArray.Expressions.Count)
            {
                Array.Resize(ref expressions, index + 1);
            }

        }
        else if (newArray.NodeType == ExpressionType.NewArrayBounds)
        {
            var sizeExpression = newArray.Expressions.Single();
            var sizeGetter = (Func<int>)Expression.Lambda(sizeExpression).Compile();
            var getter = sizeGetter();

            expressions = Enumerable.Repeat(ExpressionHelper.Default(elementType), getter).ToArray();
        }
        else
        {
            throw new NotSupportedException();
        }

        expressions[index] = Processor.AdjustType(valueExpression, elementType);
        return expressions;
    }
}