using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
	internal static class ControlFlow
	{
		public static IList<ControlFlowBlock> GetControlFlow(this ICollection<Instruction> instructions)
		{
			var jumpTargets = GetJumpTargets(instructions);
			var blocks = new List<ControlFlowBlock>();
			var block = new ControlFlowBlock();

			foreach (var instruction in instructions)
			{
				if (jumpTargets.Contains(instruction))
				{
					blocks.Add(block);
					block = new ControlFlowBlock();
				}

				block.Instructions.Add(instruction);
			}

			blocks.Add(block);

			return blocks;
		}

		static ICollection<Instruction> GetJumpTargets(IEnumerable<Instruction> instructions)
		{
			var jumpTargets = new HashSet<Instruction>();
			foreach (var instruction in instructions)
			{
				switch (instruction.OpCode.OperandType)
				{
					case OperandType.ShortInlineBrTarget:
					case OperandType.InlineBrTarget:
						jumpTargets.Add(instruction.Next);
						jumpTargets.Add((Instruction) instruction.Operand);
						break;
					case OperandType.InlineSwitch:
						jumpTargets.Add(instruction.Next);
						jumpTargets.UnionWith((IEnumerable<Instruction>) instruction.Operand);
						break;
				}
			}

			return jumpTargets;
		}
	}
}