using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DelegateDecompiler.ControlFlow;
using Mono.Reflection;

namespace DelegateDecompiler
{
    class ProcessorState(
        Block block,
        bool isStatic,
        Stack<Address> stack,
        VariableInfo[] locals,
        IList<Address> args,
        Instruction instruction,
        IDictionary<Tuple<Address, FieldInfo>, Address> delegates = null)
    {
        public IDictionary<Tuple<Address, FieldInfo>, Address> Delegates { get; } = delegates ?? new Dictionary<Tuple<Address, FieldInfo>, Address>();
        public Block Block { get; } = block;
        public Stack<Address> Stack { get; } = stack;
        public VariableInfo[] Locals { get; } = locals;
        public IList<Address> Args { get; } = args;
        public Action RunNext { get; set; }
        public bool IsStatic { get; } = isStatic;

        public Instruction CurrentInstruction { get; set; } = instruction;

        public ProcessorState Clone(Instruction instruction, Block block)
        {
            var addressMap = new Dictionary<Address, Address>();
            var buffer = Stack.Select(address => address.Clone(addressMap)).Reverse();
            
            var clonedArgs = new Address[Args.Count];
            for (var i = 0; i < Args.Count; i++)
            {
                clonedArgs[i] = Args[i].Clone(addressMap);
            }
            
            var state = new ProcessorState(block, IsStatic, new Stack<Address>(buffer), new VariableInfo[Locals.Length], clonedArgs, instruction, Delegates);
            for (var i = 0; i < Locals.Length; i++)
            {
                state.Locals[i] = new VariableInfo(Locals[i].Type)
                {
                    Address = Locals[i].Address.Clone(addressMap)
                };
            }
            return state;
        }

        public void Merge(Expression test, ProcessorState leftState, ProcessorState rightState)
        {
            Console.WriteLine($"DEBUG: Merge called with test: {test}");
            Console.WriteLine($"DEBUG: LeftState stack count: {leftState.Stack.Count}");
            Console.WriteLine($"DEBUG: RightState stack count: {rightState.Stack.Count}");
            
            var addressMap = new Dictionary<Tuple<Address, Address>, Address>();
            for (var i = 0; i < leftState.Locals.Length; i++)
            {
                var leftLocal = leftState.Locals[i];
                var rightLocal = rightState.Locals[i];
                Locals[i].Address = Address.Merge(test, leftLocal.Address, rightLocal.Address, addressMap);
            }
            
            for (var i = 0; i < leftState.Args.Count; i++)
            {
                var leftArg = leftState.Args[i];
                var rightArg = rightState.Args[i];
                Args[i] = Address.Merge(test, leftArg, rightArg, addressMap);
            }
            
            var buffer = new List<Address>();
            while (leftState.Stack.Count > 0 || rightState.Stack.Count > 0)
            {
                var rightExpression = rightState.Stack.Pop();
                var leftExpression = leftState.Stack.Pop();
                Console.WriteLine($"DEBUG: Merging stack - Left: {leftExpression} (type: {leftExpression.Type}), Right: {rightExpression} (type: {rightExpression.Type})");
                var merged = Address.Merge(test, leftExpression, rightExpression, addressMap);
                Console.WriteLine($"DEBUG: Merged result: {merged} (type: {merged.Type})");
                buffer.Add(merged);
            }
            Stack.Clear();
            foreach (var address in Enumerable.Reverse(buffer))
            {
                Stack.Push(address);
            }
        }

        public Expression Final()
        {
            Console.WriteLine($"DEBUG: Final() called with Stack.Count: {Stack.Count}");
            
            if (Stack.Count == 0)
            {
                Console.WriteLine($"DEBUG: Final() returning Empty()");
                return Expression.Empty();
            }

            if (Stack.Count == 1)
            {
                var result = Stack.Pop();
                Console.WriteLine($"DEBUG: Final() returning single expression: {result}");
                Console.WriteLine($"DEBUG: Final() single expression type: {result.Type}");
                return result;
            }

            var expressions = new Expression[Stack.Count];
            for (var i = expressions.Length - 1; i >= 0; i--)
            {
                expressions[i] = Stack.Pop();
                Console.WriteLine($"DEBUG: Final() expression[{i}]: {expressions[i]}");
            }

            var block = Expression.Block(expressions);
            Console.WriteLine($"DEBUG: Final() returning block: {block}");
            return block;
        }
    }
}
