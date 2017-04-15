using System;
using System.Runtime.InteropServices;

namespace Utils
{
	public static class CrashHelper
	{
		[DllImport("util", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		public static extern void read_null();

		[DllImport("util", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		public static extern void write_null();
	}
}
