using System;
using System.Threading;
using UnityEngine;

public class CallbackDelegate : BuglyCallback
{
	private static readonly CallbackDelegate _instance;

	public static CallbackDelegate Instance
	{
		get
		{
			return CallbackDelegate._instance;
		}
	}

	private CallbackDelegate()
	{
	}

	static CallbackDelegate()
	{
		CallbackDelegate._instance = new CallbackDelegate();
	}

	public override void OnApplicationLogCallbackHandler(string condition, string stackTrace, LogType type)
	{
		Console.Write("--------- OnApplicationLogCallbackHandler ---------\n");
		Console.Write("Current thread: {0}", Thread.CurrentThread.ManagedThreadId);
		Console.WriteLine();
		Console.Write("[{0}] - {1}\n{2}", type.ToString(), condition, stackTrace);
		Console.Write("--------- OnApplicationLogCallbackHandler ---------");
		Console.WriteLine();
	}
}
