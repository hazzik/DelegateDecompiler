using System.Collections.Generic;
using System.IO;
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

        public void Dump(TextWriter writer)
        {
            writer.WriteLine($"CFG: {Method.DeclaringType?.FullName}.{Method.Name}");
            foreach (var b in Blocks)
            {
                var flags = (b.IsEntry ? " [Entry]" : "") + (b.IsExit ? " [Exit]" : "");
                writer.WriteLine($"Block {b.Id}{flags}");
                foreach (var i in b.Instructions)
                {
                    writer.WriteLine($"  {i}");
                }
                foreach (var s in b.Successors)
                {
                    writer.WriteLine($"    -> {s.Kind} B{s.To.Id}");
                }
            }
        }
    }
}
