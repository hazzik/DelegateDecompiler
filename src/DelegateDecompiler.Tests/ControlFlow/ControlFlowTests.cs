using Mono.Reflection;
using NUnit.Framework;

namespace DelegateDecompiler.Tests.ControlFlow
{
	public class ControlFlowTests
	{ 
		public static int LinearGraph(int a, int b)
		{
			return a + b;
		}

		[Test]
		public void LinearGraphTest()
		{
			var flow = GetType().GetMethod(nameof(LinearGraph)).GetInstructions().GetControlFlow();

			Assert.That(flow, Has.Count.EqualTo(2));
			Assert.That(flow[0].Instructions, Has.Count.EqualTo(6));
			Assert.That(flow[0].Instructions, Has.Count.EqualTo(2));
		}

		public static int GraphWithCondition(bool a)
		{
			/*
IL_0000: nop				(A)
IL_0001: ldarg.0			(A)
IL_0002: brtrue.s IL_0007	(A) -> (C)
IL_0004: ldc.i4.2			(B)  
IL_0005: br.s IL_0008		(B) -> (D)
IL_0007: ldc.i4.1			(C) 
IL_0008: stloc.0			(D) 
IL_0009: br.s IL_000b		(D) -> (E)
IL_000b: ldloc.0			(E)
IL_000c: ret				(E)
			 */
			return a ? 1 : 2;
		}

		[Test]
		public void GraphWithConditionTest()
		{
			var flow = GetType().GetMethod(nameof(GraphWithCondition)).GetInstructions().GetControlFlow();

			Assert.That(flow, Has.Count.EqualTo(5));
			Assert.That(flow[0].Instructions, Has.Count.EqualTo(3));
			Assert.That(flow[1].Instructions, Has.Count.EqualTo(2));
			Assert.That(flow[2].Instructions, Has.Count.EqualTo(1));
			Assert.That(flow[3].Instructions, Has.Count.EqualTo(2));
			Assert.That(flow[4].Instructions, Has.Count.EqualTo(2));
		}

		public static int GraphWithSwitch(int a)
		{
			/*

IL_0000: nop								(A)
IL_0001: ldarg.0							(A)
IL_0002: stloc.1							(A)
IL_0003: ldloc.1							(A)
IL_0004: stloc.0							(A)
IL_0005: ldloc.0							(A)
IL_0006: switch IL_0019,IL_001d,IL_0021		(A)
IL_0017: br.s IL_0025						(B)
IL_0019: ldc.i4.3							(C)					
IL_001a: stloc.2							(C)
IL_001b: br.s IL_0029						(C)
IL_001d: ldc.i4.4							(D)
IL_001e: stloc.2							(D)						
IL_001f: br.s IL_0029						(D)
IL_0021: ldc.i4.5							(E)
IL_0022: stloc.2							(E)
IL_0023: br.s IL_0029						(E)
IL_0025: ldc.i4.6							(F)
IL_0026: stloc.2							(F)
IL_0027: br.s IL_0029						(F)
IL_0029: ldloc.2							(G)
IL_002a: ret								(G)			 
			*/

			switch (a)
			{
				case 0:
					return 3;
				case 1:
					return 4;
				case 2:
					return 5;
				default:
					return 6;
			}
		}

		[Test]
		public void GraphWithSwitchTest()
		{
			var flow = GetType().GetMethod(nameof(GraphWithSwitch)).GetInstructions().GetControlFlow();

			Assert.That(flow, Has.Count.EqualTo(7));
			Assert.That(flow[0].Instructions, Has.Count.EqualTo(7));
			Assert.That(flow[1].Instructions, Has.Count.EqualTo(1));
			Assert.That(flow[2].Instructions, Has.Count.EqualTo(3));
			Assert.That(flow[3].Instructions, Has.Count.EqualTo(3));
			Assert.That(flow[4].Instructions, Has.Count.EqualTo(3));
			Assert.That(flow[5].Instructions, Has.Count.EqualTo(3));
			Assert.That(flow[6].Instructions, Has.Count.EqualTo(2));
		}
	}
}
