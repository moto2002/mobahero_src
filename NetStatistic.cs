using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetStatistic : Singleton<NetStatistic>
{
	private bool logging;

	private Dictionary<NetEventType, List<float>> recvList = new Dictionary<NetEventType, List<float>>();

	public void StartLog()
	{
		this.logging = true;
	}

	public void Log(NetEventType type)
	{
		if (!this.logging)
		{
			return;
		}
		if (!this.recvList.ContainsKey(type))
		{
			this.recvList[type] = new List<float>();
		}
		this.recvList[type].Add(Time.realtimeSinceStartup);
	}

	public void StopLog()
	{
		this.logging = false;
	}

	private void DumpLog()
	{
	}
}
