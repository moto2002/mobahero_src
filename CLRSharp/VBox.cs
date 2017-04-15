using System;

namespace CLRSharp
{
	public class VBox
	{
		public int refcount = 0;

		public NumberOnStack typeStack;

		public NumberType type;

		public int v32;

		public long v64;

		public double vDF;

		public VBox(NumberOnStack typeStack, NumberType thistype)
		{
			this.typeStack = typeStack;
			this.type = thistype;
		}

		public VBox Clone()
		{
			VBox vBox = ValueOnStack.MakeVBox(this.type);
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				vBox.v32 = this.v32;
				break;
			case NumberOnStack.Int64:
				vBox.v64 = this.v64;
				break;
			case NumberOnStack.Double:
				vBox.vDF = this.vDF;
				break;
			}
			return vBox;
		}

		public object BoxStack()
		{
			object result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = this.v32;
				break;
			case NumberOnStack.Int64:
				result = this.v64;
				break;
			case NumberOnStack.Double:
				result = this.vDF;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public object BoxDefine()
		{
			object result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				switch (this.type)
				{
				case NumberType.SBYTE:
					result = (sbyte)this.v32;
					return result;
				case NumberType.BYTE:
					result = (byte)this.v32;
					return result;
				case NumberType.INT16:
					result = (short)this.v32;
					return result;
				case NumberType.UINT16:
					result = (ushort)this.v32;
					return result;
				case NumberType.INT32:
					result = this.v32;
					return result;
				case NumberType.UINT32:
					result = (uint)this.v32;
					return result;
				case NumberType.INT64:
					result = (long)this.v32;
					return result;
				case NumberType.UINT64:
					result = (ulong)((long)this.v32);
					return result;
				case NumberType.FLOAT:
					result = (float)this.v32;
					return result;
				case NumberType.DOUBLE:
					result = (double)this.v32;
					return result;
				case NumberType.CHAR:
					result = (char)this.v32;
					return result;
				case NumberType.BOOL:
					result = (this.v32 > 0);
					return result;
				case NumberType.ENUM:
					result = this.v32;
					return result;
				}
				result = null;
				break;
			case NumberOnStack.Int64:
				switch (this.type)
				{
				case NumberType.SBYTE:
					result = (sbyte)this.v64;
					return result;
				case NumberType.BYTE:
					result = (byte)this.v64;
					return result;
				case NumberType.INT16:
					result = (short)this.v64;
					return result;
				case NumberType.UINT16:
					result = (ushort)this.v64;
					return result;
				case NumberType.INT32:
					result = (int)this.v64;
					return result;
				case NumberType.UINT32:
					result = (uint)this.v64;
					return result;
				case NumberType.INT64:
					result = this.v64;
					return result;
				case NumberType.UINT64:
					result = (ulong)this.v64;
					return result;
				case NumberType.FLOAT:
					result = (float)this.v64;
					return result;
				case NumberType.DOUBLE:
					result = (double)this.v64;
					return result;
				case NumberType.CHAR:
					result = (char)this.v64;
					return result;
				case NumberType.BOOL:
					result = (this.v64 > 0L);
					return result;
				}
				result = null;
				break;
			case NumberOnStack.Double:
				switch (this.type)
				{
				case NumberType.SBYTE:
					result = (sbyte)this.vDF;
					return result;
				case NumberType.BYTE:
					result = (byte)this.vDF;
					return result;
				case NumberType.INT16:
					result = (short)this.vDF;
					return result;
				case NumberType.UINT16:
					result = (ushort)this.vDF;
					return result;
				case NumberType.INT32:
					result = (int)this.vDF;
					return result;
				case NumberType.UINT32:
					result = (uint)this.vDF;
					return result;
				case NumberType.INT64:
					result = (long)this.vDF;
					return result;
				case NumberType.UINT64:
					result = (ulong)this.vDF;
					return result;
				case NumberType.FLOAT:
					result = (float)this.vDF;
					return result;
				case NumberType.DOUBLE:
					result = this.vDF;
					return result;
				case NumberType.CHAR:
					result = (char)this.vDF;
					return result;
				case NumberType.BOOL:
					result = (this.vDF > 0.0);
					return result;
				}
				result = null;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public void And(VBox right)
		{
			NumberOnStack numberOnStack = this.typeStack;
			if (numberOnStack != NumberOnStack.Int32)
			{
				if (numberOnStack == NumberOnStack.Int64)
				{
					this.v64 &= right.v64;
				}
			}
			else
			{
				this.v32 &= right.v32;
			}
		}

		public void Or(VBox right)
		{
			NumberOnStack numberOnStack = this.typeStack;
			if (numberOnStack != NumberOnStack.Int32)
			{
				if (numberOnStack == NumberOnStack.Int64)
				{
					this.v64 |= right.v64;
				}
			}
			else
			{
				this.v32 |= right.v32;
			}
		}

		public void Xor(VBox right)
		{
			NumberOnStack numberOnStack = this.typeStack;
			if (numberOnStack != NumberOnStack.Int32)
			{
				if (numberOnStack == NumberOnStack.Int64)
				{
					this.v64 ^= right.v64;
				}
			}
			else
			{
				this.v32 ^= right.v32;
			}
		}

		public void Not()
		{
			NumberOnStack numberOnStack = this.typeStack;
			if (numberOnStack != NumberOnStack.Int32)
			{
				if (numberOnStack == NumberOnStack.Int64)
				{
					this.v64 = ~this.v64;
				}
			}
			else
			{
				this.v32 = ~this.v32;
			}
		}

		public void Add(VBox right)
		{
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				this.v32 += right.v32;
				break;
			case NumberOnStack.Int64:
				this.v64 += right.v64;
				break;
			case NumberOnStack.Double:
				this.vDF += right.vDF;
				break;
			}
		}

		public void Sub(VBox right)
		{
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				this.v32 -= right.v32;
				break;
			case NumberOnStack.Int64:
				this.v64 -= right.v64;
				break;
			case NumberOnStack.Double:
				this.vDF -= right.vDF;
				break;
			}
		}

		public void Mul(VBox right)
		{
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				this.v32 *= right.v32;
				break;
			case NumberOnStack.Int64:
				this.v64 *= right.v64;
				break;
			case NumberOnStack.Double:
				this.vDF *= right.vDF;
				break;
			}
		}

		public void Div(VBox right)
		{
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				this.v32 /= right.v32;
				break;
			case NumberOnStack.Int64:
				this.v64 /= right.v64;
				break;
			case NumberOnStack.Double:
				this.vDF /= right.vDF;
				break;
			}
		}

		public void Neg()
		{
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				this.v32 = -this.v32;
				break;
			case NumberOnStack.Int64:
				this.v64 = -this.v64;
				break;
			case NumberOnStack.Double:
				this.vDF = -this.vDF;
				break;
			}
		}

		public void Mod(VBox right)
		{
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				this.v32 %= right.v32;
				break;
			case NumberOnStack.Int64:
				this.v64 %= right.v64;
				break;
			case NumberOnStack.Double:
				this.vDF %= right.vDF;
				break;
			}
		}

		public VBox Mod_New(VBox right)
		{
			VBox vBox = ValueOnStack.MakeVBox(this.type);
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				vBox.v32 = this.v32 % right.v32;
				break;
			case NumberOnStack.Int64:
				vBox.v64 = this.v64 % right.v64;
				break;
			case NumberOnStack.Double:
				vBox.vDF = this.vDF % right.vDF;
				break;
			}
			return vBox;
		}

		public void SetDirect(object value)
		{
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
			{
				bool flag = value is bool;
				if (flag)
				{
					this.v32 = (((bool)value) ? 1 : 0);
				}
				else
				{
					bool flag2 = value is int;
					if (flag2)
					{
						this.v32 = (int)value;
					}
					else
					{
						bool flag3 = value is short;
						if (flag3)
						{
							this.v32 = (int)((short)value);
						}
						else
						{
							bool flag4 = value is char;
							if (flag4)
							{
								this.v32 = (int)((char)value);
							}
							else
							{
								this.v32 = (int)Convert.ToDecimal(value);
							}
						}
					}
				}
				break;
			}
			case NumberOnStack.Int64:
			{
				bool flag5 = value is long;
				if (flag5)
				{
					this.v64 = (long)value;
				}
				else
				{
					bool flag6 = value is ulong;
					if (flag6)
					{
						this.v64 = (long)((ulong)value);
					}
					else
					{
						this.v64 = (long)Convert.ToDecimal(value);
					}
				}
				break;
			}
			case NumberOnStack.Double:
				this.vDF = (double)Convert.ToDecimal(value);
				break;
			}
		}

		public void Set(VBox value)
		{
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
			{
				bool flag = value.typeStack == this.typeStack;
				if (flag)
				{
					this.v32 = value.v32;
				}
				else
				{
					this.v32 = value.ToInt();
				}
				break;
			}
			case NumberOnStack.Int64:
			{
				bool flag2 = value.typeStack == this.typeStack;
				if (flag2)
				{
					this.v64 = value.v64;
				}
				else
				{
					this.v64 = value.ToInt64();
				}
				break;
			}
			case NumberOnStack.Double:
			{
				bool flag3 = value.typeStack == this.typeStack;
				if (flag3)
				{
					this.vDF = value.vDF;
				}
				else
				{
					this.vDF = value.ToDouble();
				}
				break;
			}
			}
		}

		public bool logic_eq(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 == right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 == right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF == right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_ne(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 != right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 != right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF != right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_ne_Un(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 != right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 != right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF != right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_ge(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 >= right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 >= right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF >= right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_ge_Un(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 >= right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 >= right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF >= right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_le(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 <= right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 <= right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF <= right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_le_Un(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 <= right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 <= right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF <= right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_gt(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 > right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 > right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF > right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_gt_Un(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 > right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 > right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF > right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_lt(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 < right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 < right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF < right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool logic_lt_Un(VBox right)
		{
			bool result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (this.v32 < right.v32);
				break;
			case NumberOnStack.Int64:
				result = (this.v64 < right.v64);
				break;
			case NumberOnStack.Double:
				result = (this.vDF < right.vDF);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public bool ToBool()
		{
			NumberOnStack numberOnStack = this.typeStack;
			bool result;
			if (numberOnStack != NumberOnStack.Int32)
			{
				result = (numberOnStack == NumberOnStack.Int64 && this.v64 > 0L);
			}
			else
			{
				result = (this.v32 > 0);
			}
			return result;
		}

		public char ToChar()
		{
			char result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (char)this.v32;
				break;
			case NumberOnStack.Int64:
				result = (char)this.v64;
				break;
			case NumberOnStack.Double:
				result = (char)this.vDF;
				break;
			default:
				result = '\0';
				break;
			}
			return result;
		}

		public byte ToByte()
		{
			byte result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (byte)this.v32;
				break;
			case NumberOnStack.Int64:
				result = (byte)this.v64;
				break;
			case NumberOnStack.Double:
				result = (byte)this.vDF;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public sbyte ToSByte()
		{
			sbyte result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (sbyte)this.v32;
				break;
			case NumberOnStack.Int64:
				result = (sbyte)this.v64;
				break;
			case NumberOnStack.Double:
				result = (sbyte)this.vDF;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public short ToInt16()
		{
			short result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (short)this.v32;
				break;
			case NumberOnStack.Int64:
				result = (short)this.v64;
				break;
			case NumberOnStack.Double:
				result = (short)this.vDF;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public ushort ToUInt16()
		{
			ushort result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (ushort)this.v32;
				break;
			case NumberOnStack.Int64:
				result = (ushort)this.v64;
				break;
			case NumberOnStack.Double:
				result = (ushort)this.vDF;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public int ToInt()
		{
			int result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = this.v32;
				break;
			case NumberOnStack.Int64:
				result = (int)this.v64;
				break;
			case NumberOnStack.Double:
				result = (int)this.vDF;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public uint ToUInt()
		{
			uint result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (uint)this.v32;
				break;
			case NumberOnStack.Int64:
				result = (uint)this.v64;
				break;
			case NumberOnStack.Double:
				result = (uint)this.vDF;
				break;
			default:
				result = 0u;
				break;
			}
			return result;
		}

		public long ToInt64()
		{
			long result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (long)this.v32;
				break;
			case NumberOnStack.Int64:
				result = this.v64;
				break;
			case NumberOnStack.Double:
				result = (long)this.vDF;
				break;
			default:
				result = 0L;
				break;
			}
			return result;
		}

		public ulong ToUInt64()
		{
			ulong result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (ulong)((long)this.v32);
				break;
			case NumberOnStack.Int64:
				result = (ulong)this.v64;
				break;
			case NumberOnStack.Double:
				result = (ulong)this.vDF;
				break;
			default:
				result = 0uL;
				break;
			}
			return result;
		}

		public float ToFloat()
		{
			float result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (float)this.v32;
				break;
			case NumberOnStack.Int64:
				result = (float)this.v64;
				break;
			case NumberOnStack.Double:
				result = (float)this.vDF;
				break;
			default:
				result = 0f;
				break;
			}
			return result;
		}

		public double ToDouble()
		{
			double result;
			switch (this.typeStack)
			{
			case NumberOnStack.Int32:
				result = (double)this.v32;
				break;
			case NumberOnStack.Int64:
				result = (double)this.v64;
				break;
			case NumberOnStack.Double:
				result = this.vDF;
				break;
			default:
				result = 0.0;
				break;
			}
			return result;
		}
	}
}
