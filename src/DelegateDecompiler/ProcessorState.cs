using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Reflection;

namespace DelegateDecompiler
{
    internal class ProcessorState
    {
        public IDictionary<FieldInfo, Address> Delegates { get; private set; }
        public Stack<Address> Stack { get; private set; }
        public VariableInfo[] Locals { get; private set; }
        public IList<Address> Args { get; private set; }
        public Instruction Last { get; private set; }
        public Action RunNext { get; set; }

        public Instruction Instruction { get; set; }

        public ProcessorState(Stack<Address> stack, VariableInfo[] locals, IList<Address> args, Instruction instruction,
                              Instruction last = null, IDictionary<FieldInfo, Address> delegates = null)
        {
            Delegates = delegates ?? new Dictionary<FieldInfo, Address>();
            Stack = stack;
            Locals = locals;
            Args = args;
            Instruction = instruction;
            Last = last;
        }

        public ProcessorState Clone(Instruction instruction, Instruction last = null)
        {
            var state = new ProcessorState(null, new VariableInfo[Locals.Length], Args.ToArray(), instruction, last, Delegates);
            var addressMap = new Dictionary<Address, Address>();
            var buffer = new List<Address>();
            foreach (var address in Stack)
            {
                buffer.Add(address.Clone(addressMap));
            }
            state.Stack = new Stack<Address>(Enumerable.Reverse(buffer));
            for (int i = 0; i < Locals.Length; i++)
            {
                state.Locals[i] = new VariableInfo(Locals[i].Type);
                state.Locals[i].Address = Locals[i].Address.Clone(addressMap);
            }
            return state;
        }

        public void Merge(Expression test, ProcessorState leftState, ProcessorState rightState)
        {
            var addressMap = new Dictionary<Tuple<Address, Address>, Address>();
            for (int i = 0; i < leftState.Locals.Length; i++)
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
            return Stack.Count == 0
                ? Expression.Empty()
                : Stack.Pop();
        }
    }
}