using System;

namespace MobaProtocol.Helper
{
	public static class TypeHelper
	{
		public static byte getByte(this int i)
		{
			return (byte)i;
		}

		public static int getInt32(this string str)
		{
			return Convert.ToInt32(str);
		}

		public static string ToStringValue(this byte[] bts)
		{
			return string.Empty;
		}
	}
}
