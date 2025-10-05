using System.Collections.Generic;
using System.Reflection;
using System.Text;

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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Control Flow Graph: ")
                .Append(Method.DeclaringType?.FullName)
                .Append(".")
                .Append(Method.Name)
                .AppendLine();
           
            foreach (var b in Blocks)
            {
                sb.Append(b).AppendLine();
                foreach (var i in b.Instructions)
                    sb.Append("  ").Append(i).AppendLine();
                foreach (var s in b.Successors)
                    sb.Append("    -> ").Append(s.Kind).Append(" B").Append(s.To.Id).AppendLine();
            }

            return sb.ToString();
        }
    }
}
