using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;
using UnityEngine;

public static class Tools_TimeCheck
{
	public static DateTime ServerCurrentTime
	{
		get
		{
			if (ModelManager.Instance.Get_ServerTime_IsCorrected())
			{
				return ModelManager.Instance.Get_ServerTimeCorrected();
			}
			TimeSpan t = ModelManager.Instance.Get_loginTime_diff_X();
			DateTime d = ModelManager.Instance.Get_loginTime_DataTime();
			return d + t;
		}
	}

	public static TimeSpan TimeDValueCount(this ToolsFacade facade, DateTime _targetTime)
	{
		return _targetTime - Tools_TimeCheck.ServerCurrentTime;
	}

	public static bool IsPastTime(this ToolsFacade facade, DateTime _targetTime)
	{
		return _targetTime < Tools_TimeCheck.ServerCurrentTime;
	}

	public static TimeSpan SpanFromNowToGet(this ToolsFacade facade, DateTime startTime)
	{
		TimeSpan result = Tools_TimeCheck.ServerCurrentTime - startTime;
		if (result.TotalSeconds < 0.0)
		{
			return new TimeSpan(0, 0, 0, 0);
		}
		return result;
	}

	public static bool IsRuledDayOfWeek(this ToolsFacade facade, string ruleStr)
	{
		return string.IsNullOrEmpty(ruleStr) || ruleStr.Equals("[]") || ruleStr.Contains(((int)Tools_TimeCheck.ServerCurrentTime.DayOfWeek).ToString());
	}

	public static bool IsRuledDayOfWeek_Tomorrow(this ToolsFacade facade, string ruleStr)
	{
		if (string.IsNullOrEmpty(ruleStr) || ruleStr.Equals("[]"))
		{
			return true;
		}
		DateTime serverCurrentTime = Tools_TimeCheck.ServerCurrentTime;
		serverCurrentTime.AddDays(1.0);
		return ruleStr.Contains(((int)serverCurrentTime.DayOfWeek).ToString());
	}

	public static bool IsInTimeInterval(this ToolsFacade facade, DateTime fromTime, DateTime toTime, out bool hasDayCompensation)
	{
		DateTime serverCurrentTime = Tools_TimeCheck.ServerCurrentTime;
		DateTime t = new DateTime(1, 1, 1, serverCurrentTime.Hour, serverCurrentTime.Minute, 0);
		if (!(toTime < fromTime))
		{
			hasDayCompensation = false;
			return t >= fromTime && t < toTime;
		}
		if (t >= fromTime)
		{
			hasDayCompensation = false;
			return true;
		}
		if (t < toTime)
		{
			hasDayCompensation = true;
			return true;
		}
		hasDayCompensation = false;
		return false;
	}

	public static bool IsInTimeInterval(this ToolsFacade facade, DateTime fromTime, DateTime toTime)
	{
		DateTime serverCurrentTime = Tools_TimeCheck.ServerCurrentTime;
		return serverCurrentTime > fromTime && serverCurrentTime < toTime;
	}

	public static bool IsInTimeInterval(this ToolsFacade facade, TimeFormat fromTime, TimeFormat toTime)
	{
		DateTime serverCurrentTime = Tools_TimeCheck.ServerCurrentTime;
		DateTime t = new DateTime(serverCurrentTime.Year, serverCurrentTime.Month, serverCurrentTime.Day, fromTime.hour, fromTime.minute, fromTime.second);
		DateTime t2 = new DateTime(serverCurrentTime.Year, serverCurrentTime.Month, serverCurrentTime.Day, toTime.hour, toTime.minute, toTime.second);
		return serverCurrentTime > t && serverCurrentTime < t2;
	}

	public static bool IsInTimeInterval(this ToolsFacade facade, string timeStr, out bool hasDayCompensation)
	{
		if (string.IsNullOrEmpty(timeStr))
		{
			Debug.LogError("Null Parameter.");
			hasDayCompensation = false;
			return false;
		}
		if (timeStr.Equals("[]"))
		{
			hasDayCompensation = false;
			return true;
		}
		string[] array = timeStr.Split(new char[]
		{
			':',
			','
		});
		if (array == null || array.Length != 4)
		{
			Debug.LogError("Illegal Parameter.");
			hasDayCompensation = false;
			return false;
		}
		int hour = int.Parse(array[0]);
		int minute = int.Parse(array[1]);
		int hour2 = int.Parse(array[2]);
		int minute2 = int.Parse(array[3]);
		DateTime fromTime = new DateTime(1, 1, 1, hour, minute, 0);
		DateTime toTime = new DateTime(1, 1, 1, hour2, minute2, 0);
		return facade.IsInTimeInterval(fromTime, toTime, out hasDayCompensation);
	}

	public static bool IsInXmasTime(this ToolsFacade facade, DateTime srcTime)
	{
		DateTime t = new DateTime(2016, 12, 19, 0, 0, 0);
		DateTime t2 = new DateTime(2017, 1, 4, 0, 0, 0);
		return srcTime >= t && srcTime < t2;
	}

	public static bool IsInNewYearTime(this ToolsFacade facade, DateTime srcTime)
	{
		DateTime t = new DateTime(2017, 1, 20, 0, 0, 0);
		DateTime t2 = new DateTime(2017, 2, 12, 0, 0, 0);
		return srcTime >= t && srcTime < t2;
	}

	public static bool IsInNewYearPackageTime(this ToolsFacade facade, DateTime srcTime)
	{
		DateTime t = new DateTime(2017, 1, 9, 0, 0, 0);
		DateTime t2 = new DateTime(2017, 2, 12, 0, 0, 0);
		return srcTime >= t && srcTime < t2;
	}
}
