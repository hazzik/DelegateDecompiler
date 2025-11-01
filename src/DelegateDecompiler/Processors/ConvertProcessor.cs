using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class ConvertProcessor(Type targetType) : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new ConvertProcessor(typeof(int)), OpCodes.Conv_I, OpCodes.Conv_I4);
        processors.Register(new ConvertProcessor(typeof(sbyte)), OpCodes.Conv_I1);
        processors.Register(new ConvertProcessor(typeof(short)), OpCodes.Conv_I2);
        processors.Register(new ConvertProcessor(typeof(long)), OpCodes.Conv_I8);
        processors.Register(new ConvertProcessor(typeof(uint)), OpCodes.Conv_U, OpCodes.Conv_U4);
        processors.Register(new ConvertProcessor(typeof(byte)), OpCodes.Conv_U1);
        processors.Register(new ConvertProcessor(typeof(ushort)), OpCodes.Conv_U2);
        processors.Register(new ConvertProcessor(typeof(ulong)), OpCodes.Conv_U8);
    }
    
    public void Process(ProcessorState state, Instruction instruction)
    {
        var val = state.Stack.Pop();
        Expression expr = val;
        
        // Optimize Convert(Convert(X, intermediate), ulong/long) -> Convert(X, long)
        // This happens when byte enum is cast to long: enum -> byte -> ulong -> long
        // We want to skip all intermediate conversions and go straight from enum to long
        if ((targetType == typeof(long) || targetType == typeof(ulong)) &&
            expr is UnaryExpression outerUnary &&
            outerUnary.NodeType == ExpressionType.Convert)
        {
            // Traverse down the conversion chain to find the original expression
            Expression current = outerUnary.Operand;
            while (current is UnaryExpression unaryChain && unaryChain.NodeType == ExpressionType.Convert)
            {
                // If we find an enum at any level, use it
                if (unaryChain.Operand.Type.IsEnum)
                {
                    expr = unaryChain.Operand;
                    break;
                }
                current = unaryChain.Operand;
            }
            // Also check if the final expression is an enum
            if (current.Type.IsEnum)
            {
                expr = current;
            }
        }
        // Optimize Convert(Convert(int, byte/short), X) -> Convert(int, X)
        // This happens when operations like NOT return int, IL converts to byte, then to enum
        else if (expr is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            var operand = unary.Operand;
            // Skip intermediate conversions from int to byte/short when going to another type
            if (operand.Type == typeof(int) &&
                (unary.Type == typeof(byte) || unary.Type == typeof(sbyte) ||
                 unary.Type == typeof(short) || unary.Type == typeof(ushort)))
            {
                // Use the int directly, skip the intermediate byte/short conversion
                expr = operand;
            }
        }
        
        state.Stack.Push(Expression.Convert(expr, targetType));
    }
}