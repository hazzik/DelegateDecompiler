using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class ThrowProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        if (state.Instruction.OpCode == OpCodes.Throw)
        {
            var exception = state.Stack.Pop();
            state.Stack.Push(Expression.Throw(exception));
            return true;
        }

        return false;
    }
}