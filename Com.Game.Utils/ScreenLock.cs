using System;
using System.Collections.Generic;

namespace Com.Game.Utils
{
	public static class ScreenLock
	{
		private static readonly List<string> _lockList = new List<string>();

		public static void Lock(string key, string msg, bool normal = false)
		{
			if (!ScreenLock._lockList.Contains(key))
			{
				ScreenLock._lockList.Add(key);
				MobaMessageManagerTools.BeginWaiting_manual(key, msg, normal, 3.40282347E+38f, true);
			}
		}

		public static void Unlock(string key)
		{
			if (ScreenLock._lockList.Contains(key))
			{
				ScreenLock._lockList.RemoveAll((string x) => x == key);
				MobaMessageManagerTools.EndWaiting_manual(key);
			}
		}
	}
}
