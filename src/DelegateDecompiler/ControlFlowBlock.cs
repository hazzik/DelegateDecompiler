using System.Collections.Generic;
using Mono.Reflection;

namespace DelegateDecompiler
{
	internal class ControlFlowBlock
	{
		public IList<Instruction> Instructions { get; } = new List<Instruction>();
		public Instruction First => Instructions[0];
		public Instruction Last => Instructions[Instructions.Count - 1];
		public IList<ControlFlowBlock> Jumps { get; set; }
	}
}