using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler
{
    internal class LoadStoreProcessor : IProcessor
    {
        static readonly HashSet<OpCode> LdArgOpcodes = new HashSet<OpCode>
        {
            OpCodes.Ldarg_0,
            OpCodes.Ldarg_1,
            OpCodes.Ldarg_2,
            OpCodes.Ldarg_3,
            OpCodes.Ldarg_S,
            OpCodes.Ldarg,
            OpCodes.Ldarga,
            OpCodes.Ldarga_S
        };

        static readonly HashSet<OpCode> StLocOpcodes = new HashSet<OpCode>
        {
            OpCodes.Stloc_0,
            OpCodes.Stloc_1,
            OpCodes.Stloc_2,
            OpCodes.Stloc_3,
            OpCodes.Stloc_S,
            OpCodes.Stloc
        };

        static readonly HashSet<OpCode> LdElemOpcodes = new HashSet<OpCode>
        {
            OpCodes.Ldelem,
            OpCodes.Ldelem_I,
            OpCodes.Ldelem_I1,
            OpCodes.Ldelem_I2,
            OpCodes.Ldelem_I4,
            OpCodes.Ldelem_I8,
            OpCodes.Ldelem_U1,
            OpCodes.Ldelem_U2,
            OpCodes.Ldelem_U4,
            OpCodes.Ldelem_R4,
            OpCodes.Ldelem_R8,
            OpCodes.Ldelem_Ref
        };

        public bool Process(ProcessorState state)
        {
            // Handle Ldarg operations
            if (LdArgOpcodes.Contains(state.Instruction.OpCode))
            {
                if (state.Instruction.OpCode == OpCodes.Ldarg_0)
                {
                    Processor.LdArg(state, 0);
                }
                else if (state.Instruction.OpCode == OpCodes.Ldarg_1)
                {
                    Processor.LdArg(state, 1);
                }
                else if (state.Instruction.OpCode == OpCodes.Ldarg_2)
                {
                    Processor.LdArg(state, 2);
                }
                else if (state.Instruction.OpCode == OpCodes.Ldarg_3)
                {
                    Processor.LdArg(state, 3);
                }
                else // Ldarg_S, Ldarg, Ldarga, Ldarga_S
                {
                    var operand = (ParameterInfo)state.Instruction.Operand;
                    state.Stack.Push(state.Args.Single(x => ((ParameterExpression)x.Expression).Name == operand.Name));
                }
                return true;
            }

            // Handle Stloc operations
            if (StLocOpcodes.Contains(state.Instruction.OpCode))
            {
                if (state.Instruction.OpCode == OpCodes.Stloc_0)
                {
                    Processor.StLoc(state, 0);
                }
                else if (state.Instruction.OpCode == OpCodes.Stloc_1)
                {
                    Processor.StLoc(state, 1);
                }
                else if (state.Instruction.OpCode == OpCodes.Stloc_2)
                {
                    Processor.StLoc(state, 2);
                }
                else if (state.Instruction.OpCode == OpCodes.Stloc_3)
                {
                    Processor.StLoc(state, 3);
                }
                else // Stloc_S, Stloc
                {
                    var operand = (LocalVariableInfo)state.Instruction.Operand;
                    Processor.StLoc(state, operand.LocalIndex);
                }
                return true;
            }

            // Handle Ldelem operations
            if (LdElemOpcodes.Contains(state.Instruction.OpCode))
            {
                var index = state.Stack.Pop();
                var array = state.Stack.Pop();
                state.Stack.Push(Expression.ArrayIndex(array, index));
                return true;
            }

            // Handle Ldlen operation
            if (state.Instruction.OpCode == OpCodes.Ldlen)
            {
                var array = state.Stack.Pop();
                state.Stack.Push(Expression.ArrayLength(array));
                return true;
            }

            return false;
        }
    }
}