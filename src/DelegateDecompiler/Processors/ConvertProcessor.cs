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
        
        // Optimize Convert(Convert(int, byte/short), X) -> Convert(int, X)
        // This happens when operations like NOT return int, IL converts to byte, then to enum
        if (expr is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
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