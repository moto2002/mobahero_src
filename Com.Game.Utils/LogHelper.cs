using System;
using System.Diagnostics;

namespace Com.Game.Utils
{
	public static class LogHelper
	{
		[Conditional("MOBA_DEBUG")]
		public static void LogState(object msg)
		{
		}
	}
}
