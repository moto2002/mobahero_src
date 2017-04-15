using System;
using UnityEngine;

internal class StrategyTimer
{
	private static StrategyTimer _instance;

	private bool hasStarted;

	private float duration;

	public static StrategyTimer Instance
	{
		get
		{
			if (StrategyTimer._instance == null)
			{
				StrategyTimer._instance = new StrategyTimer();
			}
			return StrategyTimer._instance;
		}
	}

	public int Seconds
	{
		get
		{
			return (int)this.duration;
		}
	}

	public void OnInit()
	{
		this.hasStarted = true;
		this.duration = 0f;
	}

	public void OnFinish()
	{
		this.hasStarted = false;
		this.duration = 0f;
	}

	public void Update()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.duration += Time.deltaTime * 0.5f;
	}
}
