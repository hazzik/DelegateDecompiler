using System.Collections.Generic;
using System.Reflection;

namespace DelegateDecompiler.ControlFlow
{
    /// <summary>
    /// A control flow graph (CFG) representation of a method body.
    /// Provides access to basic blocks, edges, loops and basic exception flow.
    /// </summary>
    public class ControlFlowGraph
    {
        public MethodInfo Method { get; }
        public IList<Block> Blocks { get; }
        public Block Entry { get; }
        public Block Exit { get; }

        internal ControlFlowGraph(MethodInfo method, IList<Block> blocks, Block entry, Block exit)
        {
            Method = method;
            Blocks = blocks;
            Entry = entry;
            Exit = exit;
        }
    }
}
