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

            // Handle C# compiler pattern: assignment expressions that duplicate the assigned value
            // The C# compiler generates IL with 'dup' instruction that leaves both the assignment
            // and the assigned value on the stack. Since assignment expressions in .NET already
            // return the assigned value, we can return just the assignment expression.
            if (expressions.Length == 2 && 
                expressions[0] is BinaryExpression assignment && 
                assignment.NodeType == ExpressionType.Assign &&
                IsSameValueAsAssignmentRight(assignment.Right, expressions[1]))
            {
                return assignment;
            }

            return Expression.Block(expressions);
        }

        /// <summary>
        /// Determines if the second expression represents the same value as the first expression.
        /// This uses reference equality which works for the C# compiler's 'dup' pattern where
        /// the exact same expression object is placed on the stack twice.
        /// </summary>
        private static bool IsSameValueAsAssignmentRight(Expression assignmentRight, Expression stackValue)
        {
            // Use reference equality as used elsewhere in the codebase (OptimizeExpressionVisitor, Address.cs)
            // This works because the C# compiler's 'dup' instruction results in the same expression 
            // object being referenced in both positions
            return assignmentRight == stackValue;
        }
    }
}
