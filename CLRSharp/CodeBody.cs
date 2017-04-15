using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace CLRSharp
{
	public class CodeBody
	{
		public class OpCode
		{
			public int addr;

			public CodeEx code;

			public int debugline = -1;

			public object tokenUnknown;

			public int tokenAddr_Index;

			public int[] tokenAddr_Switch;

			public ICLRType tokenType;

			public IField tokenField;

			public IMethod tokenMethod;

			public int tokenI32;

			public long tokenI64;

			public float tokenR32;

			public double tokenR64;

			public string tokenStr;

			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"IL_",
					this.addr.ToString("X04"),
					" ",
					this.code
				});
			}

			public void InitToken(ThreadContext context, CodeBody body, object _p)
			{
				CodeEx codeEx = this.code;
				switch (codeEx)
				{
				case CodeEx.Ldloc_0:
					this.tokenI32 = 0;
					return;
				case CodeEx.Ldloc_1:
					this.tokenI32 = 1;
					return;
				case CodeEx.Ldloc_2:
					this.tokenI32 = 2;
					return;
				case CodeEx.Ldloc_3:
					this.tokenI32 = 3;
					return;
				case CodeEx.Stloc_0:
				case CodeEx.Stloc_1:
				case CodeEx.Stloc_2:
				case CodeEx.Stloc_3:
				case CodeEx.Ldnull:
				case CodeEx.Dup:
				case CodeEx.Pop:
				case CodeEx.Jmp:
				case CodeEx.Calli:
				case CodeEx.Ret:
				case CodeEx.Stind_Ref:
				case CodeEx.Stind_I1:
				case CodeEx.Stind_I2:
				case CodeEx.Stind_I4:
				case CodeEx.Stind_I8:
				case CodeEx.Stind_R4:
				case CodeEx.Stind_R8:
				case CodeEx.Add:
				case CodeEx.Sub:
				case CodeEx.Mul:
				case CodeEx.Div:
				case CodeEx.Div_Un:
				case CodeEx.Rem:
				case CodeEx.Rem_Un:
				case CodeEx.And:
				case CodeEx.Or:
				case CodeEx.Xor:
				case CodeEx.Shl:
				case CodeEx.Shr:
				case CodeEx.Shr_Un:
				case CodeEx.Neg:
				case CodeEx.Not:
				case CodeEx.Conv_I1:
				case CodeEx.Conv_I2:
				case CodeEx.Conv_I4:
				case CodeEx.Conv_I8:
				case CodeEx.Conv_R4:
				case CodeEx.Conv_R8:
				case CodeEx.Conv_U4:
				case CodeEx.Conv_U8:
				case CodeEx.Cpobj:
				case CodeEx.Ldobj:
				case CodeEx.Conv_R_Un:
				case CodeEx.Unbox:
				case CodeEx.Throw:
				case CodeEx.Stobj:
				case CodeEx.Conv_Ovf_I1_Un:
				case CodeEx.Conv_Ovf_I2_Un:
				case CodeEx.Conv_Ovf_I4_Un:
				case CodeEx.Conv_Ovf_I8_Un:
				case CodeEx.Conv_Ovf_U1_Un:
				case CodeEx.Conv_Ovf_U2_Un:
				case CodeEx.Conv_Ovf_U4_Un:
				case CodeEx.Conv_Ovf_U8_Un:
				case CodeEx.Conv_Ovf_I_Un:
				case CodeEx.Conv_Ovf_U_Un:
					goto IL_4AA;
				case CodeEx.Ldarg_S:
					this.tokenI32 = (_p as ParameterReference).Index;
					return;
				case CodeEx.Ldarga_S:
				case CodeEx.Starg_S:
					goto IL_42C;
				case CodeEx.Ldloc_S:
				case CodeEx.Ldloca_S:
				case CodeEx.Stloc_S:
					goto IL_3D5;
				case CodeEx.Ldc_I4_M1:
					this.tokenI32 = -1;
					return;
				case CodeEx.Ldc_I4_0:
					this.tokenI32 = 0;
					return;
				case CodeEx.Ldc_I4_1:
					this.tokenI32 = 1;
					return;
				case CodeEx.Ldc_I4_2:
					this.tokenI32 = 2;
					return;
				case CodeEx.Ldc_I4_3:
					this.tokenI32 = 3;
					return;
				case CodeEx.Ldc_I4_4:
					this.tokenI32 = 4;
					return;
				case CodeEx.Ldc_I4_5:
					this.tokenI32 = 5;
					return;
				case CodeEx.Ldc_I4_6:
					this.tokenI32 = 6;
					return;
				case CodeEx.Ldc_I4_7:
					this.tokenI32 = 7;
					return;
				case CodeEx.Ldc_I4_8:
					this.tokenI32 = 8;
					return;
				case CodeEx.Ldc_I4_S:
					this.tokenI32 = (int)Convert.ToDecimal(_p);
					return;
				case CodeEx.Ldc_I4:
					this.tokenI32 = (int)_p;
					return;
				case CodeEx.Ldc_I8:
					this.tokenI64 = (long)_p;
					return;
				case CodeEx.Ldc_R4:
					this.tokenR32 = (float)_p;
					return;
				case CodeEx.Ldc_R8:
					this.tokenR64 = (double)_p;
					return;
				case CodeEx.Call:
				case CodeEx.Callvirt:
				case CodeEx.Newobj:
					goto IL_2E0;
				case CodeEx.Br_S:
				case CodeEx.Brfalse_S:
				case CodeEx.Brtrue_S:
				case CodeEx.Beq_S:
				case CodeEx.Bge_S:
				case CodeEx.Bgt_S:
				case CodeEx.Ble_S:
				case CodeEx.Blt_S:
				case CodeEx.Bne_Un_S:
				case CodeEx.Bge_Un_S:
				case CodeEx.Bgt_Un_S:
				case CodeEx.Ble_Un_S:
				case CodeEx.Blt_Un_S:
				case CodeEx.Br:
				case CodeEx.Brfalse:
				case CodeEx.Brtrue:
				case CodeEx.Beq:
				case CodeEx.Bge:
				case CodeEx.Bgt:
				case CodeEx.Ble:
				case CodeEx.Blt:
				case CodeEx.Bne_Un:
				case CodeEx.Bge_Un:
				case CodeEx.Bgt_Un:
				case CodeEx.Ble_Un:
				case CodeEx.Blt_Un:
					break;
				case CodeEx.Switch:
				{
					Instruction[] array = _p as Instruction[];
					this.tokenAddr_Switch = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						this.tokenAddr_Switch[i] = body.addr[array[i].Offset];
					}
					return;
				}
				case CodeEx.Ldind_I1:
				case CodeEx.Ldind_U1:
				case CodeEx.Ldind_I2:
				case CodeEx.Ldind_U2:
				case CodeEx.Ldind_I4:
				case CodeEx.Ldind_U4:
				case CodeEx.Ldind_I8:
				case CodeEx.Ldind_I:
				case CodeEx.Ldind_R4:
				case CodeEx.Ldind_R8:
				case CodeEx.Ldind_Ref:
					goto IL_4A8;
				case CodeEx.Ldstr:
					this.tokenStr = (_p as string);
					return;
				case CodeEx.Castclass:
				case CodeEx.Isinst:
				case CodeEx.Box:
				case CodeEx.Newarr:
					goto IL_2BC;
				case CodeEx.Ldfld:
				case CodeEx.Ldflda:
				case CodeEx.Stfld:
				case CodeEx.Ldsfld:
				case CodeEx.Ldsflda:
				case CodeEx.Stsfld:
					this.tokenField = context.GetField(_p);
					return;
				default:
					switch (codeEx)
					{
					case CodeEx.Leave:
					case CodeEx.Leave_S:
						break;
					case CodeEx.Stind_I:
					case CodeEx.Conv_U:
					case CodeEx.Arglist:
					case CodeEx.Ceq:
					case CodeEx.Cgt:
					case CodeEx.Cgt_Un:
					case CodeEx.Clt:
					case CodeEx.Clt_Un:
					case CodeEx.Localloc:
					case CodeEx.Endfilter:
					case CodeEx.Unaligned:
					case CodeEx.Tail:
						goto IL_4AA;
					case CodeEx.Ldftn:
					case CodeEx.Ldvirtftn:
						goto IL_2E0;
					case CodeEx.Ldarg:
						this.tokenI32 = (int)_p;
						return;
					case CodeEx.Ldarga:
					case CodeEx.Starg:
						goto IL_42C;
					case CodeEx.Ldloc:
					case CodeEx.Stloc:
						this.tokenI32 = (int)_p;
						return;
					case CodeEx.Ldloca:
						goto IL_3D5;
					case CodeEx.Volatile:
						goto IL_4A8;
					case CodeEx.Initobj:
					case CodeEx.Constrained:
						goto IL_2BC;
					default:
						goto IL_4AA;
					}
					break;
				}
				this.tokenAddr_Index = body.addr[((Instruction)_p).Offset];
				return;
				IL_2BC:
				this.tokenType = context.GetType(_p);
				return;
				IL_2E0:
				this.tokenMethod = context.GetMethod(_p);
				return;
				IL_3D5:
				this.tokenI32 = ((VariableDefinition)_p).Index;
				return;
				IL_42C:
				this.tokenI32 = (_p as ParameterDefinition).Index;
				IL_4A8:
				return;
				IL_4AA:
				this.tokenUnknown = _p;
			}
		}

		public MethodParamList typelistForLoc = null;

		private MethodDefinition method;

		private bool bInited = false;

		public Dictionary<string, int> debugDoc = new Dictionary<string, int>();

		public List<CodeBody.OpCode> opCodes = new List<CodeBody.OpCode>();

		public Dictionary<int, int> addr = new Dictionary<int, int>();

		public string doc;

		public MethodBody bodyNative
		{
			get
			{
				return this.method.Body;
			}
		}

		public CodeBody(ICLRSharp_Environment env, MethodDefinition _def)
		{
			this.method = _def;
			this.Init(env);
		}

		public void Init(ICLRSharp_Environment env)
		{
			bool flag = this.bInited;
			if (!flag)
			{
				bool hasVariables = this.bodyNative.HasVariables;
				if (hasVariables)
				{
					this.typelistForLoc = new MethodParamList(env, this.bodyNative.Variables);
				}
				for (int i = 0; i < this.bodyNative.Instructions.Count; i++)
				{
					Instruction instruction = this.bodyNative.Instructions[i];
					CodeBody.OpCode opCode = new CodeBody.OpCode();
					opCode.code = (CodeEx)instruction.OpCode.Code;
					opCode.addr = instruction.Offset;
					bool flag2 = instruction.SequencePoint != null;
					if (flag2)
					{
						bool flag3 = this.debugDoc == null;
						if (flag3)
						{
							this.debugDoc = new Dictionary<string, int>();
						}
						bool flag4 = !this.debugDoc.ContainsKey(instruction.SequencePoint.Document.Url);
						if (flag4)
						{
							this.debugDoc.Add(instruction.SequencePoint.Document.Url, instruction.SequencePoint.StartLine);
						}
						opCode.debugline = instruction.SequencePoint.StartLine;
					}
					this.opCodes.Add(opCode);
					this.addr[opCode.addr] = i;
				}
				ThreadContext activeContext = ThreadContext.activeContext;
				for (int j = 0; j < this.bodyNative.Instructions.Count; j++)
				{
					CodeBody.OpCode opCode2 = this.opCodes[j];
					Instruction instruction2 = this.bodyNative.Instructions[j];
					opCode2.InitToken(activeContext, this, instruction2.Operand);
				}
				this.bInited = true;
			}
		}
	}
}
