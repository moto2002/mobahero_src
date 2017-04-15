using System;
using System.Collections.Generic;
using UnityEngine;

public class LSDebug
{
	private class FuncCall
	{
		public string func;

		public DateTime callTiming;

		public FuncCall(string func, DateTime callTiming)
		{
			this.func = func;
			this.callTiming = callTiming;
		}
	}

	public const bool PRINT_CM_TIME = false;

	private const string prefix = "ls:-----------------------------------------------------------------> ";

	private static Stack<LSDebug.FuncCall> funcStacks = new Stack<LSDebug.FuncCall>();

	private static Dictionary<string, DateTime> _timers = new Dictionary<string, DateTime>();

	public static void startFunc(string keyword)
	{
		LSDebug.funcStacks.Push(new LSDebug.FuncCall(keyword, DateTime.Now));
	}

	public static void finishFunc()
	{
		LSDebug.FuncCall funcCall = LSDebug.funcStacks.Pop();
		LSDebug.log(string.Format("function call: {0,-50} takes time:{1,-10}", funcCall.func, (DateTime.Now - funcCall.callTiming).TotalSeconds));
	}

	public static void log(string str)
	{
	}

	public static void warn(string str)
	{
		Debug.LogWarning("ls:-----------------------------------------------------------------> " + str);
	}

	public static void error(string str)
	{
		Debug.LogError("ls:-----------------------------------------------------------------> " + str);
	}

	public static void resetTimer(string mark)
	{
		if (LSDebug._timers.ContainsKey(mark))
		{
			LSDebug._timers[mark] = DateTime.Now;
		}
		else
		{
			LSDebug._timers.Add(mark, DateTime.Now);
		}
	}

	public static void resetTimer(int mark)
	{
		LSDebug.resetTimer(mark.ToString());
	}

	public static double getInterval(string marker, bool clearTimer = false)
	{
		double result;
		if (!LSDebug._timers.ContainsKey(marker))
		{
			Debug.LogError(string.Format("未设置定时器:{0}", marker));
			result = 0.0;
		}
		else
		{
			double totalSeconds = (DateTime.Now - LSDebug._timers[marker]).TotalSeconds;
			if (clearTimer)
			{
				LSDebug._timers.Remove(marker);
			}
			result = totalSeconds;
		}
		return result;
	}

	public static double getInterval(int marker, bool clearTimer = false)
	{
		return LSDebug.getInterval(marker.ToString(), clearTimer);
	}

	public static bool isTimer(int mark)
	{
		return LSDebug._timers.ContainsKey(mark.ToString());
	}

	public static bool isTimer(string mark)
	{
		return LSDebug._timers.ContainsKey(mark);
	}
}
