using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Utils
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct EnumEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct
	{
		private int ToInt(TEnum en)
		{
			return EnumInt32ToInt.Convert<TEnum>(en);
		}

		public bool Equals(TEnum firstEnum, TEnum secondEnum)
		{
			return this.ToInt(firstEnum) == this.ToInt(secondEnum);
		}

		public int GetHashCode(TEnum firstEnum)
		{
			return this.ToInt(firstEnum);
		}
	}
}
