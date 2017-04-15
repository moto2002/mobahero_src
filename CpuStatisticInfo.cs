using System;
using UnityEngine;

[Serializable]
public class CpuStatisticInfo
{
	public string statisticName;

	public int cpuTimePerFrame;

	public int totalCpuTime;

	public int frameCount;

	[HideInInspector]
	public long _totalCpuTime;

	[HideInInspector]
	public long _sampleBeginTime;

	[HideInInspector]
	public long _sampleEndTime;
}
