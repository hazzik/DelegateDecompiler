using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class ConvertTypeProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new ConvertTypeProcessor(), OpCodes.Castclass, OpCodes.Unbox, OpCodes.Unbox_Any);
        }
    
        public void Process(ProcessorState state, Instruction instruction)
        {
            var address = state.Stack.Pop();
            Expression expression = address;
            var targetType = (Type)instruction.Operand;
            
            // Optimize Convert(Convert(int, byte/short), X) -> Convert(int, X)
            // This happens when operations like NOT return int, IL converts to byte, then casts to enum
            if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                var operand = unary.Operand;
                // Skip intermediate conversions from int to byte/short
                if (operand.Type == typeof(int) &&
                    (unary.Type == typeof(byte) || unary.Type == typeof(sbyte) ||
                     unary.Type == typeof(short) || unary.Type == typeof(ushort)))
                {
                    expression = operand;
                }
            }
            
            state.Stack.Push(Expression.Convert(expression, targetType));
        }
    }
}
