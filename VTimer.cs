using Com.Game.Module;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class VTimer : Singleton<VTimer>
{
	public bool isStopTimer;

	public bool isStartTimer;

	public float start_time;

	public float time_interval = 1f;

	private CoroutineManager m_CoroutineMamager = new CoroutineManager();

	private float m_curTime;

	public float CurTime
	{
		get
		{
			return this.m_curTime;
		}
	}

	public void StartTimer(float cur_time)
	{
		this.start_time = cur_time;
		if (!this.isStartTimer)
		{
			this.isStartTimer = true;
			this.m_CoroutineMamager.StartCoroutine(this.StartElapsedTimer(), true);
		}
	}

	public void StartTimer()
	{
		this.StartTimer(Time.time);
	}

	public void SetStartTime(float cur_time)
	{
		this.start_time = cur_time;
	}

	[DebuggerHidden]
	private IEnumerator StartElapsedTimer()
	{
		VTimer.<StartElapsedTimer>c__Iterator4 <StartElapsedTimer>c__Iterator = new VTimer.<StartElapsedTimer>c__Iterator4();
		<StartElapsedTimer>c__Iterator.<>f__this = this;
		return <StartElapsedTimer>c__Iterator;
	}

	public void StopTimer()
	{
		this.m_CoroutineMamager.StopAllCoroutine();
		this.isStopTimer = false;
		this.isStartTimer = false;
		this.m_curTime = 0f;
	}
}
