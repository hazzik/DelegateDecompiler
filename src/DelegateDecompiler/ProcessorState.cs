using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Reflection;

namespace DelegateDecompiler
{
    class ProcessorState(
        Stack<Address> stack,
        VariableInfo[] locals,
        IList<Address> args,
        Instruction instruction,
        Instruction last = null,
        IDictionary<Tuple<Address, FieldInfo>, Address> delegates = null)
    {
        public IDictionary<Tuple<Address, FieldInfo>, Address> Delegates { get; } = delegates ?? new Dictionary<Tuple<Address, FieldInfo>, Address>();
        public Stack<Address> Stack { get; } = stack;
        public VariableInfo[] Locals { get; } = locals;
        public IList<Address> Args { get; } = args;
        public Instruction Last { get; } = last;
        public Action RunNext { get; set; }

        public Instruction Instruction { get; set; } = instruction;

        public ProcessorState Clone(Instruction instruction, Instruction last = null)
        {
            var addressMap = new Dictionary<Address, Address>();
            var buffer = Stack.Select(address => address.Clone(addressMap)).Reverse();
            var state = new ProcessorState(new Stack<Address>(buffer), new VariableInfo[Locals.Length], Args.ToArray(), instruction, last, Delegates);
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
            var addressMap = new Dictionary<Tuple<Address, Address>, Address>();
            for (var i = 0; i < leftState.Locals.Length; i++)
            {
                var leftLocal = leftState.Locals[i];
                var rightLocal = rightState.Locals[i];
                Locals[i].Address = Address.Merge(test, leftLocal.Address, rightLocal.Address, addressMap);
            }
            var buffer = new List<Address>();
            while (leftState.Stack.Count > 0 || rightState.Stack.Count > 0)
            {
                var rightExpression = rightState.Stack.Pop();
                var leftExpression = leftState.Stack.Pop();
                buffer.Add(Address.Merge(test, leftExpression, rightExpression, addressMap));
            }
            Stack.Clear();
            foreach (var address in Enumerable.Reverse(buffer))
            {
                Stack.Push(address);
            }
        }

        public Expression Final()
        {
            if (Stack.Count == 0)
                return Expression.Empty();

            if (Stack.Count == 1)
                return Stack.Pop();

            var expressions = new Expression[Stack.Count];
            for (var i = expressions.Length - 1; i >= 0; i--)
            {
                expressions[i] = Stack.Pop();
            }

            return Expression.Block(expressions);
        }
    }
}
