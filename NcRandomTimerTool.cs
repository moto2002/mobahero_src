using System;
using UnityEngine;

public class NcRandomTimerTool : NcTimerTool
{
	protected float m_fRandomTime;

	protected float m_fUpdateTime;

	protected float m_fMinIntervalTime;

	protected int m_nRepeatCount;

	protected int m_nCallCount;

	protected object m_ArgObject;

	public bool UpdateRandomTimer(bool bReset)
	{
		if (this.UpdateRandomTimer())
		{
			this.ResetUpdateTime();
			return true;
		}
		return false;
	}

	public bool UpdateRandomTimer()
	{
		if (!this.m_bEnable)
		{
			return false;
		}
		bool flag = this.m_fUpdateTime <= base.GetTime();
		if (flag)
		{
			this.m_fUpdateTime += this.m_fMinIntervalTime + ((0f >= this.m_fRandomTime) ? 0f : (UnityEngine.Random.value % this.m_fRandomTime));
			this.m_nCallCount++;
			if (this.m_nRepeatCount != 0 && this.m_nRepeatCount <= this.m_nCallCount)
			{
				this.m_bEnable = false;
			}
		}
		return flag;
	}

	public void ResetUpdateTime()
	{
		this.m_fUpdateTime = base.GetTime() + this.m_fMinIntervalTime + ((0f >= this.m_fRandomTime) ? 0f : (UnityEngine.Random.value % this.m_fRandomTime));
	}

	public int GetCallCount()
	{
		return this.m_nCallCount;
	}

	public object GetArgObject()
	{
		return this.m_ArgObject;
	}

	public float GetElapsedRate()
	{
		if (this.m_fUpdateTime == 0f)
		{
			return 1f;
		}
		return base.GetTime() / this.m_fUpdateTime;
	}

	public void SetTimer(float fStartTime, float fRandomTime)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRandomTime);
	}

	public void SetTimer(float fStartTime, float fRandomTime, float fMinIntervalTime)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRandomTime, fMinIntervalTime);
	}

	public void SetTimer(float fStartTime, float fRandomTime, float fMinIntervalTime, int nRepeatCount)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRandomTime, fMinIntervalTime, nRepeatCount);
	}

	public void SetTimer(float fStartTime, float fRandomTime, object arg)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRandomTime, arg);
	}

	public void SetTimer(float fStartTime, float fRandomTime, float fMinIntervalTime, object arg)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRandomTime, fMinIntervalTime, arg);
	}

	public void SetTimer(float fStartTime, float fRandomTime, float fMinIntervalTime, int nRepeatCount, object arg)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRandomTime, fMinIntervalTime, nRepeatCount, arg);
	}

	public void SetRelTimer(float fStartRelTime, float fRandomTime)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fRandomTime = fRandomTime;
		this.m_fUpdateTime = ((0f >= this.m_fRandomTime) ? 0f : (UnityEngine.Random.value % this.m_fRandomTime));
		this.m_fMinIntervalTime = 0f;
		this.m_nRepeatCount = 0;
	}

	public void SetRelTimer(float fStartRelTime, float fRandomTime, float fMinIntervalTime)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fRandomTime = fRandomTime;
		this.m_fUpdateTime = ((0f >= this.m_fRandomTime) ? 0f : (UnityEngine.Random.value % this.m_fRandomTime));
		this.m_fMinIntervalTime = fMinIntervalTime;
		this.m_nRepeatCount = 0;
	}

	public void SetRelTimer(float fStartRelTime, float fRandomTime, float fMinIntervalTime, int nRepeatCount)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fRandomTime = fRandomTime;
		this.m_fUpdateTime = ((0f >= this.m_fRandomTime) ? 0f : (UnityEngine.Random.value % this.m_fRandomTime));
		this.m_fMinIntervalTime = fMinIntervalTime;
		this.m_nRepeatCount = nRepeatCount;
	}

	public void SetRelTimer(float fStartRelTime, float fRandomTime, object arg)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fRandomTime = fRandomTime;
		this.m_fUpdateTime = ((0f >= this.m_fRandomTime) ? 0f : (UnityEngine.Random.value % this.m_fRandomTime));
		this.m_fMinIntervalTime = 0f;
		this.m_nRepeatCount = 0;
		this.m_ArgObject = arg;
	}

	public void SetRelTimer(float fStartRelTime, float fRandomTime, float fMinIntervalTime, object arg)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fRandomTime = fRandomTime;
		this.m_fUpdateTime = ((0f >= this.m_fRandomTime) ? 0f : (UnityEngine.Random.value % this.m_fRandomTime));
		this.m_fMinIntervalTime = fMinIntervalTime;
		this.m_nRepeatCount = 0;
		this.m_ArgObject = arg;
	}

	public void SetRelTimer(float fStartRelTime, float fRandomTime, float fMinIntervalTime, int nRepeatCount, object arg)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fRandomTime = fRandomTime;
		this.m_fUpdateTime = ((0f >= this.m_fRandomTime) ? 0f : (UnityEngine.Random.value % this.m_fRandomTime));
		this.m_fMinIntervalTime = fMinIntervalTime;
		this.m_nRepeatCount = nRepeatCount;
		this.m_ArgObject = arg;
	}
}
