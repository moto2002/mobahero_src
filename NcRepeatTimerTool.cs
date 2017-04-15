using System;

public class NcRepeatTimerTool : NcTimerTool
{
	protected float m_fUpdateTime;

	protected float m_fIntervalTime;

	protected int m_nRepeatCount;

	protected int m_nCallCount;

	protected object m_ArgObject;

	public bool UpdateTimer()
	{
		if (!this.m_bEnable)
		{
			return false;
		}
		bool flag = this.m_fUpdateTime <= base.GetTime();
		if (flag)
		{
			this.m_fUpdateTime += this.m_fIntervalTime;
			this.m_nCallCount++;
			if (this.m_fIntervalTime <= 0f || (this.m_nRepeatCount != 0 && this.m_nRepeatCount <= this.m_nCallCount))
			{
				this.m_bEnable = false;
			}
		}
		return flag;
	}

	public void ResetUpdateTime()
	{
		this.m_fUpdateTime = base.GetTime() + this.m_fIntervalTime;
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

	public void SetTimer(float fStartTime)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime());
	}

	public void SetTimer(float fStartTime, float fRepeatTime)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, int nRepeatCount)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, nRepeatCount);
	}

	public void SetTimer(float fStartTime, object arg)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), arg);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, object arg)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, arg);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, int nRepeatCount, object arg)
	{
		this.SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, nRepeatCount, arg);
	}

	public void SetRelTimer(float fStartRelTime)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fUpdateTime = fStartRelTime;
		this.m_fIntervalTime = 0f;
		this.m_nRepeatCount = 0;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fUpdateTime = fStartRelTime;
		this.m_fIntervalTime = fRepeatTime;
		this.m_nRepeatCount = 0;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, int nRepeatCount)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fUpdateTime = fStartRelTime;
		this.m_fIntervalTime = fRepeatTime;
		this.m_nRepeatCount = nRepeatCount;
	}

	public void SetRelTimer(float fStartRelTime, object arg)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fUpdateTime = fStartRelTime;
		this.m_fIntervalTime = 0f;
		this.m_nRepeatCount = 0;
		this.m_ArgObject = arg;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, object arg)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fUpdateTime = fStartRelTime;
		this.m_fIntervalTime = fRepeatTime;
		this.m_nRepeatCount = 0;
		this.m_ArgObject = arg;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, int nRepeatCount, object arg)
	{
		base.Start();
		this.m_nCallCount = 0;
		this.m_fUpdateTime = fStartRelTime;
		this.m_fIntervalTime = fRepeatTime;
		this.m_nRepeatCount = nRepeatCount;
		this.m_ArgObject = arg;
	}
}
