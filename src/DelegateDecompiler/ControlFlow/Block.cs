using System.Collections.Generic;
using Mono.Reflection;

namespace DelegateDecompiler.ControlFlow
{
    /// <summary>
    /// A basic block is a straight-line code sequence with no branches except into the entry and out of the exit.
    /// </summary>
    public sealed class Block
    {
        internal Block(int id, Instruction first)
        {
            Id = id;
            First = first;
        }

        public int Id { get; }
        public Instruction First { get; }
        public Instruction Last { get; internal set; }
        public bool IsEntry { get; internal set; }
        public bool IsExit { get; internal set; }

        public List<Instruction> Instructions { get; } = new List<Instruction>();
        public List<BlockEdge> Successors { get; } = new List<BlockEdge>();
        public List<BlockEdge> Predecessors { get; } = new List<BlockEdge>();

        public override string ToString() => $"Block {Id} ({First?.Offset:x4}-{Last?.Offset:x4})" + (IsEntry ? " [Entry]" : string.Empty) + (IsExit ? " [Exit]" : string.Empty);
    }
}