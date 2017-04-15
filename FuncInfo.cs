using System;

public class FuncInfo
{
	public string name;

	public int callCount;

	public long total;

	public long maxTime;

	public int recCnt;

	public long lastRecord;

	public FuncInfo(string funcName)
	{
		this.name = funcName;
	}

	public void recordTime(long timeEnd)
	{
		if (--this.recCnt == 0)
		{
			timeEnd -= this.lastRecord;
			this.total += timeEnd;
			if (timeEnd > this.maxTime)
			{
				this.maxTime = timeEnd;
			}
		}
		this.callCount++;
	}
}
