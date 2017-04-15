using System;
using UnityEngine;

public class YJLog
{
	public static void log_ex(object message)
	{
		int millisecond = DateTime.Now.Millisecond;
		string text;
		if (millisecond < 10)
		{
			text = "00" + millisecond;
		}
		else if (millisecond < 100)
		{
			text = "0" + millisecond;
		}
		else
		{
			text = millisecond.ToString();
		}
		string arg = string.Concat(new object[]
		{
			DateTime.Now.Year,
			"-",
			DateTime.Now.Month,
			"-",
			DateTime.Now.Day,
			" ",
			DateTime.Now.Hour,
			":",
			DateTime.Now.Minute,
			":",
			DateTime.Now.Second,
			".",
			text,
			"  "
		});
		Debug.LogError(arg + message);
	}
}
