using System;
using UnityEngine;

public static class Reminder
{
	public static void ReminderStr(string str)
	{
		Debug.LogError("(策划的锅！！!)" + str);
	}

	public static void ReminderStr_CX(string str)
	{
		Debug.LogError("(程序的锅！！!)" + str);
	}
}
