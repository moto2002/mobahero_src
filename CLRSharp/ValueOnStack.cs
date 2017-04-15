using System;
using System.Collections.Generic;

namespace CLRSharp
{
	public class ValueOnStack
	{
		private static Dictionary<Type, NumberType> typecode = null;

		[ThreadStatic]
		public static Queue<VBox> unusedVBox = new Queue<VBox>();

		public static NumberType GetTypeCode(Type type)
		{
			bool flag = ValueOnStack.typecode == null;
			if (flag)
			{
				ValueOnStack.typecode = new Dictionary<Type, NumberType>();
				ValueOnStack.typecode[typeof(bool)] = NumberType.BOOL;
				ValueOnStack.typecode[typeof(sbyte)] = NumberType.SBYTE;
				ValueOnStack.typecode[typeof(byte)] = NumberType.BYTE;
				ValueOnStack.typecode[typeof(short)] = NumberType.INT16;
				ValueOnStack.typecode[typeof(ushort)] = NumberType.UINT16;
				ValueOnStack.typecode[typeof(int)] = NumberType.INT32;
				ValueOnStack.typecode[typeof(uint)] = NumberType.UINT32;
				ValueOnStack.typecode[typeof(long)] = NumberType.INT64;
				ValueOnStack.typecode[typeof(ulong)] = NumberType.UINT64;
				ValueOnStack.typecode[typeof(float)] = NumberType.FLOAT;
				ValueOnStack.typecode[typeof(double)] = NumberType.DOUBLE;
				ValueOnStack.typecode[typeof(IntPtr)] = NumberType.INTPTR;
				ValueOnStack.typecode[typeof(UIntPtr)] = NumberType.UINTPTR;
				ValueOnStack.typecode[typeof(decimal)] = NumberType.DECIMAL;
				ValueOnStack.typecode[typeof(char)] = NumberType.CHAR;
			}
			bool isEnum = type.IsEnum;
			NumberType result;
			if (isEnum)
			{
				result = NumberType.ENUM;
			}
			else
			{
				NumberType numberType = NumberType.IsNotNumber;
				ValueOnStack.typecode.TryGetValue(type, out numberType);
				result = numberType;
			}
			return result;
		}

		public static VBox MakeVBox(ICLRType type)
		{
			bool flag = type == null;
			VBox result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = ValueOnStack.MakeVBox(type.TypeForSystem);
			}
			return result;
		}

		public static VBox MakeVBoxBool(bool b)
		{
			VBox vBox = ValueOnStack.MakeVBox(NumberType.BOOL);
			vBox.v32 = (b ? 1 : 0);
			return vBox;
		}

		public static VBox MakeVBox(Type type)
		{
			NumberType typeCode = ValueOnStack.GetTypeCode(type);
			return ValueOnStack.MakeVBox(typeCode);
		}

		public static VBox MakeVBox(NumberType code)
		{
			VBox result;
			switch (code)
			{
			case NumberType.SBYTE:
			case NumberType.BYTE:
			case NumberType.INT16:
			case NumberType.UINT16:
			case NumberType.INT32:
			case NumberType.UINT32:
			case NumberType.CHAR:
			case NumberType.BOOL:
			case NumberType.ENUM:
			{
				bool flag = ValueOnStack.unusedVBox.Count > 0;
				if (flag)
				{
					VBox vBox = ValueOnStack.unusedVBox.Dequeue();
					vBox.typeStack = NumberOnStack.Int32;
					vBox.type = code;
					result = vBox;
					return result;
				}
				result = new VBox(NumberOnStack.Int32, code);
				return result;
			}
			case NumberType.INT64:
			case NumberType.UINT64:
			{
				bool flag2 = ValueOnStack.unusedVBox.Count > 0;
				if (flag2)
				{
					VBox vBox2 = ValueOnStack.unusedVBox.Dequeue();
					vBox2.typeStack = NumberOnStack.Int64;
					vBox2.type = code;
					result = vBox2;
					return result;
				}
				result = new VBox(NumberOnStack.Int64, code);
				return result;
			}
			case NumberType.FLOAT:
			case NumberType.DOUBLE:
			{
				bool flag3 = ValueOnStack.unusedVBox.Count > 0;
				if (flag3)
				{
					VBox vBox3 = ValueOnStack.unusedVBox.Dequeue();
					vBox3.typeStack = NumberOnStack.Double;
					vBox3.type = code;
					result = vBox3;
					return result;
				}
				result = new VBox(NumberOnStack.Double, code);
				return result;
			}
			}
			result = null;
			return result;
		}

		public static VBox Convert(VBox box, NumberType type)
		{
			VBox vBox = ValueOnStack.MakeVBox(type);
			vBox.Set(box);
			return vBox;
		}

		public static void UnUse(VBox box)
		{
			box.refcount = 0;
			ValueOnStack.unusedVBox.Enqueue(box);
		}
	}
}
