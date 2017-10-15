using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace ReflectionStudio.Core.Project
{
	internal class BodyWorker
	{
		// Fields
		private readonly MethodBody m_Body;
		private readonly CilWorker m_CilWorker;
		private readonly IDictionary<Instruction, Instruction> m_FixupMap = new Dictionary<Instruction, Instruction>(10);
		private readonly IDictionary<OpCode, OpCode> m_OpCodeRemap = new Dictionary<OpCode, OpCode>(20);
		private readonly IList<Instruction> m_OriginalInstructions;

		// Methods
		public BodyWorker(MethodBody body)
		{
			this.m_Body = body;
			this.m_CilWorker = body.CilWorker;
			this.m_OriginalInstructions = new List<Instruction>(body.Instructions.Count);
			foreach (Instruction instruction in body.Instructions)
			{
				this.m_OriginalInstructions.Add(instruction);
			}
			this.InitOpCodeRemap();
		}

		public Instruction Append(Instruction ins)
		{
			this.m_CilWorker.Append(ins);
			return ins;
		}

		public Instruction AppendAfter(Instruction afterIns, Instruction ins)
		{
			this.m_CilWorker.InsertAfter(afterIns, ins);
			return ins;
		}

		public Instruction Create(OpCode opcode)
		{
			return this.m_CilWorker.Create(opcode);
		}

		//public Instruction Create(OpCode opcode, CallSite site)
		//{
		//    return this.m_CilWorker.Create(opcode, site);
		//}

		public Instruction Create(OpCode opcode, Instruction label)
		{
			return this.m_CilWorker.Create(opcode, label);
		}

		public Instruction Create(OpCode opcode, VariableDefinition var)
		{
			return this.m_CilWorker.Create(opcode, var);
		}

		public Instruction Create(OpCode opcode, FieldReference field)
		{
			return this.m_CilWorker.Create(opcode, field);
		}

		public Instruction Create(OpCode opcode, MethodReference method)
		{
			return this.m_CilWorker.Create(opcode, method);
		}

		public Instruction Create(OpCode opcode, ParameterDefinition param)
		{
			return this.m_CilWorker.Create(opcode, param);
		}

		public Instruction Create(OpCode opcode, TypeReference type)
		{
			return this.m_CilWorker.Create(opcode, type);
		}

		public Instruction Create(OpCode opcode, byte b)
		{
			return this.m_CilWorker.Create(opcode, b);
		}

		public Instruction Create(OpCode opcode, double d)
		{
			return this.m_CilWorker.Create(opcode, d);
		}

		public Instruction Create(OpCode opcode, int i)
		{
			return this.m_CilWorker.Create(opcode, i);
		}

		public Instruction Create(OpCode opcode, Instruction[] labels)
		{
			return this.m_CilWorker.Create(opcode, labels);
		}

		public Instruction Create(OpCode opcode, long l)
		{
			return this.m_CilWorker.Create(opcode, l);
		}

		public Instruction Create(OpCode opcode, sbyte b)
		{
			return this.m_CilWorker.Create(opcode, b);
		}

		public Instruction Create(OpCode opcode, float f)
		{
			return this.m_CilWorker.Create(opcode, f);
		}

		public Instruction Create(OpCode opcode, string str)
		{
			return this.m_CilWorker.Create(opcode, str);
		}

		public void Done()
		{
			this.PerformFixupMapping();
			this.PerformOpCodeRemapping();
			//this.m_Body.opOptimize();
		}

		private void InitOpCodeRemap()
		{
			this.m_OpCodeRemap[OpCodes.Br_S] = OpCodes.Br;
			this.m_OpCodeRemap[OpCodes.Brfalse_S] = OpCodes.Brfalse;
			this.m_OpCodeRemap[OpCodes.Brtrue_S] = OpCodes.Brtrue;
			this.m_OpCodeRemap[OpCodes.Beq_S] = OpCodes.Beq;
			this.m_OpCodeRemap[OpCodes.Bge_S] = OpCodes.Bge;
			this.m_OpCodeRemap[OpCodes.Bgt_S] = OpCodes.Bgt;
			this.m_OpCodeRemap[OpCodes.Ble_S] = OpCodes.Ble;
			this.m_OpCodeRemap[OpCodes.Blt_S] = OpCodes.Blt;
			this.m_OpCodeRemap[OpCodes.Bne_Un_S] = OpCodes.Bne_Un;
			this.m_OpCodeRemap[OpCodes.Bge_Un_S] = OpCodes.Bge_Un;
			this.m_OpCodeRemap[OpCodes.Bgt_Un_S] = OpCodes.Bgt_Un;
			this.m_OpCodeRemap[OpCodes.Ble_Un_S] = OpCodes.Ble_Un;
			this.m_OpCodeRemap[OpCodes.Blt_Un_S] = OpCodes.Blt_Un;
			this.m_OpCodeRemap[OpCodes.Leave_S] = OpCodes.Leave;
		}

		public Instruction InsertAt(Instruction atIns, Instruction ins)
		{
			this.m_CilWorker.InsertBefore(atIns, ins);
			this.m_FixupMap.Add(atIns, ins);
			return ins;
		}

		public Instruction InsertBefore(Instruction beforeIns, Instruction ins)
		{
			this.m_CilWorker.InsertBefore(beforeIns, ins);
			return ins;
		}

		private void PerformFixupMapping()
		{
			foreach (KeyValuePair<Instruction, Instruction> pair in this.m_FixupMap)
			{
				Instruction key = pair.Key;
				Instruction instruction2 = pair.Value;
				foreach (ExceptionHandler handler in this.m_Body.ExceptionHandlers)
				{
					if (handler.TryStart == key)
					{
						handler.TryStart = instruction2;
					}
					if (handler.TryEnd == key)
					{
						handler.TryEnd = instruction2;
					}
					if (handler.FilterStart == key)
					{
						handler.FilterStart = instruction2;
					}
					if (handler.FilterEnd == key)
					{
						handler.FilterEnd = instruction2;
					}
					if (handler.HandlerStart == key)
					{
						handler.HandlerStart = instruction2;
					}
					if (handler.HandlerEnd == key)
					{
						handler.HandlerEnd = instruction2;
					}
				}
				foreach (Scope scope in this.m_Body.Scopes)
				{
					if (scope.Start == key)
					{
						scope.Start = instruction2;
					}
					if (scope.End == key)
					{
						scope.End = instruction2;
					}
				}
				foreach (Instruction instruction3 in this.m_OriginalInstructions)
				{
					Instruction[] instructionArray;
					OperandType operandType = instruction3.OpCode.OperandType;
					if (operandType != OperandType.InlineBrTarget)
					{
						if (operandType == OperandType.InlineSwitch)
						{
							goto Label_0184;
						}
						if (operandType != OperandType.ShortInlineBrTarget)
						{
							continue;
						}
					}
					if (instruction3.Operand == key)
					{
						instruction3.Operand = instruction2;
					}
					continue;
				Label_0184:
					instructionArray = (Instruction[]) instruction3.Operand;
					for (int i = 0; i < instructionArray.Length; i++)
					{
						if (instructionArray[i] == key)
						{
							instructionArray[i] = instruction2;
						}
					}
				}
			}
		}

		private void PerformOpCodeRemapping()
		{
			foreach (Instruction instruction in this.m_OriginalInstructions)
			{
				if (this.m_OpCodeRemap.ContainsKey(instruction.OpCode))
				{
					instruction.OpCode = this.m_OpCodeRemap[instruction.OpCode];
				}
			}
		}

		public void RemapBodyOutside(Instruction ins)
		{
			this.m_FixupMap.Add(this.m_Body.Instructions.Outside, ins);
		}

		public void Remove(Instruction ins)
		{
			this.m_CilWorker.Remove(ins);
		}

		public Instruction Replace(Instruction oldIns, Instruction ins)
		{
			this.m_CilWorker.Replace(oldIns, ins);
			this.m_FixupMap.Add(oldIns, ins);
			return ins;
		}

		// Properties
		public IList<Instruction> OriginalInstructions
		{
			get
			{
				List<Instruction> list = new List<Instruction>(this.m_OriginalInstructions.Count);
				list.AddRange(this.m_OriginalInstructions);
				return list;
			}
		}
	}
}
