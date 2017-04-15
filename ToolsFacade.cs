using Assets.Scripts.Server;
using System;

public class ToolsFacade : IGlobalComServer
{
	private static ToolsFacade mInstance;

	public static ToolsFacade Instance
	{
		get
		{
			return ToolsFacade.mInstance;
		}
	}

	public static DateTime ServerCurrentTime
	{
		get
		{
			return Tools_TimeCheck.ServerCurrentTime;
		}
	}

	public void OnAwake()
	{
		ToolsFacade.mInstance = this;
	}

	public void OnStart()
	{
	}

	public void OnUpdate()
	{
	}

	public void OnDestroy()
	{
	}

	public void Enable(bool b)
	{
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
