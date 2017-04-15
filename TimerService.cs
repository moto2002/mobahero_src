using System;
using System.Collections.Generic;
using System.Timers;

public class TimerService
{
	private static TimerService _instance;

	private static readonly object _lockObj = new object();

	private static List<TimerInfo> timers = new List<TimerInfo>();

	private Timer timer;

	public TimerService()
	{
		this.timer = new Timer(1000.0);
		this.timer.Elapsed += new ElapsedEventHandler(this.OnTimeUpdate);
		this.timer.AutoReset = true;
	}

	public static TimerService getInstace()
	{
		if (TimerService._instance == null)
		{
			TimerService._instance = new TimerService();
		}
		return TimerService._instance;
	}

	public void Start()
	{
		this.timer.Enabled = true;
		this.timer.Start();
	}

	public void Stop()
	{
		this.timer.Stop();
	}

	public TimerInfo AddTimer(object target, int interval)
	{
		TimerInfo timerInfo = new TimerInfo();
		timerInfo.tick = 0L;
		timerInfo.interval = interval;
		timerInfo.type = TimerType.Interval;
		timerInfo.target = target;
		TimerService.timers.Add(timerInfo);
		return timerInfo;
	}

	public TimerInfo AddTimer(object target, int hour, int minute)
	{
		TimerInfo timerInfo = new TimerInfo();
		timerInfo.type = TimerType.DestTime;
		timerInfo.target = target;
		timerInfo.hour = hour;
		timerInfo.minute = minute;
		TimerService.timers.Add(timerInfo);
		return timerInfo;
	}

	public void RemoveTimer(TimerInfo info)
	{
		if (TimerService.timers.Contains(info) && info != null)
		{
			info.isRemove = true;
		}
	}

	private void OnTimeUpdate(object sender, ElapsedEventArgs e)
	{
		if (TimerService.timers.Count == 0)
		{
			return;
		}
		object lockObj = TimerService._lockObj;
		lock (lockObj)
		{
			for (int i = 0; i < TimerService.timers.Count; i++)
			{
				TimerInfo timerInfo = TimerService.timers[i];
				if (!timerInfo.isRemove)
				{
					TimerType type = timerInfo.type;
					if (type != TimerType.Interval)
					{
						if (type == TimerType.DestTime)
						{
							if (timerInfo.hour == DateTime.Now.Hour && timerInfo.minute == DateTime.Now.Minute && timerInfo.lastTime != DateTime.Now.Day)
							{
								timerInfo.lastTime = DateTime.Now.Day;
								TimerBehaviour timerBehaviour = timerInfo.target as TimerBehaviour;
								timerBehaviour.TimerUpdate();
							}
						}
					}
					else if (timerInfo.tick < (long)(timerInfo.interval - 1))
					{
						timerInfo.tick += 1L;
					}
					else
					{
						timerInfo.tick = 0L;
						TimerBehaviour timerBehaviour2 = timerInfo.target as TimerBehaviour;
						timerBehaviour2.TimerUpdate();
					}
				}
			}
			for (int j = TimerService.timers.Count - 1; j >= 0; j--)
			{
				if (TimerService.timers[j].isRemove)
				{
					TimerService.timers.Remove(TimerService.timers[j]);
				}
			}
		}
	}
}
