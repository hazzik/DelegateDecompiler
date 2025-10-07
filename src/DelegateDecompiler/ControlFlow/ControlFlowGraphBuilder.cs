using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.ControlFlow
{
    /// <summary>
    /// Builds a control flow graph for a method.
    /// </summary>
    public static class ControlFlowGraphBuilder
    {
        public static ControlFlowGraph Build(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            var body = method.GetMethodBody();
            if (body == null) throw new InvalidOperationException("Method has no body");

            var instructions = method.GetInstructions();
            if (instructions.Count == 0)
            {
                var emptyEntry = new Block(0, null) { IsEntry = true, IsExit = true };
                return new ControlFlowGraph(method, new List<Block> { emptyEntry }, emptyEntry, emptyEntry);
            }

            var blockStart = new HashSet<Instruction>();
            // first instruction always starts a block
            blockStart.Add(instructions[0]);

            // discover targets
            foreach (var i in instructions)
            {
                switch (i.Operand)
                {
                    case Instruction target:
                    {
                        blockStart.Add(target);
                        break;
                    }
                    case Instruction[] targets:
                    {
                        blockStart.UnionWith(targets);
                        break;
                    }
                }

                if ((i.OpCode.FlowControl == FlowControl.Return || i.OpCode.FlowControl == FlowControl.Throw || i.OpCode.FlowControl == FlowControl.Cond_Branch || i.OpCode.FlowControl == FlowControl.Branch) && i.Next != null)
                    blockStart.Add(i.Next);
            }

            // Exception handler starts
            foreach (var eh in body.ExceptionHandlingClauses)
            {
                var tryStart = instructions.FirstOrDefault(i => i.Offset == eh.TryOffset);
                if (tryStart != null)
                    blockStart.Add(tryStart);

                var handlerStart = instructions.FirstOrDefault(i => i.Offset == eh.HandlerOffset);
                if (handlerStart != null)
                    blockStart.Add(handlerStart);

                if (eh.Flags == ExceptionHandlingClauseOptions.Filter)
                {
                    var filterStart = instructions.FirstOrDefault(i => i.Offset == eh.FilterOffset);
                    if (filterStart != null)
                        blockStart.Add(filterStart);
                }
            }

            var blocks = new List<Block>();
            var blockMap = new Dictionary<Instruction, Block>();
            Block current = null;
            foreach (var i in instructions)
            {
                if (blockStart.Contains(i))
                {
                    current = new Block(blocks.Count, i);
                    blocks.Add(current);
                    blockMap[i] = current;
                }

                current!.Instructions.Add(i);
                current.Last = i;

                if (i.OpCode.FlowControl == FlowControl.Return || i.OpCode.FlowControl == FlowControl.Throw ||
                    i.OpCode.FlowControl == FlowControl.Cond_Branch || i.OpCode.FlowControl == FlowControl.Branch)
                    current = null;
            }

            // Add synthetic exit block
            var exit = new Block(blocks.Count, null) { IsExit = true };
            blocks.Add(exit);

            // Build edges
            foreach (var block in blocks.Where(b => !b.IsExit)) 
                Connect(block, exit, blockMap);

            // Exception edges (coarse grained: from any block intersecting try to handler entry)
            foreach (var eh in body.ExceptionHandlingClauses)
            {
                var handlerStart = instructions.FirstOrDefault(i => i.Offset == eh.HandlerOffset);
                if (handlerStart == null || !blockMap.TryGetValue(handlerStart, out var handlerBlock)) continue;

                var tryStart = eh.TryOffset;
                var tryEnd = eh.TryOffset + eh.TryLength; // exclusive
                foreach (var b in blocks.Where(b => !b.IsExit && b.First != null))
                {
                    var bFirst = b.First.Offset;
                    var bLast = b.Last.Offset;
                    if (bFirst >= tryStart && bFirst < tryEnd || bLast >= tryStart && bLast < tryEnd)
                    {
                        var kind = eh.Flags switch
                        {
                            ExceptionHandlingClauseOptions.Finally => EdgeKind.Finally,
                            ExceptionHandlingClauseOptions.Filter => EdgeKind.Filter,
                            _ => EdgeKind.Exception
                        };
                        Connect(b, handlerBlock, kind, isException: true);
                    }
                }
            }

            var entry = blocks[0];
            entry.IsEntry = true;

            return new ControlFlowGraph(method, blocks, entry, exit);
        }

        static void Connect(Block from, Block to, Dictionary<Instruction, Block> blockMap)
        {
            if (from.Last == null)
                return;

            switch (from.Last.OpCode.FlowControl)
            {
                case FlowControl.Return or FlowControl.Throw:
                    Connect(from, to, EdgeKind.UnconditionalBranch);
                    break;
                case FlowControl.Branch:
                    Connect(from, blockMap[(Instruction)from.Last.Operand], EdgeKind.UnconditionalBranch);
                    break;
                case FlowControl.Cond_Branch when from.Last.Operand is Instruction target:
                {
                    Connect(from, blockMap[target], EdgeKind.ConditionalTrue);
                    if (from.Last.Next != null && blockMap.TryGetValue(from.Last.Next, out var fBlock))
                        Connect(from, fBlock, EdgeKind.ConditionalFalse);
                    break;
                }
                case FlowControl.Cond_Branch when from.Last.Operand is Instruction[] targets:
                    foreach (var target in targets)
                        Connect(from, blockMap[target], EdgeKind.Switch);

                    if (from.Last.Next != null && blockMap.TryGetValue(from.Last.Next, out var dBlock))
                        Connect(from, dBlock, EdgeKind.Switch);
                    break;
                default:
                    if (from.Last.Next != null && blockMap.TryGetValue(from.Last.Next, out var nextBlock))
                        Connect(from, nextBlock, EdgeKind.FallThrough);
                    break;
            }
        }

        static void Connect(Block from, Block to, EdgeKind kind, bool isException = false)
        {
            if (from == to) // self edge will be discovered through loop detection if needed; keep simple
                return;

            var edge = new BlockEdge(from, to, kind, isException);
            from.Successors.Add(edge);
            to.Predecessors.Add(edge);
        }
    }
}
