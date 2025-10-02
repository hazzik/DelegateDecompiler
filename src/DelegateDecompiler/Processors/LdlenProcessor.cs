using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdlenProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        if (state.Instruction.OpCode == OpCodes.Ldlen)
        {
            var array = state.Stack.Pop();
            state.Stack.Push(Expression.ArrayLength(array));
            return true;
        }

        return false;
    }
}