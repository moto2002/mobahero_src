using System;
using System.Threading;

namespace MobaHeros.Server
{
	public static class ThreadUtils
	{
		public static int MainThreadId
		{
			get;
			private set;
		}

		public static bool IsMainThread
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId == ThreadUtils.MainThreadId;
			}
		}

		public static void Init()
		{
			ThreadUtils.MainThreadId = Thread.CurrentThread.ManagedThreadId;
		}
	}
}
