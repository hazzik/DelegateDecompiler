using System;
using System.Collections.Generic;
using System.Linq;
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

			CalculateJumps(blocks);

			return blocks;
		}

		static ICollection<Instruction> GetJumpTargets(ICollection<Instruction> instructions)
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

		static void CalculateJumps(List<ControlFlowBlock> blocks)
		{
			var map = blocks.ToDictionary(x => x.First.Offset);
			foreach (var block in blocks)
			{
				block.Jumps = CalculateJumps(block, map);
			}
		}

		public static IList<ControlFlowBlock> CalculateJumps(ControlFlowBlock block, Dictionary<int, ControlFlowBlock> map)
		{
			var jumps = new List<ControlFlowBlock>();
			switch (block.Last.OpCode.OperandType)
			{
				case OperandType.ShortInlineBrTarget:
				case OperandType.InlineBrTarget:
					var operand = (Instruction) block.Last.Operand;
					jumps.Add(map[operand.Offset]);
					break;
				case OperandType.InlineSwitch:
					var operands = (IEnumerable<Instruction>) block.Last.Operand;
					jumps.AddRange(operands.Select(o => map[o.Offset]));
					break;
			}

			switch (block.Last.OpCode.FlowControl)
			{
				case FlowControl.Meta:
				case FlowControl.Next:
				case FlowControl.Call:
				case FlowControl.Cond_Branch:
				{
					var operand = block.Last.Next;
					jumps.Add(map[operand.Offset]);
					break;
				}
			}

			return jumps;
		}
	}
}