using System.Reflection;

namespace DelegateDecompiler.ControlFlow
{
    internal static class ControlFlowGraphExtensions
    {
        /// <summary>
        /// Builds a control flow graph for the supplied method.
        /// </summary>
        public static ControlFlowGraph BuildControlFlowGraph(this MethodInfo method) => ControlFlowGraphBuilder.Build(method);
    }
}
