using Assets.Scripts.Server;
using System;
using UnityEngine;

public class TestGCCollect : IGlobalComServer
{
	private bool enable;

	public void OnAwake()
	{
	}

	public void OnStart()
	{
	}

	public void OnUpdate()
	{
		if (this.enable && Time.frameCount % 5 == 0)
		{
			GC.Collect();
		}
	}

	public void OnDestroy()
	{
	}

	public void Enable(bool b)
	{
		this.enable = b;
	}

	public void OnRestart()
	{
	}

	public void OnApplicationQuit()
	{
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnApplicationPause(bool isPause)
	{
	}
}
