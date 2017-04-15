using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace CLRSharp
{
	public class ThreadContext
	{
		[ThreadStatic]
		private static ThreadContext _activeContext = null;

		private Stack<StackFrame> stacks = new Stack<StackFrame>();

		public bool SetNoTry = false;

		private Dictionary<int, IMethod> methodCache = new Dictionary<int, IMethod>();

		private Dictionary<int, IField> fieldCache = new Dictionary<int, IField>();

		public static ThreadContext activeContext
		{
			get
			{
				return ThreadContext._activeContext;
			}
		}

		public ICLRSharp_Environment environment
		{
			get;
			private set;
		}

		public int DebugLevel
		{
			get;
			private set;
		}

		public ThreadContext(ICLRSharp_Environment env)
		{
			this.environment = env;
			this.DebugLevel = 0;
			bool flag = ThreadContext._activeContext != null;
			if (flag)
			{
				env.logger.Log_Error("在同一线程上多次创建ThreadContext");
			}
			ThreadContext._activeContext = this;
		}

		public ThreadContext(ICLRSharp_Environment env, int DebugLevel)
		{
			this.environment = env;
			this.DebugLevel = DebugLevel;
			bool flag = ThreadContext._activeContext != null;
			if (flag)
			{
				env.logger.Log_Error("在同一线程上多次创建ThreadContext");
			}
			ThreadContext._activeContext = this;
		}

		public Stack<StackFrame> GetStackFrames()
		{
			return this.stacks;
		}

		public string Dump()
		{
			string text = "";
			foreach (StackFrame current in this.GetStackFrames())
			{
				Instruction code = current.GetCode();
				Instruction instruction = code;
				while (instruction != null && instruction.SequencePoint == null)
				{
					instruction = instruction.Previous;
				}
				bool flag = instruction != null && instruction.SequencePoint != null;
				if (flag)
				{
					text = string.Concat(new object[]
					{
						text,
						instruction.SequencePoint.Document.Url,
						"(",
						instruction.SequencePoint.StartLine,
						")\n"
					});
				}
				else
				{
					text += "!no pdb info,no code filename(no line)!\n";
				}
				bool flag2 = code == null;
				if (!flag2)
				{
					text = text + "    IL " + code.ToString() + "\n";
					bool flag3 = current._params != null;
					if (flag3)
					{
						text = string.Concat(new object[]
						{
							text,
							"    ===Params(",
							current._params.Length,
							")===\n"
						});
						for (int i = 0; i < current._params.Length; i++)
						{
							text = string.Concat(new object[]
							{
								text,
								"        param",
								i.ToString("D04"),
								current._params[i],
								"\n"
							});
						}
					}
					text = string.Concat(new object[]
					{
						text,
						"    ===VarSlots(",
						current.slotVar.Count,
						")===\n"
					});
					for (int j = 0; j < current.slotVar.Count; j++)
					{
						text = string.Concat(new object[]
						{
							text,
							"        var",
							j.ToString("D04"),
							current.slotVar[j],
							"\n"
						});
					}
				}
			}
			return text;
		}

		public object ExecuteFunc(IMethod_Sharp method, object _this, object[] _params)
		{
			bool flag = this.DebugLevel >= 9;
			if (flag)
			{
				this.environment.logger.Log("<Call>::" + method.DeclaringType.FullName + "::" + method.Name.ToString());
			}
			StackFrame stackFrame = new StackFrame(method.Name, method.isStatic);
			this.stacks.Push(stackFrame);
			bool flag2 = method.Name == ".ctor";
			bool flag3 = flag2;
			object[] array;
			if (flag3)
			{
				array = new object[(_params == null) ? 1 : (_params.Length + 1)];
				bool flag4 = _params != null;
				if (flag4)
				{
					_params.CopyTo(array, 1);
				}
				array[0] = _this;
			}
			else
			{
				bool flag5 = !method.isStatic;
				if (flag5)
				{
					array = new object[(_params == null) ? 1 : (_params.Length + 1)];
					array[0] = _this;
					bool flag6 = _params != null;
					if (flag6)
					{
						_params.CopyTo(array, 1);
					}
				}
				else
				{
					array = _params;
				}
			}
			stackFrame.SetParams(array);
			bool flag7 = method.body != null;
			if (flag7)
			{
				stackFrame.Init(method.body);
				stackFrame.SetCodePos(0);
				stackFrame._codepos = 0;
				bool flag8 = method.body.bodyNative.HasExceptionHandlers && !this.SetNoTry;
				if (flag8)
				{
					this.RunCodeWithTry(method.body, stackFrame);
				}
				else
				{
					this.RunCode(stackFrame, method.body);
				}
			}
			bool flag9 = this.DebugLevel >= 9;
			if (flag9)
			{
				this.environment.logger.Log("<CallEnd>");
			}
			object obj = this.stacks.Pop().Return();
			return flag2 ? _this : obj;
		}

		private void RunCodeWithTry(CodeBody body, StackFrame stack)
		{
			try
			{
				this.RunCode(stack, body);
			}
			catch (Exception ex)
			{
				bool flag = false;
				bool hasExceptionHandlers = body.bodyNative.HasExceptionHandlers;
				if (hasExceptionHandlers)
				{
					flag = this.JumpToErr(body, stack, ex);
				}
				bool flag2 = !flag;
				if (flag2)
				{
					throw ex;
				}
			}
		}

		public ICLRType GetType(string fullname)
		{
			ICLRType type = this.environment.GetType(fullname);
			ICLRType_Sharp iCLRType_Sharp = type as ICLRType_Sharp;
			bool flag = iCLRType_Sharp != null && iCLRType_Sharp.NeedCCtor;
			if (flag)
			{
				iCLRType_Sharp.InvokeCCtor(this);
			}
			return type;
		}

		public ICLRType GetType(object token)
		{
			token.GetHashCode();
			bool flag = token is TypeDefinition;
			string fullName;
			if (flag)
			{
				TypeDefinition typeDefinition = token as TypeDefinition;
				ModuleDefinition module = typeDefinition.Module;
				fullName = typeDefinition.FullName;
			}
			else
			{
				bool flag2 = token is TypeReference;
				if (!flag2)
				{
					throw new NotImplementedException();
				}
				TypeReference typeReference = token as TypeReference;
				ModuleDefinition module = typeReference.Module;
				fullName = typeReference.FullName;
			}
			return this.GetType(fullName);
		}

		public IMethod GetMethod(object token)
		{
			IMethod result;
			try
			{
				IMethod method = null;
				bool flag = this.methodCache.TryGetValue(token.GetHashCode(), out method);
				if (flag)
				{
					result = method;
				}
				else
				{
					MethodParamList methodParamList = null;
					bool flag2 = token is MethodReference;
					string name;
					string text;
					MethodParamList types;
					if (flag2)
					{
						MethodReference methodReference = token as MethodReference;
						ModuleDefinition module = methodReference.Module;
						name = methodReference.Name;
						text = methodReference.DeclaringType.FullName;
						types = new MethodParamList(this.environment, methodReference);
						bool isGenericInstance = methodReference.IsGenericInstance;
						if (isGenericInstance)
						{
							GenericInstanceMethod method2 = methodReference as GenericInstanceMethod;
							methodParamList = new MethodParamList(this.environment, method2);
						}
					}
					else
					{
						bool flag3 = token is MethodDefinition;
						if (!flag3)
						{
							throw new NotImplementedException();
						}
						MethodDefinition methodDefinition = token as MethodDefinition;
						ModuleDefinition module = methodDefinition.Module;
						name = methodDefinition.Name;
						text = methodDefinition.DeclaringType.FullName;
						types = new MethodParamList(this.environment, methodDefinition);
						bool isGenericInstance2 = methodDefinition.IsGenericInstance;
						if (isGenericInstance2)
						{
							throw new NotImplementedException();
						}
					}
					ICLRType type = this.GetType(text);
					bool flag4 = type == null;
					if (flag4)
					{
						text = text.Replace("0...", "");
						type = this.GetType(text);
					}
					bool flag5 = type == null;
					if (flag5)
					{
						throw new Exception("type can't find:" + text);
					}
					bool flag6 = methodParamList != null;
					IMethod method3;
					if (flag6)
					{
						method3 = type.GetMethodT(name, methodParamList, types);
					}
					else
					{
						method3 = type.GetMethod(name, types);
					}
					this.methodCache[token.GetHashCode()] = method3;
					result = method3;
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error GetMethod==<这意味着这个函数无法被L#找到>" + token, innerException);
			}
			return result;
		}

		public IField GetField(object token)
		{
			IField field = null;
			bool flag = this.fieldCache.TryGetValue(token.GetHashCode(), out field);
			IField result;
			if (flag)
			{
				result = field;
			}
			else
			{
				bool flag2 = token is FieldDefinition;
				if (flag2)
				{
					FieldDefinition fieldDefinition = token as FieldDefinition;
					ICLRType type = this.GetType(fieldDefinition.DeclaringType.FullName);
					field = type.GetField(fieldDefinition.Name);
				}
				else
				{
					bool flag3 = token is FieldReference;
					if (!flag3)
					{
						throw new NotImplementedException("不可处理的token" + token.GetType().ToString());
					}
					FieldReference fieldReference = token as FieldReference;
					ICLRType type2 = this.GetType(fieldReference.DeclaringType.FullName);
					field = type2.GetField(fieldReference.Name);
				}
				this.fieldCache[token.GetHashCode()] = field;
				result = field;
			}
			return result;
		}

		private object GetToken(object token)
		{
			bool flag = token is FieldDefinition || token is FieldReference;
			object result;
			if (flag)
			{
				FieldDefinition fieldDefinition = token as FieldDefinition;
				bool flag2 = fieldDefinition != null && fieldDefinition.Name[0] == '$';
				if (flag2)
				{
					result = fieldDefinition.InitialValue;
				}
				else
				{
					result = this.GetField(token);
				}
			}
			else
			{
				bool flag3 = token is TypeDefinition || token is TypeReference;
				if (!flag3)
				{
					throw new NotImplementedException("不可处理的token" + token.GetType().ToString());
				}
				result = this.GetType(token);
			}
			return result;
		}

		private int GetParamPos(object token)
		{
			bool flag = token is byte;
			int result;
			if (flag)
			{
				result = (int)((byte)token);
			}
			else
			{
				bool flag2 = token is sbyte;
				if (flag2)
				{
					result = (int)((sbyte)token);
				}
				else
				{
					bool flag3 = token is int;
					if (flag3)
					{
						result = (int)token;
					}
					else
					{
						bool flag4 = token is ParameterReference;
						if (!flag4)
						{
							throw new NotImplementedException();
						}
						int index = (token as ParameterReference).Index;
						result = index;
					}
				}
			}
			return result;
		}

		private int GetBaseCount(Type _now, Type _base)
		{
			bool flag = _now == _base;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = !_now.IsSubclassOf(_base);
				if (flag2)
				{
					result = -1;
				}
				else
				{
					result = this.GetBaseCount(_now.BaseType, _base) + 1;
				}
			}
			return result;
		}

		private bool JumpToErr(CodeBody body, StackFrame frame, Exception err)
		{
			Instruction code = frame.GetCode();
			List<ExceptionHandler> list = new List<ExceptionHandler>();
			ExceptionHandler exceptionHandler = null;
			int num = -1;
			foreach (ExceptionHandler current in body.bodyNative.ExceptionHandlers)
			{
				bool flag = current.HandlerType == ExceptionHandlerType.Catch;
				if (flag)
				{
					Type typeForSystem = this.GetType(current.CatchType).TypeForSystem;
					bool flag2 = typeForSystem == err.GetType() || err.GetType().IsSubclassOf(typeForSystem);
					if (flag2)
					{
						bool flag3 = current.TryStart.Offset <= code.Offset && current.TryEnd.Offset >= code.Offset;
						if (flag3)
						{
							bool flag4 = exceptionHandler == null;
							if (flag4)
							{
								exceptionHandler = current;
								num = this.GetBaseCount(typeForSystem, err.GetType());
							}
							else
							{
								bool flag5 = current.TryStart.Offset > exceptionHandler.TryStart.Offset || current.TryEnd.Offset < exceptionHandler.TryEnd.Offset;
								if (flag5)
								{
									exceptionHandler = current;
									num = this.GetBaseCount(typeForSystem, err.GetType());
								}
								else
								{
									bool flag6 = current.TryStart.Offset == exceptionHandler.TryStart.Offset || current.TryEnd.Offset == exceptionHandler.TryEnd.Offset;
									if (flag6)
									{
										bool flag7 = typeForSystem == err.GetType();
										if (flag7)
										{
											exceptionHandler = current;
											num = this.GetBaseCount(typeForSystem, err.GetType());
										}
										else
										{
											bool flag8 = this.GetType(exceptionHandler.CatchType).TypeForSystem == err.GetType();
											if (flag8)
											{
												continue;
											}
											int baseCount = this.GetBaseCount(typeForSystem, err.GetType());
											bool flag9 = baseCount == -1;
											if (flag9)
											{
												continue;
											}
											bool flag10 = baseCount < num;
											if (flag10)
											{
												exceptionHandler = current;
												num = baseCount;
											}
										}
									}
								}
							}
							list.Add(current);
						}
					}
				}
			}
			bool flag11 = exceptionHandler != null;
			bool result;
			if (flag11)
			{
				frame.stackCalc.Push(err);
				frame.SetCodePos(exceptionHandler.HandlerStart.Offset);
				this.RunCodeWithTry(body, frame);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void RunCode(StackFrame stack, CodeBody body)
		{
			CodeBody.OpCode opCode;
			while (true)
			{
				int codepos = stack._codepos;
				opCode = body.opCodes[codepos];
				bool flag = this.DebugLevel >= 9;
				if (flag)
				{
					this.environment.logger.Log(opCode.ToString());
				}
				switch (opCode.code)
				{
				case CodeEx.Nop:
					stack.Nop();
					continue;
				case CodeEx.Break:
					stack.Break(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Ldarg_0:
					stack.Ldarg(0);
					continue;
				case CodeEx.Ldarg_1:
					stack.Ldarg(1);
					continue;
				case CodeEx.Ldarg_2:
					stack.Ldarg(2);
					continue;
				case CodeEx.Ldarg_3:
					stack.Ldarg(3);
					continue;
				case CodeEx.Ldloc_0:
					stack.Ldloc(0);
					continue;
				case CodeEx.Ldloc_1:
					stack.Ldloc(1);
					continue;
				case CodeEx.Ldloc_2:
					stack.Ldloc(2);
					continue;
				case CodeEx.Ldloc_3:
					stack.Ldloc(3);
					continue;
				case CodeEx.Stloc_0:
					stack.Stloc(0);
					continue;
				case CodeEx.Stloc_1:
					stack.Stloc(1);
					continue;
				case CodeEx.Stloc_2:
					stack.Stloc(2);
					continue;
				case CodeEx.Stloc_3:
					stack.Stloc(3);
					continue;
				case CodeEx.Ldarg_S:
				{
					bool isStatic = body.bodyNative.Method.IsStatic;
					if (isStatic)
					{
						stack.Ldarg(opCode.tokenI32);
					}
					else
					{
						stack.Ldarg(opCode.tokenI32 + 1);
					}
					continue;
				}
				case CodeEx.Ldarga_S:
				{
					bool isStatic2 = body.bodyNative.Method.IsStatic;
					if (isStatic2)
					{
						stack.Ldarga(opCode.tokenI32);
					}
					else
					{
						stack.Ldarga(opCode.tokenI32 + 1);
					}
					continue;
				}
				case CodeEx.Starg_S:
				{
					bool isStatic3 = body.bodyNative.Method.IsStatic;
					if (isStatic3)
					{
						stack.Starg(this, opCode.tokenI32);
					}
					else
					{
						stack.Starg(this, opCode.tokenI32 + 1);
					}
					continue;
				}
				case CodeEx.Ldloc_S:
					stack.Ldloc(opCode.tokenI32);
					continue;
				case CodeEx.Ldloca_S:
					stack.Ldloca(opCode.tokenI32);
					continue;
				case CodeEx.Stloc_S:
					stack.Stloc(opCode.tokenI32);
					continue;
				case CodeEx.Ldnull:
					stack.Ldnull();
					continue;
				case CodeEx.Ldc_I4_M1:
					stack.Ldc_I4(-1);
					continue;
				case CodeEx.Ldc_I4_0:
					stack.Ldc_I4(0);
					continue;
				case CodeEx.Ldc_I4_1:
					stack.Ldc_I4(1);
					continue;
				case CodeEx.Ldc_I4_2:
					stack.Ldc_I4(2);
					continue;
				case CodeEx.Ldc_I4_3:
					stack.Ldc_I4(3);
					continue;
				case CodeEx.Ldc_I4_4:
					stack.Ldc_I4(4);
					continue;
				case CodeEx.Ldc_I4_5:
					stack.Ldc_I4(5);
					continue;
				case CodeEx.Ldc_I4_6:
					stack.Ldc_I4(6);
					continue;
				case CodeEx.Ldc_I4_7:
					stack.Ldc_I4(7);
					continue;
				case CodeEx.Ldc_I4_8:
					stack.Ldc_I4(8);
					continue;
				case CodeEx.Ldc_I4_S:
					stack.Ldc_I4(opCode.tokenI32);
					continue;
				case CodeEx.Ldc_I4:
					stack.Ldc_I4(opCode.tokenI32);
					continue;
				case CodeEx.Ldc_I8:
					stack.Ldc_I8(opCode.tokenI64);
					continue;
				case CodeEx.Ldc_R4:
					stack.Ldc_R4(opCode.tokenR32);
					continue;
				case CodeEx.Ldc_R8:
					stack.Ldc_R8(opCode.tokenR64);
					continue;
				case CodeEx.Dup:
					stack.Dup();
					continue;
				case CodeEx.Pop:
					stack.Pop();
					continue;
				case CodeEx.Jmp:
					stack.Jmp(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Call:
					stack.Call(this, opCode.tokenMethod, false);
					continue;
				case CodeEx.Calli:
					stack.Calli(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Ret:
					goto IL_3CF;
				case CodeEx.Br_S:
					stack.Br(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Brfalse_S:
					stack.Brfalse(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Brtrue_S:
					stack.Brtrue(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Beq_S:
					stack.Beq(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bge_S:
					stack.Bge(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bgt_S:
					stack.Bgt(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Ble_S:
					stack.Ble(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Blt_S:
					stack.Blt(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bne_Un_S:
					stack.Bne_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bge_Un_S:
					stack.Bge_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bgt_Un_S:
					stack.Bgt_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Ble_Un_S:
					stack.Ble_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Blt_Un_S:
					stack.Blt_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Br:
					stack.Br(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Brfalse:
					stack.Brfalse(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Brtrue:
					stack.Brtrue(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Beq:
					stack.Beq(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bge:
					stack.Bge(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bgt:
					stack.Bgt(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Ble:
					stack.Ble(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Blt:
					stack.Blt(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bne_Un:
					stack.Bne_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bge_Un:
					stack.Bge_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Bgt_Un:
					stack.Bgt_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Ble_Un:
					stack.Ble_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Blt_Un:
					stack.Blt_Un(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Switch:
					stack.Switch(this, opCode.tokenAddr_Switch);
					continue;
				case CodeEx.Ldind_I1:
					stack.Ldind_I1();
					continue;
				case CodeEx.Ldind_U1:
					stack.Ldind_U1();
					continue;
				case CodeEx.Ldind_I2:
					stack.Ldind_I2();
					continue;
				case CodeEx.Ldind_U2:
					stack.Ldind_U2();
					continue;
				case CodeEx.Ldind_I4:
					stack.Ldind_I4();
					continue;
				case CodeEx.Ldind_U4:
					stack.Ldind_U4();
					continue;
				case CodeEx.Ldind_I8:
					stack.Ldind_I8();
					continue;
				case CodeEx.Ldind_I:
					stack.Ldind_I();
					continue;
				case CodeEx.Ldind_R4:
					stack.Ldind_R4();
					continue;
				case CodeEx.Ldind_R8:
					stack.Ldind_R8();
					continue;
				case CodeEx.Ldind_Ref:
					stack.Ldind_Ref();
					continue;
				case CodeEx.Stind_Ref:
					stack.Stind_Ref(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Stind_I1:
					stack.Stind_I1(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Stind_I2:
					stack.Stind_I2(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Stind_I4:
					stack.Stind_I4(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Stind_I8:
					stack.Stind_I8(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Stind_R4:
					stack.Stind_R4(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Stind_R8:
					stack.Stind_R8(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Add:
					stack.Add();
					continue;
				case CodeEx.Sub:
					stack.Sub();
					continue;
				case CodeEx.Mul:
					stack.Mul();
					continue;
				case CodeEx.Div:
					stack.Div();
					continue;
				case CodeEx.Div_Un:
					stack.Div_Un();
					continue;
				case CodeEx.Rem:
					stack.Rem();
					continue;
				case CodeEx.Rem_Un:
					stack.Rem_Un();
					continue;
				case CodeEx.And:
					stack.And();
					continue;
				case CodeEx.Or:
					stack.Or();
					continue;
				case CodeEx.Xor:
					stack.Xor();
					continue;
				case CodeEx.Shl:
					stack.Shl(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Shr:
					stack.Shr(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Shr_Un:
					stack.Shr_Un(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Neg:
					stack.Neg();
					continue;
				case CodeEx.Not:
					stack.Not();
					continue;
				case CodeEx.Conv_I1:
					stack.Conv_I1();
					continue;
				case CodeEx.Conv_I2:
					stack.Conv_I2();
					continue;
				case CodeEx.Conv_I4:
					stack.Conv_I4();
					continue;
				case CodeEx.Conv_I8:
					stack.Conv_I8();
					continue;
				case CodeEx.Conv_R4:
					stack.Conv_R4();
					continue;
				case CodeEx.Conv_R8:
					stack.Conv_R8();
					continue;
				case CodeEx.Conv_U4:
					stack.Conv_U4();
					continue;
				case CodeEx.Conv_U8:
					stack.Conv_U8();
					continue;
				case CodeEx.Callvirt:
					stack.Call(this, opCode.tokenMethod, true);
					continue;
				case CodeEx.Cpobj:
					stack.Cpobj(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Ldobj:
					stack.Ldobj(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Ldstr:
					stack.Ldstr(opCode.tokenStr);
					continue;
				case CodeEx.Newobj:
					stack.NewObj(this, opCode.tokenMethod);
					continue;
				case CodeEx.Castclass:
					stack.Castclass(this, opCode.tokenType);
					continue;
				case CodeEx.Isinst:
					stack.Isinst(this, opCode.tokenType);
					continue;
				case CodeEx.Conv_R_Un:
					stack.Conv_R_Un();
					continue;
				case CodeEx.Unbox:
					stack.Unbox();
					continue;
				case CodeEx.Throw:
					stack.Throw(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Ldfld:
					stack.Ldfld(this, opCode.tokenField);
					continue;
				case CodeEx.Ldflda:
					stack.Ldflda(this, opCode.tokenField);
					continue;
				case CodeEx.Stfld:
					stack.Stfld(this, opCode.tokenField);
					continue;
				case CodeEx.Ldsfld:
					stack.Ldsfld(this, opCode.tokenField);
					continue;
				case CodeEx.Ldsflda:
					stack.Ldsflda(this, opCode.tokenField);
					continue;
				case CodeEx.Stsfld:
					stack.Stsfld(this, opCode.tokenField);
					continue;
				case CodeEx.Stobj:
					stack.Stobj(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Conv_Ovf_I1_Un:
					stack.Conv_Ovf_I1_Un();
					continue;
				case CodeEx.Conv_Ovf_I2_Un:
					stack.Conv_Ovf_I2_Un();
					continue;
				case CodeEx.Conv_Ovf_I4_Un:
					stack.Conv_Ovf_I4_Un();
					continue;
				case CodeEx.Conv_Ovf_I8_Un:
					stack.Conv_Ovf_I8_Un();
					continue;
				case CodeEx.Conv_Ovf_U1_Un:
					stack.Conv_Ovf_U1_Un();
					continue;
				case CodeEx.Conv_Ovf_U2_Un:
					stack.Conv_Ovf_U2_Un();
					continue;
				case CodeEx.Conv_Ovf_U4_Un:
					stack.Conv_Ovf_U4_Un();
					continue;
				case CodeEx.Conv_Ovf_U8_Un:
					stack.Conv_Ovf_U8_Un();
					continue;
				case CodeEx.Conv_Ovf_I_Un:
					stack.Conv_Ovf_I_Un();
					continue;
				case CodeEx.Conv_Ovf_U_Un:
					stack.Conv_Ovf_U_Un();
					continue;
				case CodeEx.Box:
					stack.Box(opCode.tokenType);
					continue;
				case CodeEx.Newarr:
					stack.NewArr(this, opCode.tokenType.TypeForSystem);
					continue;
				case CodeEx.Ldlen:
					stack.LdLen();
					continue;
				case CodeEx.Ldelema:
					stack.Ldelema(opCode.tokenUnknown);
					continue;
				case CodeEx.Ldelem_I1:
					stack.Ldelem_I1();
					continue;
				case CodeEx.Ldelem_U1:
					stack.Ldelem_U1();
					continue;
				case CodeEx.Ldelem_I2:
					stack.Ldelem_I2();
					continue;
				case CodeEx.Ldelem_U2:
					stack.Ldelem_U2();
					continue;
				case CodeEx.Ldelem_I4:
					stack.Ldelem_I4();
					continue;
				case CodeEx.Ldelem_U4:
					stack.Ldelem_U4();
					continue;
				case CodeEx.Ldelem_I8:
					stack.Ldelem_I8();
					continue;
				case CodeEx.Ldelem_I:
					stack.Ldelem_I();
					continue;
				case CodeEx.Ldelem_R4:
					stack.Ldelem_R4();
					continue;
				case CodeEx.Ldelem_R8:
					stack.Ldelem_R8();
					continue;
				case CodeEx.Ldelem_Ref:
					stack.Ldelem_Ref();
					continue;
				case CodeEx.Stelem_I:
					stack.Stelem_I();
					continue;
				case CodeEx.Stelem_I1:
					stack.Stelem_I1();
					continue;
				case CodeEx.Stelem_I2:
					stack.Stelem_I2();
					continue;
				case CodeEx.Stelem_I4:
					stack.Stelem_I4();
					continue;
				case CodeEx.Stelem_I8:
					stack.Stelem_I8();
					continue;
				case CodeEx.Stelem_R4:
					stack.Stelem_R4();
					continue;
				case CodeEx.Stelem_R8:
					stack.Stelem_R8();
					continue;
				case CodeEx.Stelem_Ref:
					stack.Stelem_Ref();
					continue;
				case CodeEx.Ldelem_Any:
					stack.Ldelem_Any(opCode.tokenUnknown);
					continue;
				case CodeEx.Stelem_Any:
					stack.Stelem_Any();
					continue;
				case CodeEx.Unbox_Any:
					stack.Unbox_Any();
					continue;
				case CodeEx.Conv_Ovf_I1:
					stack.Conv_Ovf_I1();
					continue;
				case CodeEx.Conv_Ovf_U1:
					stack.Conv_Ovf_U1();
					continue;
				case CodeEx.Conv_Ovf_I2:
					stack.Conv_Ovf_I2();
					continue;
				case CodeEx.Conv_Ovf_U2:
					stack.Conv_Ovf_U2();
					continue;
				case CodeEx.Conv_Ovf_I4:
					stack.Conv_Ovf_I4();
					continue;
				case CodeEx.Conv_Ovf_U4:
					stack.Conv_Ovf_U4();
					continue;
				case CodeEx.Conv_Ovf_I8:
					stack.Conv_Ovf_I8();
					continue;
				case CodeEx.Conv_Ovf_U8:
					stack.Conv_Ovf_U8();
					continue;
				case CodeEx.Refanyval:
					stack.Refanyval(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Ckfinite:
					stack.Ckfinite();
					continue;
				case CodeEx.Mkrefany:
					stack.Mkrefany(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Ldtoken:
					stack.Ldtoken(this, this.GetToken(opCode.tokenUnknown));
					continue;
				case CodeEx.Conv_U2:
					stack.Conv_U2();
					continue;
				case CodeEx.Conv_U1:
					stack.Conv_U1();
					continue;
				case CodeEx.Conv_I:
					stack.Conv_I();
					continue;
				case CodeEx.Conv_Ovf_I:
					stack.Conv_Ovf_I();
					continue;
				case CodeEx.Conv_Ovf_U:
					stack.Conv_Ovf_U();
					continue;
				case CodeEx.Add_Ovf:
					stack.Add_Ovf(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Add_Ovf_Un:
					stack.Add_Ovf_Un(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Mul_Ovf:
					stack.Mul_Ovf(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Mul_Ovf_Un:
					stack.Mul_Ovf_Un(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Sub_Ovf:
					stack.Sub_Ovf(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Sub_Ovf_Un:
					stack.Sub_Ovf_Un(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Endfinally:
					stack.Endfinally(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Leave:
					stack.Leave(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Leave_S:
					stack.Leave(opCode.tokenAddr_Index);
					continue;
				case CodeEx.Stind_I:
					stack.Stind_I(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Conv_U:
					stack.Conv_U();
					continue;
				case CodeEx.Arglist:
					stack.Arglist(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Ceq:
					stack.Ceq();
					continue;
				case CodeEx.Cgt:
					stack.Cgt();
					continue;
				case CodeEx.Cgt_Un:
					stack.Cgt_Un();
					continue;
				case CodeEx.Clt:
					stack.Clt();
					continue;
				case CodeEx.Clt_Un:
					stack.Clt_Un();
					continue;
				case CodeEx.Ldftn:
					stack.Ldftn(this, opCode.tokenMethod);
					continue;
				case CodeEx.Ldvirtftn:
					stack.Ldvirtftn(this, opCode.tokenMethod);
					continue;
				case CodeEx.Ldarg:
				{
					bool isStatic4 = body.bodyNative.Method.IsStatic;
					if (isStatic4)
					{
						stack.Ldarg(opCode.tokenI32);
					}
					else
					{
						stack.Ldarg(opCode.tokenI32 + 1);
					}
					continue;
				}
				case CodeEx.Ldarga:
				{
					bool isStatic5 = body.bodyNative.Method.IsStatic;
					if (isStatic5)
					{
						stack.Ldarga(opCode.tokenI32);
					}
					else
					{
						stack.Ldarga(opCode.tokenI32 + 1);
					}
					continue;
				}
				case CodeEx.Starg:
				{
					bool isStatic6 = body.bodyNative.Method.IsStatic;
					if (isStatic6)
					{
						stack.Starg(this, opCode.tokenI32);
					}
					else
					{
						stack.Starg(this, opCode.tokenI32 + 1);
					}
					continue;
				}
				case CodeEx.Ldloc:
					stack.Ldloc(opCode.tokenI32);
					continue;
				case CodeEx.Ldloca:
					stack.Ldloca(opCode.tokenI32);
					continue;
				case CodeEx.Stloc:
					stack.Stloc(opCode.tokenI32);
					continue;
				case CodeEx.Localloc:
					stack.Localloc(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Endfilter:
					stack.Endfilter(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Unaligned:
					stack.Unaligned(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Volatile:
					stack.Volatile();
					continue;
				case CodeEx.Tail:
					stack.Tail(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Initobj:
					stack.Initobj(this, opCode.tokenType);
					continue;
				case CodeEx.Constrained:
					stack.Constrained(this, opCode.tokenType);
					continue;
				case CodeEx.Cpblk:
					stack.Cpblk(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Initblk:
					stack.Initblk(this, opCode.tokenUnknown);
					continue;
				case CodeEx.No:
					stack.No(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Rethrow:
					stack.Rethrow(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Sizeof:
					stack.Sizeof(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Refanytype:
					stack.Refanytype(this, opCode.tokenUnknown);
					continue;
				case CodeEx.Readonly:
					stack.Readonly(this, opCode.tokenUnknown);
					continue;
				}
				break;
			}
			throw new Exception("未实现的OpCode:" + opCode.code);
			IL_3CF:
			stack.Ret();
		}
	}
}
