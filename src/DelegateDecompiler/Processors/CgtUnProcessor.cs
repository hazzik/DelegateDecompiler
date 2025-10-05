using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class CgtUnProcessor : IProcessor
{
    public bool Process(ProcessorState state, Instruction instruction)
    {
        // Special handling for Cgt_Un which has special logic for null/zero comparison
        if (instruction.OpCode != OpCodes.Cgt_Un)
            return false;

        var val1 = state.Stack.Pop();
        var val2 = state.Stack.Pop();

        var constantExpression = val1.Expression as ConstantExpression;
        if (constantExpression != null && (constantExpression.Value as int? == 0 || constantExpression.Value == null))
        {
            // Special case for comparison with null/zero
            state.Stack.Push(Processor.MakeBinaryExpression(val2, val1, ExpressionType.NotEqual));
        }
        else
        {
            state.Stack.Push(Processor.MakeBinaryExpression(val2, val1, ExpressionType.GreaterThan));
        }

        return true;
    }
}