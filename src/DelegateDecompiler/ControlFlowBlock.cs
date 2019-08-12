using System.Collections.Generic;
using Mono.Reflection;

namespace DelegateDecompiler
{
	internal class ControlFlowBlock
	{
		public IList<Instruction> Instructions { get; } = new List<Instruction>();
	}
}