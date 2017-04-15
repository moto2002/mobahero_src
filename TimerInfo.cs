using System;

public class TimerInfo
{
	public long tick;

	public TimerType type;

	public int interval;

	public int hour;

	public int minute;

	public int lastTime = -1;

	public object target;

	public bool isRemove;
}
