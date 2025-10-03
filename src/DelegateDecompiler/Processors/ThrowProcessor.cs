using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class ThrowProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        if (state.Instruction.OpCode == OpCodes.Throw)
        {
            // OpCodes.Throw pops the exception from the stack and throws it
            var exception = state.Stack.Pop();
            var throwExpression = Expression.Throw(exception, typeof(void));
            state.Stack.Push(throwExpression);
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Rethrow)
        {
            // OpCodes.Rethrow doesn't pop anything from the stack and rethrows the current exception
            var rethrowExpression = Expression.Rethrow(typeof(void));
            state.Stack.Push(rethrowExpression);
            return true;
        }

        return false;
    }
}