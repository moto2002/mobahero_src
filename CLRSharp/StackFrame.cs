using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CLRSharp
{
	public class StackFrame
	{
		public class MyCalcStack : Stack<object>
		{
			private Queue<VBox> unused = new Queue<VBox>();

			public void Push(VBox box)
			{
				bool flag = box != null;
				if (flag)
				{
					box.refcount++;
					while (this.unused.Count > 0)
					{
						VBox vBox = this.unused.Dequeue();
						bool flag2 = vBox.refcount == 0;
						if (flag2)
						{
							ValueOnStack.UnUse(vBox);
						}
					}
				}
				base.Push(box);
			}

			public new object Pop()
			{
				object obj = base.Pop();
				VBox vBox = obj as VBox;
				bool flag = vBox != null;
				if (flag)
				{
					vBox.refcount--;
					bool flag2 = vBox.refcount == 0;
					if (flag2)
					{
						this.unused.Enqueue(vBox);
					}
				}
				return obj;
			}

			public void ClearVBox()
			{
				while (this.unused.Count > 0)
				{
					VBox vBox = this.unused.Dequeue();
					bool flag = vBox.refcount == 0;
					if (flag)
					{
						ValueOnStack.UnUse(vBox);
					}
					else
					{
						Console.WriteLine("not zero.");
					}
				}
				base.Clear();
			}
		}

		public class MySlotVar : List<object>
		{
			public new void Add(object obj)
			{
				base.Add(obj);
			}

			public void Add(VBox box)
			{
				bool flag = box != null;
				if (flag)
				{
					box.refcount++;
				}
				base.Add(box);
			}

			public void ClearVBox()
			{
				foreach (object current in this)
				{
					VBox vBox = current as VBox;
					bool flag = vBox != null;
					if (flag)
					{
						vBox.refcount--;
						bool flag2 = vBox.refcount == 0;
						if (flag2)
						{
							ValueOnStack.UnUse(vBox);
						}
						else
						{
							Console.WriteLine("not zero.");
						}
					}
				}
				base.Clear();
			}
		}

		public enum RefType
		{
			loc,
			arg,
			field,
			Array
		}

		public class RefObj
		{
			public StackFrame frame;

			public int pos;

			public StackFrame.RefType type;

			public IField _field;

			public object _this;

			public Array _array;

			public RefObj(StackFrame frame, int pos, StackFrame.RefType type)
			{
				this.frame = frame;
				this.pos = pos;
				this.type = type;
			}

			public RefObj(IField field, object _this)
			{
				this.type = StackFrame.RefType.field;
				this._field = field;
				this._this = _this;
			}

			public RefObj(Array array, int index)
			{
				this.type = StackFrame.RefType.Array;
				this._array = array;
				this.pos = index;
			}

			public void Set(object obj)
			{
				bool flag = this.type == StackFrame.RefType.arg;
				if (flag)
				{
					this.frame._params[this.pos] = obj;
				}
				else
				{
					bool flag2 = this.type == StackFrame.RefType.loc;
					if (flag2)
					{
						while (this.frame.slotVar.Count <= this.pos)
						{
							this.frame.slotVar.Add(null);
						}
						this.frame.slotVar[this.pos] = obj;
					}
					else
					{
						bool flag3 = this.type == StackFrame.RefType.field;
						if (flag3)
						{
							this._field.Set(this._this, obj);
						}
						else
						{
							bool flag4 = this.type == StackFrame.RefType.Array;
							if (flag4)
							{
								this._array.SetValue(obj, this.pos);
							}
						}
					}
				}
			}

			public object Get()
			{
				bool flag = this.type == StackFrame.RefType.arg;
				object result;
				if (flag)
				{
					result = this.frame._params[this.pos];
				}
				else
				{
					bool flag2 = this.type == StackFrame.RefType.loc;
					if (flag2)
					{
						while (this.frame.slotVar.Count <= this.pos)
						{
							this.frame.slotVar.Add(null);
						}
						result = this.frame.slotVar[this.pos];
					}
					else
					{
						bool flag3 = this.type == StackFrame.RefType.field;
						if (flag3)
						{
							result = this._field.Get(this._this);
						}
						else
						{
							bool flag4 = this.type == StackFrame.RefType.Array;
							if (flag4)
							{
								result = this._array.GetValue(this.pos);
							}
							else
							{
								result = null;
							}
						}
					}
				}
				return result;
			}
		}

		private Instruction _posold;

		public int _codepos = 0;

		public StackFrame.MyCalcStack stackCalc = new StackFrame.MyCalcStack();

		public StackFrame.MySlotVar slotVar = new StackFrame.MySlotVar();

		public object[] _params = null;

		private CodeBody _body = null;

		public string Name
		{
			get;
			private set;
		}

		public bool IsStatic
		{
			get;
			private set;
		}

		public CodeBody codebody
		{
			get
			{
				return this._body;
			}
		}

		public StackFrame(string name, bool isStatic)
		{
			this.Name = name;
			this.IsStatic = this.IsStatic;
		}

		public void SetCodePos(int offset)
		{
			this._codepos = this._body.addr[offset];
		}

		public Instruction GetCode()
		{
			bool flag = this._body == null;
			Instruction result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int addr = this._body.opCodes[this._codepos].addr;
				foreach (Instruction current in this._body.bodyNative.Instructions)
				{
					bool flag2 = addr == current.Offset;
					if (flag2)
					{
						result = current;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public void SetParams(object[] _p)
		{
			bool flag = _p == null;
			if (flag)
			{
				this._params = null;
			}
			else
			{
				this._params = new object[_p.Length];
				int i = 0;
				while (i < _p.Length)
				{
					bool flag2 = _p[i] != null;
					if (!flag2)
					{
						goto IL_60;
					}
					VBox vBox = ValueOnStack.MakeVBox(_p[i].GetType());
					bool flag3 = vBox != null;
					if (!flag3)
					{
						goto IL_60;
					}
					vBox.SetDirect(_p[i]);
					this._params[i] = vBox;
					IL_6C:
					i++;
					continue;
					IL_60:
					this._params[i] = _p[i];
					goto IL_6C;
				}
			}
		}

		public void Init(CodeBody body)
		{
			this._body = body;
			bool flag = body.typelistForLoc != null;
			if (flag)
			{
				for (int i = 0; i < body.typelistForLoc.Count; i++)
				{
					ICLRType type = this._body.typelistForLoc[i];
					this.slotVar.Add(ValueOnStack.MakeVBox(type));
				}
			}
		}

		public object Return()
		{
			this.slotVar.ClearVBox();
			bool flag = this.stackCalc.Count == 0;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				object obj = this.stackCalc.Pop();
				this.stackCalc.ClearVBox();
				result = obj;
			}
			return result;
		}

		private void FillArray(object array, byte[] bytes)
		{
			bool flag = bytes == null;
			if (!flag)
			{
				bool flag2 = array is byte[];
				if (flag2)
				{
					byte[] array2 = array as byte[];
					for (int i = 0; i < bytes.Length; i++)
					{
						array2[i] = bytes[i];
					}
				}
				else
				{
					bool flag3 = array is sbyte[];
					if (flag3)
					{
						sbyte[] array3 = array as sbyte[];
						for (int j = 0; j < bytes.Length; j++)
						{
							array3[j] = (sbyte)bytes[j];
						}
					}
					else
					{
						bool flag4 = array is short[];
						if (flag4)
						{
							int num = 2;
							short[] array4 = array as short[];
							for (int k = 0; k < bytes.Length / num; k++)
							{
								array4[k] = BitConverter.ToInt16(bytes, k * num);
							}
						}
						else
						{
							bool flag5 = array is ushort[];
							if (flag5)
							{
								int num2 = 2;
								ushort[] array5 = array as ushort[];
								for (int l = 0; l < bytes.Length / num2; l++)
								{
									array5[l] = BitConverter.ToUInt16(bytes, l * num2);
								}
							}
							else
							{
								bool flag6 = array is char[];
								if (flag6)
								{
									int num3 = 2;
									char[] array6 = array as char[];
									for (int m = 0; m < Math.Min(bytes.Length / num3, array6.Length); m++)
									{
										array6[m] = (char)BitConverter.ToUInt16(bytes, m * num3);
									}
								}
								else
								{
									bool flag7 = array is int[];
									if (flag7)
									{
										int num4 = 4;
										int[] array7 = array as int[];
										for (int n = 0; n < bytes.Length / num4; n++)
										{
											array7[n] = BitConverter.ToInt32(bytes, n * num4);
										}
									}
									else
									{
										bool flag8 = array is uint[];
										if (flag8)
										{
											int num5 = 4;
											uint[] array8 = array as uint[];
											for (int num6 = 0; num6 < bytes.Length / num5; num6++)
											{
												array8[num6] = BitConverter.ToUInt32(bytes, num6 * num5);
											}
										}
										else
										{
											bool flag9 = array is long[];
											if (flag9)
											{
												int num7 = 8;
												long[] array9 = array as long[];
												for (int num8 = 0; num8 < bytes.Length / num7; num8++)
												{
													array9[num8] = BitConverter.ToInt64(bytes, num8 * num7);
												}
											}
											else
											{
												bool flag10 = array is ulong[];
												if (flag10)
												{
													int num9 = 8;
													ulong[] array10 = array as ulong[];
													for (int num10 = 0; num10 < bytes.Length / num9; num10++)
													{
														array10[num10] = BitConverter.ToUInt64(bytes, num10 * num9);
													}
												}
												else
												{
													bool flag11 = array is float[];
													if (flag11)
													{
														int num11 = 4;
														float[] array11 = array as float[];
														for (int num12 = 0; num12 < bytes.Length / num11; num12++)
														{
															array11[num12] = BitConverter.ToSingle(bytes, num12 * num11);
														}
													}
													else
													{
														bool flag12 = array is double[];
														if (flag12)
														{
															int num13 = 8;
															double[] array12 = array as double[];
															for (int num14 = 0; num14 < bytes.Length / num13; num14++)
															{
																array12[num14] = BitConverter.ToDouble(bytes, num14 * num13);
															}
														}
														else
														{
															bool flag13 = array is bool[];
															if (!flag13)
															{
																throw new NotImplementedException("array=" + array.GetType());
															}
															int num15 = 1;
															bool[] array13 = array as bool[];
															for (int num16 = 0; num16 < bytes.Length / num15; num16++)
															{
																array13[num16] = BitConverter.ToBoolean(bytes, num16 * num15);
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void Call(ThreadContext context, IMethod _clrmethod, bool bVisual)
		{
			bool flag = _clrmethod == null;
			if (flag)
			{
				this._codepos++;
			}
			else
			{
				object[] array = null;
				object obj = null;
				bool flag2 = _clrmethod is IMethod_Sharp;
				bool flag3 = _clrmethod.ParamList != null;
				if (flag3)
				{
					array = new object[_clrmethod.ParamList.Count];
					for (int i = 0; i < array.Length; i++)
					{
						int num = array.Length - 1 - i;
						ICLRType iCLRType = _clrmethod.ParamList[num];
						object obj2 = this.stackCalc.Pop();
						bool flag4 = obj2 is CLRSharp_Instance && iCLRType.TypeForSystem != typeof(CLRSharp_Instance);
						if (flag4)
						{
							CLRSharp_Instance cLRSharp_Instance = obj2 as CLRSharp_Instance;
							bool flag5 = cLRSharp_Instance.type.ContainBase(iCLRType.TypeForSystem);
							bool flag6 = flag5;
							if (flag6)
							{
								ICrossBind crossBind = context.environment.GetCrossBind(iCLRType.TypeForSystem);
								bool flag7 = crossBind != null;
								if (flag7)
								{
									obj2 = crossBind.CreateBind(cLRSharp_Instance);
								}
								else
								{
									obj2 = cLRSharp_Instance.system_base;
								}
							}
						}
						bool flag8 = obj2 is VBox && !flag2;
						if (flag8)
						{
							obj2 = (obj2 as VBox).BoxDefine();
						}
						bool flag9 = obj2 is ICLRType_System;
						if (flag9)
						{
							obj2 = (obj2 as ICLRType_System).TypeForSystem;
						}
						bool flag10 = obj2 is int && iCLRType.TypeForSystem != typeof(int) && iCLRType.TypeForSystem != typeof(object);
						if (flag10)
						{
							VBox vBox = ValueOnStack.MakeVBox(iCLRType);
							bool flag11 = vBox != null;
							if (flag11)
							{
								vBox.SetDirect(obj2);
								bool flag12 = flag2;
								if (flag12)
								{
									obj2 = vBox;
								}
								else
								{
									obj2 = vBox.BoxDefine();
								}
							}
						}
						array[num] = obj2;
					}
				}
				bool flag13 = !_clrmethod.isStatic;
				if (flag13)
				{
					obj = this.stackCalc.Pop();
				}
				bool flag14 = _clrmethod.DeclaringType.FullName.Contains("System.Runtime.CompilerServices.RuntimeHelpers") && _clrmethod.Name.Contains("InitializeArray");
				if (flag14)
				{
					byte[] array2 = array[1] as byte[];
					bool flag15 = array2 == null && array[1] is Field_Common_CLRSharp;
					if (flag15)
					{
						array2 = (array[1] as Field_Common_CLRSharp).field.InitialValue;
					}
					this.FillArray(array[0], array2);
					this._codepos++;
				}
				else
				{
					bool flag16 = _clrmethod.DeclaringType.FullName.Contains("System.Type") && _clrmethod.Name.Contains("GetTypeFromHandle");
					if (flag16)
					{
						this.stackCalc.Push(array[0]);
						this._codepos++;
					}
					else
					{
						bool flag17 = _clrmethod.DeclaringType.FullName.Contains("System.Object") && _clrmethod.Name.Contains(".ctor");
						if (flag17)
						{
							this._codepos++;
						}
						else
						{
							bool flag18 = obj is StackFrame.RefObj && _clrmethod.Name != ".ctor";
							if (flag18)
							{
								obj = (obj as StackFrame.RefObj).Get();
							}
							bool flag19 = obj is VBox;
							if (flag19)
							{
								obj = (obj as VBox).BoxDefine();
							}
							bool flag20 = obj is CLRSharp_Instance && _clrmethod is IMethod_System;
							object obj3 = _clrmethod.Invoke(context, obj, array, bVisual);
							bool flag21 = flag20;
							if (flag21)
							{
								bool flag22 = _clrmethod.Name.Contains(".ctor");
								if (flag22)
								{
									(obj as CLRSharp_Instance).system_base = obj3;
									obj3 = obj;
								}
							}
							bool flag23 = _clrmethod.ReturnType != null && _clrmethod.ReturnType.FullName != "System.Void";
							if (flag23)
							{
								bool flag24 = !(obj3 is VBox);
								if (flag24)
								{
									VBox vBox2 = ValueOnStack.MakeVBox(_clrmethod.ReturnType);
									bool flag25 = vBox2 != null;
									if (flag25)
									{
										vBox2.SetDirect(obj3);
										obj3 = vBox2;
									}
								}
								this.stackCalc.Push(obj3);
							}
							else
							{
								bool flag26 = obj is StackFrame.RefObj && _clrmethod.Name == ".ctor";
								if (flag26)
								{
									(obj as StackFrame.RefObj).Set(obj3);
								}
							}
							this._codepos++;
						}
					}
				}
			}
		}

		public void Nop()
		{
			this._codepos++;
		}

		public void Dup()
		{
			object obj = this.stackCalc.Peek();
			bool flag = obj is VBox;
			if (flag)
			{
				obj = (obj as VBox).Clone();
			}
			this.stackCalc.Push(obj);
			this._codepos++;
		}

		public void Pop()
		{
			this.stackCalc.Pop();
			this._codepos++;
		}

		public void Ret()
		{
			this._codepos++;
		}

		public void Box(ICLRType type)
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool isEnum = type.TypeForSystem.IsEnum;
			if (isEnum)
			{
				bool flag = vBox != null;
				int value;
				if (flag)
				{
					value = vBox.v32;
				}
				else
				{
					value = (int)obj;
				}
				obj = Enum.ToObject(type.TypeForSystem, value);
			}
			else
			{
				bool flag2 = vBox != null;
				if (flag2)
				{
					NumberType typeCode = ValueOnStack.GetTypeCode(type.TypeForSystem);
					bool flag3 = typeCode == vBox.type;
					if (flag3)
					{
						obj = vBox.BoxDefine();
					}
					else
					{
						VBox vBox2 = new VBox(vBox.typeStack, typeCode);
						vBox2.Set(vBox);
						obj = vBox2.BoxDefine();
					}
				}
			}
			this.stackCalc.Push(obj);
			this._codepos++;
		}

		public void Unbox()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = ValueOnStack.MakeVBox(obj.GetType());
			bool flag = vBox != null;
			if (flag)
			{
				vBox.SetDirect(obj);
				this.stackCalc.Push(vBox);
			}
			else
			{
				this.stackCalc.Push(obj);
			}
			this._codepos++;
		}

		public void Unbox_Any()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = ValueOnStack.MakeVBox(obj.GetType());
			bool flag = vBox != null;
			if (flag)
			{
				vBox.SetDirect(obj);
				this.stackCalc.Push(vBox);
			}
			else
			{
				this.stackCalc.Push(obj);
			}
			this._codepos++;
		}

		public void Br(int addr_index)
		{
			this._codepos = addr_index;
		}

		public void Leave(int addr_index)
		{
			this.stackCalc.Clear();
			this._codepos = addr_index;
		}

		public void Brtrue(int addr_index)
		{
			object obj = this.stackCalc.Pop();
			bool flag = false;
			bool flag2 = obj != null;
			if (flag2)
			{
				bool flag3 = obj is VBox;
				if (flag3)
				{
					VBox vBox = obj as VBox;
					flag = vBox.ToBool();
				}
				else
				{
					bool isClass = obj.GetType().IsClass;
					if (isClass)
					{
						flag = true;
					}
					else
					{
						bool flag4 = obj is bool;
						if (flag4)
						{
							flag = (bool)obj;
						}
						else
						{
							flag = (Convert.ToDecimal(obj) > decimal.Zero);
						}
					}
				}
			}
			bool flag5 = flag;
			if (flag5)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Brfalse(int addr_index)
		{
			object obj = this.stackCalc.Pop();
			bool flag = false;
			bool flag2 = obj != null;
			if (flag2)
			{
				bool flag3 = obj is VBox;
				if (flag3)
				{
					VBox vBox = obj as VBox;
					flag = vBox.ToBool();
				}
				else
				{
					bool isClass = obj.GetType().IsClass;
					if (isClass)
					{
						flag = true;
					}
					else
					{
						bool flag4 = obj is bool;
						if (flag4)
						{
							flag = (bool)obj;
						}
						else
						{
							flag = (Convert.ToDecimal(obj) > decimal.Zero);
						}
					}
				}
			}
			bool flag5 = !flag;
			if (flag5)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Beq(int addr_index)
		{
			object obj = this.stackCalc.Pop();
			object obj2 = this.stackCalc.Pop();
			bool flag = obj2 is VBox && obj is VBox;
			if (flag)
			{
				VBox right = obj as VBox;
				VBox vBox = obj2 as VBox;
				bool flag2 = vBox.logic_eq(right);
				if (flag2)
				{
					this._codepos = addr_index;
				}
				else
				{
					this._codepos++;
				}
			}
			else
			{
				bool flag3 = obj2 is int;
				if (!flag3)
				{
					throw new Exception("what a fuck");
				}
				int num = (int)obj2;
				bool flag4 = obj is int;
				int num2;
				if (flag4)
				{
					num2 = (int)obj;
				}
				else
				{
					bool flag5 = obj is VBox;
					if (!flag5)
					{
						throw new Exception("what a fuck");
					}
					num2 = (obj as VBox).v32;
				}
				bool flag6 = num == num2;
				if (flag6)
				{
					this._codepos = addr_index;
				}
				else
				{
					this._codepos++;
				}
			}
		}

		public void Bne(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_ne(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Bne_Un(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_ne_Un(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Bge(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_ge(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Bge_Un(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_ge_Un(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Bgt(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_gt(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Bgt_Un(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_gt_Un(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Ble(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_le(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Ble_Un(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_le_Un(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Blt(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_lt(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Blt_Un(int addr_index)
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			bool flag = vBox.logic_lt_Un(right);
			if (flag)
			{
				this._codepos = addr_index;
			}
			else
			{
				this._codepos++;
			}
		}

		public void Ldc_I4(int v)
		{
			VBox vBox = ValueOnStack.MakeVBox(NumberType.INT32);
			vBox.v32 = v;
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldc_I8(long v)
		{
			VBox vBox = ValueOnStack.MakeVBox(NumberType.INT64);
			vBox.v64 = v;
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldc_R4(float v)
		{
			VBox vBox = ValueOnStack.MakeVBox(NumberType.FLOAT);
			vBox.vDF = (double)v;
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldc_R8(double v)
		{
			VBox vBox = ValueOnStack.MakeVBox(NumberType.DOUBLE);
			vBox.vDF = v;
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Stloc(int pos)
		{
			object obj = this.stackCalc.Pop();
			while (this.slotVar.Count <= pos)
			{
				this.slotVar.Add(null);
			}
			bool flag = obj != null && obj.GetType().IsValueType;
			if (flag)
			{
				MethodInfo method = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
				this.slotVar[pos] = method.Invoke(obj, null);
			}
			else
			{
				VBox vBox = this.slotVar[pos] as VBox;
				bool flag2 = vBox == null;
				if (flag2)
				{
					this.slotVar[pos] = obj;
				}
				else
				{
					bool flag3 = obj is VBox;
					if (flag3)
					{
						vBox.Set(obj as VBox);
					}
					else
					{
						vBox.SetDirect(obj);
					}
				}
			}
			this._codepos++;
		}

		public void Ldloc(int pos)
		{
			object obj = this.slotVar[pos];
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				obj = vBox.Clone();
			}
			this.stackCalc.Push(obj);
			this._codepos++;
		}

		public void Ldloca(int pos)
		{
			this.stackCalc.Push(new StackFrame.RefObj(this, pos, StackFrame.RefType.loc));
			this._codepos++;
		}

		public void Ldstr(string text)
		{
			this.stackCalc.Push(text);
			this._codepos++;
		}

		public void Ldarg(int pos)
		{
			object obj = null;
			bool flag = this._params != null;
			if (flag)
			{
				obj = this._params[pos];
			}
			VBox vBox = obj as VBox;
			bool flag2 = vBox != null;
			if (flag2)
			{
				obj = vBox.Clone();
			}
			this.stackCalc.Push(obj);
			this._codepos++;
		}

		public void Ldarga(int pos)
		{
			this.stackCalc.Push(new StackFrame.RefObj(this, pos, StackFrame.RefType.arg));
			this._codepos++;
		}

		public void Ceq()
		{
			object obj = this.stackCalc.Pop();
			object obj2 = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			VBox vBox2 = obj2 as VBox;
			bool flag = obj2 == null || obj == null;
			bool b;
			if (flag)
			{
				b = (obj2 == obj);
			}
			else
			{
				bool flag2 = vBox2 == null || vBox == null;
				if (flag2)
				{
					bool flag3 = vBox2 != null;
					if (flag3)
					{
						vBox = ValueOnStack.MakeVBox(obj.GetType());
						vBox.SetDirect(obj);
						b = vBox2.logic_eq(vBox);
					}
					else
					{
						bool flag4 = vBox != null;
						if (flag4)
						{
							vBox2 = ValueOnStack.MakeVBox(obj2.GetType());
							vBox2.SetDirect(obj2);
							b = vBox2.logic_eq(vBox);
						}
						else
						{
							bool flag5 = obj2 != null;
							if (flag5)
							{
								b = obj2.Equals(obj);
							}
							else
							{
								b = (obj2 == obj);
							}
						}
					}
				}
				else
				{
					b = vBox2.logic_eq(vBox);
				}
			}
			this.stackCalc.Push(ValueOnStack.MakeVBoxBool(b));
			this._codepos++;
		}

		public void Cgt()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			this.stackCalc.Push(ValueOnStack.MakeVBoxBool(vBox.logic_gt(right)));
			this._codepos++;
		}

		public void Cgt_Un()
		{
			object obj = this.stackCalc.Pop();
			object obj2 = this.stackCalc.Pop();
			bool flag = obj == null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.MakeVBoxBool(obj2 != null));
				this._codepos++;
			}
			else
			{
				VBox vBox = this.GetVBox(obj);
				VBox vBox2 = this.GetVBox(obj2);
				this.stackCalc.Push(ValueOnStack.MakeVBoxBool(vBox2.logic_gt_Un(vBox)));
				this._codepos++;
			}
		}

		public void Clt()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			this.stackCalc.Push(ValueOnStack.MakeVBoxBool(vBox.logic_lt(right)));
			this._codepos++;
		}

		public void Clt_Un()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			this.stackCalc.Push(ValueOnStack.MakeVBoxBool(vBox.logic_lt_Un(right)));
			this._codepos++;
		}

		public void Ckfinite()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is float;
			if (flag)
			{
				float f = (float)obj;
				this.stackCalc.Push((float.IsInfinity(f) || float.IsNaN(f)) ? 1 : 0);
			}
			else
			{
				double d = (double)obj;
				this.stackCalc.Push((double.IsInfinity(d) || double.IsNaN(d)) ? 1 : 0);
			}
			this._codepos++;
		}

		public void Add()
		{
			VBox vBox = this.GetVBox(this.stackCalc.Pop());
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			if (flag)
			{
				VBox vBox2 = obj as VBox;
				vBox2.Add(vBox);
				this.stackCalc.Push(vBox2);
			}
			else
			{
				VBox vBox3 = this.GetVBox(obj);
				vBox3.Add(vBox);
				this.stackCalc.Push(vBox3.BoxDefine());
			}
			this._codepos++;
		}

		public VBox GetVBox(object obj)
		{
			bool flag = obj is VBox;
			VBox vBox;
			if (flag)
			{
				vBox = (obj as VBox);
			}
			else
			{
				vBox = ValueOnStack.MakeVBox(obj.GetType());
				vBox.SetDirect(obj);
			}
			return vBox;
		}

		public void Sub()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			if (flag)
			{
				VBox vBox = obj as VBox;
				vBox.Sub(right);
				this.stackCalc.Push(vBox);
			}
			else
			{
				VBox vBox2 = ValueOnStack.MakeVBox(obj.GetType());
				vBox2.SetDirect(obj);
				vBox2.Sub(right);
				this.stackCalc.Push(vBox2.BoxDefine());
			}
			this._codepos++;
		}

		public void Mul()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.Mul(right);
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Div()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.Div(right);
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Div_Un()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.Div(right);
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Rem()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.Mod(right);
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Rem_Un()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.Mod(right);
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Neg()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			if (flag)
			{
				VBox vBox = obj as VBox;
				VBox vBox2 = vBox.Clone();
				vBox2.Neg();
				this.stackCalc.Push(vBox2);
			}
			else
			{
				bool flag2 = obj is int;
				if (flag2)
				{
					this.stackCalc.Push(-(int)obj);
				}
				else
				{
					bool flag3 = obj is long;
					if (flag3)
					{
						this.stackCalc.Push(-(long)obj);
					}
					else
					{
						this.stackCalc.Push(obj);
					}
				}
			}
			this._codepos++;
		}

		public void Conv_I1()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.SBYTE));
			}
			else
			{
				this.stackCalc.Push((sbyte)obj);
			}
			this._codepos++;
		}

		public void Conv_U1()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.BYTE));
			}
			else
			{
				this.stackCalc.Push((byte)obj);
			}
			this._codepos++;
		}

		public void Conv_I2()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.INT16));
			}
			else
			{
				this.stackCalc.Push((short)obj);
			}
			this._codepos++;
		}

		public void Conv_U2()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.UINT16));
			}
			else
			{
				this.stackCalc.Push((ushort)obj);
			}
			this._codepos++;
		}

		public void Conv_I4()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.INT32));
			}
			else
			{
				this.stackCalc.Push((int)obj);
			}
			this._codepos++;
		}

		public void Conv_U4()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.UINT32));
			}
			else
			{
				this.stackCalc.Push((uint)obj);
			}
			this._codepos++;
		}

		public void Conv_I8()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.INT64));
			}
			else
			{
				this.stackCalc.Push((long)obj);
			}
			this._codepos++;
		}

		public void Conv_U8()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.UINT64));
			}
			else
			{
				this.stackCalc.Push((ulong)obj);
			}
			this._codepos++;
		}

		public void Conv_I()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.INT32));
			}
			else
			{
				this.stackCalc.Push((int)obj);
			}
			this._codepos++;
		}

		public void Conv_U()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.UINT32));
			}
			else
			{
				this.stackCalc.Push((uint)obj);
			}
			this._codepos++;
		}

		public void Conv_R4()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.FLOAT));
			}
			else
			{
				bool flag2 = obj.GetType() == typeof(double);
				if (flag2)
				{
					this.stackCalc.Push((float)((double)obj));
				}
				else
				{
					this.stackCalc.Push((float)obj);
				}
			}
			this._codepos++;
		}

		public void Conv_R8()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.DOUBLE));
			}
			else
			{
				this.stackCalc.Push((double)obj);
			}
			this._codepos++;
		}

		public void Conv_R_Un()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.FLOAT));
			}
			else
			{
				this.stackCalc.Push((float)obj);
			}
			this._codepos++;
		}

		public void NewArr(ThreadContext context, Type type)
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			if (flag)
			{
				obj = (obj as VBox).BoxDefine();
			}
			Array item = Array.CreateInstance(type, (int)obj);
			this.stackCalc.Push(item);
			this._codepos++;
		}

		public void LdLen()
		{
			object obj = this.stackCalc.Pop();
			Array array = obj as Array;
			VBox vBox = ValueOnStack.MakeVBox(NumberType.INT32);
			vBox.v32 = array.Length;
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldelema(object obj)
		{
			object obj2 = this.stackCalc.Pop();
			bool flag = obj2 is VBox;
			int index;
			if (flag)
			{
				index = (obj2 as VBox).ToInt();
			}
			else
			{
				index = (int)obj2;
			}
			Array array = this.stackCalc.Pop() as Array;
			this.stackCalc.Push(new StackFrame.RefObj(array, index));
			this._codepos++;
		}

		public void Ldelem_I1()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is sbyte[];
			if (flag2)
			{
				sbyte[] array = obj2 as sbyte[];
				VBox vBox = ValueOnStack.MakeVBox(NumberType.SBYTE);
				vBox.v32 = (int)array[num];
				this.stackCalc.Push(vBox);
			}
			else
			{
				bool flag3 = obj2 is bool[];
				if (!flag3)
				{
					throw new Exception("not support.this array i1");
				}
				bool[] array2 = obj2 as bool[];
				VBox vBox2 = ValueOnStack.MakeVBox(NumberType.BOOL);
				vBox2.v32 = (array2[num] ? 1 : 0);
				this.stackCalc.Push(vBox2);
			}
			this._codepos++;
		}

		public void Ldelem_U1()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is byte[];
			if (flag2)
			{
				byte[] array = obj2 as byte[];
				VBox vBox = ValueOnStack.MakeVBox(NumberType.BYTE);
				vBox.v32 = (int)array[num];
				this.stackCalc.Push(vBox);
				this._codepos++;
			}
			else
			{
				bool flag3 = obj2 is bool[];
				if (flag3)
				{
					bool[] array2 = obj2 as bool[];
					VBox vBox2 = ValueOnStack.MakeVBox(NumberType.BOOL);
					vBox2.v32 = (array2[num] ? 1 : 0);
					this.stackCalc.Push(vBox2);
					this._codepos++;
				}
			}
		}

		public void Ldelem_I2()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			short[] array = this.stackCalc.Pop() as short[];
			VBox vBox = ValueOnStack.MakeVBox(NumberType.INT16);
			vBox.v32 = (int)array[num];
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldelem_U2()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is ushort[];
			if (flag2)
			{
				ushort[] array = obj2 as ushort[];
				VBox vBox = ValueOnStack.MakeVBox(NumberType.UINT16);
				vBox.v32 = (int)array[num];
				this.stackCalc.Push(vBox);
			}
			else
			{
				char[] array2 = obj2 as char[];
				VBox vBox2 = ValueOnStack.MakeVBox(NumberType.CHAR);
				vBox2.v32 = (int)array2[num];
				this.stackCalc.Push(vBox2);
			}
			this._codepos++;
		}

		public void Ldelem_I4()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			int[] array = this.stackCalc.Pop() as int[];
			VBox vBox = ValueOnStack.MakeVBox(NumberType.INT32);
			vBox.v32 = array[num];
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldelem_U4()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			uint[] array = this.stackCalc.Pop() as uint[];
			VBox vBox = ValueOnStack.MakeVBox(NumberType.UINT32);
			vBox.v32 = (int)array[num];
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldelem_I8()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is long[];
			if (flag2)
			{
				long[] array = obj2 as long[];
				VBox vBox = ValueOnStack.MakeVBox(NumberType.INT64);
				vBox.v64 = array[num];
				this.stackCalc.Push(vBox);
			}
			else
			{
				ulong[] array2 = obj2 as ulong[];
				VBox vBox2 = ValueOnStack.MakeVBox(NumberType.INT64);
				vBox2.v64 = (long)array2[num];
				this.stackCalc.Push(vBox2);
			}
			this._codepos++;
		}

		public void Ldelem_I()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			int[] array = this.stackCalc.Pop() as int[];
			VBox vBox = ValueOnStack.MakeVBox(NumberType.INT32);
			vBox.v32 = array[num];
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldelem_R4()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			float[] array = this.stackCalc.Pop() as float[];
			VBox vBox = ValueOnStack.MakeVBox(NumberType.FLOAT);
			vBox.vDF = (double)array[num];
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldelem_R8()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			double[] array = this.stackCalc.Pop() as double[];
			VBox vBox = ValueOnStack.MakeVBox(NumberType.DOUBLE);
			vBox.vDF = array[num];
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Ldelem_Ref()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int index;
			if (flag)
			{
				index = (obj as VBox).ToInt();
			}
			else
			{
				index = (int)obj;
			}
			Array array = this.stackCalc.Pop() as Array;
			this.stackCalc.Push(array.GetValue(index));
			this._codepos++;
		}

		public void Ldelem_Any(object obj)
		{
			object obj2 = this.stackCalc.Pop();
			bool flag = obj2 is VBox;
			int index;
			if (flag)
			{
				index = (obj2 as VBox).ToInt();
			}
			else
			{
				index = (int)obj2;
			}
			Array array = this.stackCalc.Pop() as Array;
			this.stackCalc.Push(array.GetValue(index));
			this._codepos++;
		}

		public void Stelem_I()
		{
			this.Stelem_I4();
		}

		public void Stelem_I1()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)((sbyte)obj);
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is VBox;
			int num2;
			if (flag2)
			{
				num2 = (obj2 as VBox).ToInt();
			}
			else
			{
				num2 = (int)obj2;
			}
			object obj3 = this.stackCalc.Pop();
			bool flag3 = obj3 is sbyte[];
			if (flag3)
			{
				(obj3 as sbyte[])[num2] = (sbyte)num;
			}
			else
			{
				bool flag4 = obj3 is byte[];
				if (flag4)
				{
					(obj3 as byte[])[num2] = (byte)num;
				}
				else
				{
					bool flag5 = obj3 is bool[];
					if (flag5)
					{
						(obj3 as bool[])[num2] = (num > 0);
					}
				}
			}
			this._codepos++;
		}

		public void Stelem_I2()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)((short)obj);
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is VBox;
			int num2;
			if (flag2)
			{
				num2 = (obj2 as VBox).ToInt();
			}
			else
			{
				num2 = (int)obj2;
			}
			object obj3 = this.stackCalc.Pop();
			bool flag3 = obj3 is char[];
			if (flag3)
			{
				(obj3 as char[])[num2] = (char)num;
			}
			else
			{
				bool flag4 = obj3 is short[];
				if (flag4)
				{
					(obj3 as short[])[num2] = (short)num;
				}
				else
				{
					bool flag5 = obj3 is ushort[];
					if (flag5)
					{
						(obj3 as ushort[])[num2] = (ushort)num;
					}
				}
			}
			this._codepos++;
		}

		public void Stelem_I4()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int num;
			if (flag)
			{
				num = (obj as VBox).ToInt();
			}
			else
			{
				num = (int)obj;
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is VBox;
			int num2;
			if (flag2)
			{
				num2 = (obj2 as VBox).ToInt();
			}
			else
			{
				num2 = (int)obj2;
			}
			object obj3 = this.stackCalc.Pop();
			bool flag3 = obj3 is int[];
			if (flag3)
			{
				int[] array = obj3 as int[];
				array[num2] = num;
			}
			else
			{
				bool flag4 = obj3 is uint[];
				if (flag4)
				{
					uint[] array2 = obj3 as uint[];
					array2[num2] = (uint)num;
				}
			}
			this._codepos++;
		}

		public void Stelem_I8()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			long num;
			if (flag)
			{
				num = (obj as VBox).ToInt64();
			}
			else
			{
				num = (long)obj;
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is VBox;
			int num2;
			if (flag2)
			{
				num2 = (obj2 as VBox).ToInt();
			}
			else
			{
				num2 = (int)obj2;
			}
			object obj3 = this.stackCalc.Pop();
			bool flag3 = obj3 is long[];
			if (flag3)
			{
				long[] array = obj3 as long[];
				array[num2] = num;
			}
			else
			{
				bool flag4 = obj3 is ulong[];
				if (flag4)
				{
					ulong[] array2 = obj3 as ulong[];
					array2[num2] = (ulong)num;
				}
			}
			this._codepos++;
		}

		public void Stelem_R4()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			float num;
			if (flag)
			{
				num = (obj as VBox).ToFloat();
			}
			else
			{
				num = (float)obj;
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is VBox;
			int num2;
			if (flag2)
			{
				num2 = (obj2 as VBox).ToInt();
			}
			else
			{
				num2 = (int)obj2;
			}
			float[] array = this.stackCalc.Pop() as float[];
			array[num2] = num;
			this._codepos++;
		}

		public void Stelem_R8()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			double num;
			if (flag)
			{
				num = (obj as VBox).ToDouble();
			}
			else
			{
				num = (double)obj;
			}
			object obj2 = this.stackCalc.Pop();
			bool flag2 = obj2 is VBox;
			int num2;
			if (flag2)
			{
				num2 = (obj2 as VBox).ToInt();
			}
			else
			{
				num2 = (int)obj2;
			}
			double[] array = this.stackCalc.Pop() as double[];
			array[num2] = num;
			this._codepos++;
		}

		public void Stelem_Ref()
		{
			object obj = this.stackCalc.Pop();
			object obj2 = this.stackCalc.Pop();
			bool flag = obj2 is VBox;
			int num;
			if (flag)
			{
				num = (obj2 as VBox).ToInt();
			}
			else
			{
				num = (int)obj2;
			}
			object[] array = this.stackCalc.Pop() as object[];
			array[num] = obj;
			this._codepos++;
		}

		public void Stelem_Any()
		{
			object value = this.stackCalc.Pop();
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			int index;
			if (flag)
			{
				index = (obj as VBox).ToInt();
			}
			else
			{
				index = (int)obj;
			}
			Array array = this.stackCalc.Pop() as Array;
			array.SetValue(value, index);
			this._codepos++;
		}

		public void NewObj(ThreadContext context, IMethod _clrmethod)
		{
			object[] array = null;
			bool flag = _clrmethod is IMethod_Sharp;
			bool flag2 = _clrmethod.ParamList != null;
			if (flag2)
			{
				array = new object[_clrmethod.ParamList.Count];
				for (int i = 0; i < array.Length; i++)
				{
					int num = array.Length - 1 - i;
					ICLRType iCLRType = _clrmethod.ParamList[num];
					object obj = this.stackCalc.Pop();
					bool flag3 = obj is VBox && !flag;
					if (flag3)
					{
						obj = (obj as VBox).BoxDefine();
					}
					bool flag4 = obj is int && iCLRType.TypeForSystem != typeof(int) && iCLRType.TypeForSystem != typeof(object);
					if (flag4)
					{
						VBox vBox = ValueOnStack.MakeVBox(iCLRType);
						bool flag5 = vBox != null;
						if (flag5)
						{
							vBox.SetDirect(obj);
							bool flag6 = flag;
							if (flag6)
							{
								obj = vBox;
							}
							else
							{
								obj = vBox.BoxDefine();
							}
						}
					}
					array[num] = obj;
				}
			}
			object item = _clrmethod.Invoke(context, null, array);
			this.stackCalc.Push(item);
			this._codepos++;
		}

		public void Ldfld(ThreadContext context, IField field)
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				obj = (obj as StackFrame.RefObj).Get();
			}
			object obj2 = field.Get(obj);
			VBox vBox = ValueOnStack.MakeVBox(field.FieldType);
			bool flag2 = vBox != null;
			if (flag2)
			{
				vBox.SetDirect(obj2);
				obj2 = vBox;
			}
			this.stackCalc.Push(obj2);
			this._codepos++;
		}

		public void Ldflda(ThreadContext context, IField field)
		{
			object this2 = this.stackCalc.Pop();
			this.stackCalc.Push(new StackFrame.RefObj(field, this2));
			this._codepos++;
		}

		public void Ldsfld(ThreadContext context, IField field)
		{
			object obj = field.Get(null);
			VBox vBox = ValueOnStack.MakeVBox(field.FieldType);
			bool flag = vBox != null;
			if (flag)
			{
				vBox.SetDirect(obj);
				obj = vBox;
			}
			this.stackCalc.Push(obj);
			this._codepos++;
		}

		public void Ldsflda(ThreadContext context, IField field)
		{
			this.stackCalc.Push(new StackFrame.RefObj(field, null));
			this._codepos++;
		}

		public void Stfld(ThreadContext context, IField field)
		{
			object obj = this.stackCalc.Pop();
			object obj2 = this.stackCalc.Pop();
			bool flag = obj2 is StackFrame.RefObj;
			if (flag)
			{
				bool flag2 = (obj2 as StackFrame.RefObj).Get() == null && !field.isStatic;
				if (flag2)
				{
					(obj2 as StackFrame.RefObj).Set(field.DeclaringType.InitObj());
				}
				obj2 = (obj2 as StackFrame.RefObj).Get();
			}
			bool flag3 = obj is VBox;
			if (flag3)
			{
				obj = (obj as VBox).BoxDefine();
			}
			VBox vBox = ValueOnStack.MakeVBox(field.FieldType);
			bool flag4 = vBox != null;
			if (flag4)
			{
				vBox.SetDirect(obj);
				obj = vBox.BoxDefine();
			}
			field.Set(obj2, obj);
			this._codepos++;
		}

		public void Stsfld(ThreadContext context, IField field)
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			if (flag)
			{
				obj = (obj as VBox).BoxDefine();
			}
			field.Set(null, obj);
			this._codepos++;
		}

		public void Constrained(ThreadContext context, ICLRType obj)
		{
			this._codepos++;
		}

		public void Isinst(ThreadContext context, ICLRType _type)
		{
			object obj = this.stackCalc.Pop();
			bool flag = _type.IsInst(obj);
			if (flag)
			{
				this.stackCalc.Push(obj);
			}
			else
			{
				this.stackCalc.Push(null);
			}
			this._codepos++;
		}

		public void Ldtoken(ThreadContext context, object token)
		{
			this.stackCalc.Push(token);
			this._codepos++;
		}

		public void Conv_Ovf_I1()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.SBYTE));
			}
			else
			{
				this.stackCalc.Push((sbyte)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_U1()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.BYTE));
			}
			else
			{
				this.stackCalc.Push((byte)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_I2()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.INT16));
			}
			else
			{
				this.stackCalc.Push((short)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_U2()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.UINT16));
			}
			else
			{
				this.stackCalc.Push((short)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_I4()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.INT32));
			}
			else
			{
				this.stackCalc.Push((int)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_U4()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.UINT32));
			}
			else
			{
				this.stackCalc.Push((uint)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_I8()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.INT64));
			}
			else
			{
				this.stackCalc.Push((long)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_U8()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.UINT64));
			}
			else
			{
				this.stackCalc.Push((long)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_I()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.INT32));
			}
			else
			{
				this.stackCalc.Push((int)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_U()
		{
			object obj = this.stackCalc.Pop();
			VBox vBox = obj as VBox;
			bool flag = vBox != null;
			if (flag)
			{
				this.stackCalc.Push(ValueOnStack.Convert(vBox, NumberType.UINT32));
			}
			else
			{
				this.stackCalc.Push((uint)obj);
			}
			this._codepos++;
		}

		public void Conv_Ovf_I1_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_U1_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_I2_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_U2_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_I4_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_U4_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_I8_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_U8_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_I_Un()
		{
			throw new NotImplementedException();
		}

		public void Conv_Ovf_U_Un()
		{
			throw new NotImplementedException();
		}

		public void Ldftn(ThreadContext context, IMethod method)
		{
			this.stackCalc.Push(new RefFunc(method, null));
			this._codepos++;
		}

		public void Ldvirtftn(ThreadContext context, IMethod method)
		{
			object this2 = this.stackCalc.Pop();
			this.stackCalc.Push(new RefFunc(method, this2));
			this._codepos++;
		}

		public void Starg(ThreadContext context, int p)
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			if (flag)
			{
				obj = (obj as VBox).Clone();
			}
			this._params[p] = obj;
			this._codepos++;
		}

		public void Calli(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Break(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Ldnull()
		{
			this.stackCalc.Push(null);
			this._codepos++;
		}

		public void Jmp(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Switch(ThreadContext context, int[] index)
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is VBox;
			uint num;
			if (flag)
			{
				num = (obj as VBox).ToUInt();
			}
			else
			{
				bool flag2 = obj is int;
				if (flag2)
				{
					num = (uint)((int)obj);
				}
				else
				{
					num = uint.Parse(obj.ToString());
				}
			}
			bool flag3 = (ulong)num >= (ulong)((long)index.Length);
			if (flag3)
			{
				this._codepos++;
			}
			else
			{
				this._codepos = index[(int)num];
			}
		}

		public void Ldind_I1()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_I1:");
		}

		public void Ldind_U1()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_U1:");
		}

		public void Ldind_I2()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_I2:");
		}

		public void Ldind_U2()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_U2:");
		}

		public void Ldind_I4()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_I4:");
		}

		public void Ldind_U4()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_U4:");
		}

		public void Ldind_I8()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_I8:");
		}

		public void Ldind_I()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_I:");
		}

		public void Ldind_R4()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_R4:");
		}

		public void Ldind_R8()
		{
			object obj = this.stackCalc.Pop();
			bool flag = obj is StackFrame.RefObj;
			if (flag)
			{
				StackFrame.RefObj refObj = obj as StackFrame.RefObj;
				object obj2 = refObj.Get();
				VBox vBox = ValueOnStack.MakeVBox(obj2.GetType());
				vBox.SetDirect(obj2);
				this.stackCalc.Push(vBox);
				this._codepos++;
				return;
			}
			throw new Exception("not impl Ldind_R8:");
		}

		public void Ldind_Ref()
		{
			throw new Exception("not impl Ldind_Ref:");
		}

		public void Stind_Ref(ThreadContext context, object obj)
		{
			object obj2 = this.stackCalc.Pop();
			object obj3 = this.stackCalc.Pop();
			bool flag = obj3 is StackFrame.RefObj;
			if (flag)
			{
				(obj3 as StackFrame.RefObj).Set((obj2 is VBox) ? (obj2 as VBox).BoxDefine() : obj2);
			}
			this._codepos++;
		}

		public void Stind_I1(ThreadContext context, object obj)
		{
			object obj2 = this.stackCalc.Pop();
			object obj3 = this.stackCalc.Pop();
			bool flag = obj3 is StackFrame.RefObj;
			if (flag)
			{
				(obj3 as StackFrame.RefObj).Set((obj2 is VBox) ? (obj2 as VBox).BoxDefine() : obj2);
			}
			this._codepos++;
		}

		public void Stind_I2(ThreadContext context, object obj)
		{
			object obj2 = this.stackCalc.Pop();
			object obj3 = this.stackCalc.Pop();
			bool flag = obj3 is StackFrame.RefObj;
			if (flag)
			{
				(obj3 as StackFrame.RefObj).Set((obj2 is VBox) ? (obj2 as VBox).BoxDefine() : obj2);
			}
			this._codepos++;
		}

		public void Stind_I4(ThreadContext context, object obj)
		{
			object obj2 = this.stackCalc.Pop();
			object obj3 = this.stackCalc.Pop();
			bool flag = obj3 is StackFrame.RefObj;
			if (flag)
			{
				(obj3 as StackFrame.RefObj).Set((obj2 is VBox) ? (obj2 as VBox).BoxDefine() : obj2);
			}
			this._codepos++;
		}

		public void Stind_I8(ThreadContext context, object obj)
		{
			object obj2 = this.stackCalc.Pop();
			object obj3 = this.stackCalc.Pop();
			bool flag = obj3 is StackFrame.RefObj;
			if (flag)
			{
				(obj3 as StackFrame.RefObj).Set((obj2 is VBox) ? (obj2 as VBox).BoxDefine() : obj2);
			}
			this._codepos++;
		}

		public void Stind_R4(ThreadContext context, object obj)
		{
			object obj2 = this.stackCalc.Pop();
			object obj3 = this.stackCalc.Pop();
			bool flag = obj3 is StackFrame.RefObj;
			if (flag)
			{
				(obj3 as StackFrame.RefObj).Set((obj2 is VBox) ? (obj2 as VBox).BoxDefine() : obj2);
			}
			this._codepos++;
		}

		public void Stind_R8(ThreadContext context, object obj)
		{
			object obj2 = this.stackCalc.Pop();
			object obj3 = this.stackCalc.Pop();
			bool flag = obj3 is StackFrame.RefObj;
			if (flag)
			{
				(obj3 as StackFrame.RefObj).Set((obj2 is VBox) ? (obj2 as VBox).BoxDefine() : obj2);
			}
			this._codepos++;
		}

		public void And()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.And(right);
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Or()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.Or(right);
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Xor()
		{
			VBox right = this.stackCalc.Pop() as VBox;
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.Xor(right);
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Shl(ThreadContext context, object obj)
		{
			VBox vBox = this.stackCalc.Pop() as VBox;
			VBox vBox2 = this.stackCalc.Pop() as VBox;
			vBox2.v32 <<= vBox.v32;
			this.stackCalc.Push(vBox2);
			this._codepos++;
		}

		public void Shr(ThreadContext context, object obj)
		{
			VBox vBox = this.stackCalc.Pop() as VBox;
			VBox vBox2 = this.stackCalc.Pop() as VBox;
			vBox2.v32 >>= vBox.v32;
			this.stackCalc.Push(vBox2);
			this._codepos++;
		}

		public void Shr_Un(ThreadContext context, object obj)
		{
			VBox vBox = this.stackCalc.Pop() as VBox;
			VBox vBox2 = this.stackCalc.Pop() as VBox;
			vBox2.v32 >>= vBox.v32;
			this.stackCalc.Push(vBox2);
			this._codepos++;
		}

		public void Not()
		{
			VBox vBox = this.stackCalc.Pop() as VBox;
			vBox.Not();
			this.stackCalc.Push(vBox);
			this._codepos++;
		}

		public void Cpobj(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Ldobj(ThreadContext context, object obj)
		{
			StackFrame.RefObj refObj = this.stackCalc.Pop() as StackFrame.RefObj;
			this.stackCalc.Push(refObj.Get());
			this._codepos++;
		}

		public void Castclass(ThreadContext context, ICLRType _type)
		{
			bool flag = _type is ICLRType_System;
			if (flag)
			{
				object obj = this.stackCalc.Peek();
				bool flag2 = obj != null;
				if (flag2)
				{
					Type typeForSystem = (_type as ICLRType_System).TypeForSystem;
					Type type = obj.GetType();
					bool flag3 = type != typeForSystem && !obj.GetType().IsSubclassOf(typeForSystem);
					if (flag3)
					{
						throw new Exception("不可转换");
					}
				}
			}
			this._codepos++;
		}

		public void Throw(ThreadContext context, object obj)
		{
			Exception ex = this.stackCalc.Pop() as Exception;
			throw ex;
		}

		public void Stobj(ThreadContext context, object obj)
		{
			object obj2 = this.stackCalc.Pop();
			StackFrame.RefObj refObj = this.stackCalc.Pop() as StackFrame.RefObj;
			refObj.Set(obj2);
			this._codepos++;
		}

		public void Refanyval(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Mkrefany(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Add_Ovf(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Add_Ovf_Un(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Mul_Ovf(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Mul_Ovf_Un(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Sub_Ovf(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Sub_Ovf_Un(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Endfinally(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Stind_I(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Arglist(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Localloc(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Endfilter(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Unaligned(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Volatile()
		{
			this._codepos++;
		}

		public void Tail(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Initobj(ThreadContext context, ICLRType _type)
		{
			StackFrame.RefObj refObj = this.stackCalc.Pop() as StackFrame.RefObj;
			object obj = _type.InitObj();
			refObj.Set(obj);
			this._codepos++;
		}

		public void Cpblk(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Initblk(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void No(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Rethrow(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Sizeof(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Refanytype(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}

		public void Readonly(ThreadContext context, object obj)
		{
			Type type = obj.GetType();
			throw new NotImplementedException(type.ToString());
		}
	}
}
