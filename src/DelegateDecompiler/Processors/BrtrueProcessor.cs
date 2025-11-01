using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class BrtrueProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new BrtrueProcessor(), OpCodes.Brtrue, OpCodes.Brtrue_S);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var address = state.Stack.Peek();
        if (address.Expression is MemberExpression memberExpression && 
            Processor.IsCachedAnonymousMethodDelegate(memberExpression.Member as FieldInfo))
        {
            state.Stack.Pop();
            // Always false for cached delegates
            // if (this.<delegate> != null) { this.<delegate> } else { this.<delegate> = <build> }
            var condition = Expression.Constant(false);
            state.Stack.Push(condition);
        }
        else
        {
            var val = state.Stack.Pop();
            var condition = val.Type == typeof(bool) ? val : Expression.NotEqual(val, ExpressionHelper.Default(val.Type));
            state.Stack.Push(condition);
        }
    }
}