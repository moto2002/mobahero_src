using System;
using UnityEngine;

public static class vp_TimeUtility
{
	public struct Units
	{
		public int hours;

		public int minutes;

		public int seconds;

		public int deciSeconds;

		public int centiSeconds;

		public int milliSeconds;
	}

	public static vp_TimeUtility.Units TimeToUnits(float timeInSeconds)
	{
		vp_TimeUtility.Units result = default(vp_TimeUtility.Units);
		result.hours = (int)timeInSeconds / 3600;
		result.minutes = ((int)timeInSeconds - result.hours * 3600) / 60;
		result.seconds = (int)timeInSeconds % 60;
		result.deciSeconds = (int)((timeInSeconds - (float)result.seconds) * 10f) % 60;
		result.centiSeconds = (int)((timeInSeconds - (float)result.seconds) * 100f % 600f);
		result.milliSeconds = (int)((timeInSeconds - (float)result.seconds) * 1000f % 6000f);
		return result;
	}

	public static float UnitsToSeconds(vp_TimeUtility.Units units)
	{
		float num = 0f;
		num += (float)(units.hours * 3600);
		num += (float)(units.minutes * 60);
		num += (float)units.seconds;
		num += (float)units.deciSeconds * 0.1f;
		num += (float)(units.centiSeconds / 100);
		return num + (float)(units.milliSeconds / 1000);
	}

	public static string TimeToString(float timeInSeconds, bool showHours, bool showMinutes, bool showSeconds, bool showTenths, bool showHundredths, bool showMilliSeconds, char delimiter = ':')
	{
		vp_TimeUtility.Units units = vp_TimeUtility.TimeToUnits(timeInSeconds);
		string text = (units.hours >= 10) ? units.hours.ToString() : ("0" + units.hours.ToString());
		string arg = (units.minutes >= 10) ? units.minutes.ToString() : ("0" + units.minutes.ToString());
		string arg2 = (units.seconds >= 10) ? units.seconds.ToString() : ("0" + units.seconds.ToString());
		string arg3 = units.deciSeconds.ToString();
		string arg4 = (units.centiSeconds >= 10) ? units.centiSeconds.ToString() : ("0" + units.centiSeconds.ToString());
		string text2 = (units.milliSeconds >= 100) ? units.milliSeconds.ToString() : ("0" + units.milliSeconds.ToString());
		text2 = ((units.milliSeconds >= 10) ? text2 : ("0" + text2));
		return string.Concat(new string[]
		{
			(!showHours) ? string.Empty : text,
			(!showMinutes) ? string.Empty : (delimiter + arg),
			(!showSeconds) ? string.Empty : (delimiter + arg2),
			(!showTenths) ? string.Empty : (delimiter + arg3),
			(!showHundredths) ? string.Empty : (delimiter + arg4),
			(!showMilliSeconds) ? string.Empty : (delimiter + text2)
		}).TrimStart(new char[]
		{
			delimiter
		});
	}

	public static string SystemTimeToString(DateTime systemTime, bool showHours, bool showMinutes, bool showSeconds, bool showTenths, bool showHundredths, bool showMilliSeconds, char delimiter = ':')
	{
		return vp_TimeUtility.TimeToString(vp_TimeUtility.SystemTimeToSeconds(systemTime), showHours, showMinutes, showSeconds, showTenths, showHundredths, showMilliSeconds, delimiter);
	}

	public static string SystemTimeToString(bool showHours, bool showMinutes, bool showSeconds, bool showTenths, bool showHundredths, bool showMilliSeconds, char delimiter = ':')
	{
		return vp_TimeUtility.SystemTimeToString(DateTime.Now, showHours, showMinutes, showSeconds, showTenths, showHundredths, showMilliSeconds, delimiter);
	}

	public static vp_TimeUtility.Units SystemTimeToUnits(DateTime systemTime)
	{
		return new vp_TimeUtility.Units
		{
			hours = systemTime.Hour,
			minutes = systemTime.Minute,
			seconds = systemTime.Second,
			deciSeconds = (int)((float)systemTime.Millisecond / 100f),
			centiSeconds = systemTime.Millisecond / 10,
			milliSeconds = systemTime.Millisecond
		};
	}

	public static vp_TimeUtility.Units SystemTimeToUnits()
	{
		return vp_TimeUtility.SystemTimeToUnits(DateTime.Now);
	}

	public static float SystemTimeToSeconds(DateTime systemTime)
	{
		return vp_TimeUtility.UnitsToSeconds(vp_TimeUtility.SystemTimeToUnits(systemTime));
	}

	public static float SystemTimeToSeconds()
	{
		return vp_TimeUtility.SystemTimeToSeconds(DateTime.Now);
	}

	public static float TimeToDegrees(float seconds, bool includeHours = false, bool includeMinutes = false, bool includeSeconds = true, bool includeMilliSeconds = true)
	{
		vp_TimeUtility.Units units = vp_TimeUtility.TimeToUnits(seconds);
		if (includeHours && includeMinutes && includeSeconds)
		{
			return vp_TimeUtility.HoursToDegreesInternal((float)units.hours, (float)units.minutes, (float)units.seconds);
		}
		if (includeHours && includeMinutes)
		{
			return vp_TimeUtility.HoursToDegreesInternal((float)units.hours, (float)units.minutes, 0f);
		}
		if (includeMinutes && includeSeconds && includeMilliSeconds)
		{
			return vp_TimeUtility.MinutesToDegreesInternal((float)units.minutes, (float)units.seconds, (float)units.milliSeconds);
		}
		if (includeMinutes && includeSeconds)
		{
			return vp_TimeUtility.MinutesToDegreesInternal((float)units.minutes, (float)units.seconds, 0f);
		}
		if (includeSeconds && includeMilliSeconds)
		{
			return vp_TimeUtility.SecondsToDegreesInternal((float)units.seconds, (float)units.milliSeconds);
		}
		if (includeHours)
		{
			return vp_TimeUtility.HoursToDegreesInternal((float)units.hours, 0f, 0f);
		}
		if (includeMinutes)
		{
			return vp_TimeUtility.MinutesToDegreesInternal((float)units.minutes, 0f, 0f);
		}
		if (includeSeconds)
		{
			return vp_TimeUtility.TimeToDegrees((float)units.seconds, false, false, true, true);
		}
		if (includeMilliSeconds)
		{
			return vp_TimeUtility.MilliSecondsToDegreesInternal((float)units.milliSeconds);
		}
		Debug.LogError("Error: (vp_TimeUtility.TimeToDegrees) This combination of time units is not supported.");
		return 0f;
	}

	public static Vector3 SystemTimeToDegrees(DateTime time, bool smooth = true)
	{
		return new Vector3(vp_TimeUtility.HoursToDegreesInternal((float)time.Hour, (!smooth) ? 0f : ((float)time.Minute), (!smooth) ? 0f : ((float)time.Second)), vp_TimeUtility.MinutesToDegreesInternal((float)time.Minute, (!smooth) ? 0f : ((float)time.Second), (!smooth) ? 0f : ((float)time.Millisecond)), vp_TimeUtility.SecondsToDegreesInternal((float)time.Second, (!smooth) ? 0f : ((float)time.Millisecond)));
	}

	public static Vector3 SystemTimeToDegrees(bool smooth = true)
	{
		return vp_TimeUtility.SystemTimeToDegrees(DateTime.Now, smooth);
	}

	private static float HoursToDegreesInternal(float hours, float minutes = 0f, float seconds = 0f)
	{
		return hours * 30f + minutes * 0.5f + seconds * 0.008333333f;
	}

	private static float MinutesToDegreesInternal(float minutes, float seconds = 0f, float milliSeconds = 0f)
	{
		return minutes * 6f + seconds * 0.1f + milliSeconds * 0.0001f;
	}

	private static float SecondsToDegreesInternal(float seconds, float milliSeconds = 0f)
	{
		return seconds * 6f + milliSeconds * 0.006f;
	}

	private static float MilliSecondsToDegreesInternal(float milliSeconds)
	{
		return milliSeconds * 0.36f;
	}
}
