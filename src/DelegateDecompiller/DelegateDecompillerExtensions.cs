using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiller
{
    public static class DelegateDecompillerExtensions
    {
        public static LambdaExpression Decompile(this Delegate @delegate)
        {
            return Decompile(@delegate.Method);
        }

        public static LambdaExpression Decompile(this MethodBase method)
        {
            var parameters = method.GetParameters();

            var args = parameters
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToList();

            var instructions = method.GetInstructions();
            Expression ex = Expression.Empty();
            var stack = new Stack<Expression>();
            var locals = new Expression[16];
            foreach (var instruction in instructions)
            {
                if (instruction.OpCode == OpCodes.Nop)
                {
                    //do nothing;
                }
                else if (instruction.OpCode == OpCodes.Ldarg_0)
                {
                    stack.Push(args[0]);
                } 
                if (instruction.OpCode == OpCodes.Ldarg_1)
                {
                    stack.Push(args[1]);
                }
                else if (instruction.OpCode == OpCodes.Stloc_0)
                {
                    locals[0] = stack.Pop();
                } 
                else if (instruction.OpCode == OpCodes.Br_S)
                {
                    //do nothing yet
                }
                else if (instruction.OpCode == OpCodes.Ldloc_0)
                {
                    stack.Push(locals[0]);
                }
                else if (instruction.OpCode == OpCodes.Ret)
                {
                    ex = stack.Pop();
                }
                else if (instruction.OpCode == OpCodes.Add)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.Add(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Sub)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.Subtract(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Box)
                {
                    //do nothing for now
                } 
                Console.WriteLine(instruction);
            }

            return Expression.Lambda(ex, args);
        }
    }
}
