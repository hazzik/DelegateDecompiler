using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdlenProcessor : IProcessor
{
    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (instruction.OpCode != OpCodes.Ldlen)
            return false;

        var array = state.Stack.Pop();
        state.Stack.Push(Expression.ArrayLength(array));
        return true;
    }
}