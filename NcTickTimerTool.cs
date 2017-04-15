using System;
using UnityEngine;

public class NcTickTimerTool
{
	protected int m_nStartTickCount;

	protected int m_nCheckTickCount;

	public NcTickTimerTool()
	{
		this.StartTickCount();
	}

	public static NcTickTimerTool GetTickTimer()
	{
		return new NcTickTimerTool();
	}

	public void StartTickCount()
	{
		this.m_nStartTickCount = Environment.TickCount;
		this.m_nCheckTickCount = this.m_nStartTickCount;
	}

	public int GetStartedTickCount()
	{
		return Environment.TickCount - this.m_nStartTickCount;
	}

	public int GetElapsedTickCount()
	{
		int result = Environment.TickCount - this.m_nCheckTickCount;
		this.m_nCheckTickCount = Environment.TickCount;
		return result;
	}

	public void LogElapsedTickCount()
	{
		Debug.Log(this.GetElapsedTickCount());
	}
}
