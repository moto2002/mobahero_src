using Assets.Scripts.Server;
using System;
using UnityEngine;

public class AnalyticsToolManager : IGlobalComServer
{
	public static void StartAnalytics()
	{
		if (GlobalSettings.isLoginByAnySDK)
		{
			Debug.Log("TalkingDataGA OnStart");
			TalkingDataGA.OnStart("343299A1A2045DE1EB91E4891CB33B10", string.Empty);
		}
		if (GlobalSettings.isLoginByHoolaiSDK)
		{
			TalkingDataGA.OnStart("B7210B11A60E42B6BA76C76206038888", string.Empty);
		}
		else
		{
			TalkingDataGA.OnStart("343299A1A2045DE1EB91E4891CB33B10", string.Empty);
		}
		if (GlobalSettings.isLoginByHoolaiSDK)
		{
			ReYun.instance.Init();
		}
	}

	public static void EndAnalytics()
	{
		TalkingDataGA.OnEnd();
	}

	public static void Register(string accountId)
	{
		if (GlobalSettings.isLoginByHoolaiSDK)
		{
			ReYun.instance.Register(accountId);
		}
	}

	public static void Quest(string taskId, int level)
	{
		if (GlobalSettings.isLoginByHoolaiSDK)
		{
			ReYun.instance.Quest(taskId, level);
		}
	}

	public static void SetAccountId(string accountId)
	{
		TDGAAccount.SetAccount(accountId);
	}

	public static void SetChargeRequest(string orderId, int price, string paymentType)
	{
	}

	public static void SetChargeSuccess(string orderId)
	{
	}

	public static void SetLevel(string accountId, int level)
	{
		TDGAAccount tDGAAccount = TDGAAccount.SetAccount(accountId);
		if (tDGAAccount != null)
		{
			tDGAAccount.SetLevel(level);
		}
		if (GlobalSettings.isLoginByHoolaiSDK)
		{
			ReYun.instance.Login(accountId, level);
		}
	}

	public static void StartLevel(string level)
	{
		TDGAMission.OnBegin(level);
	}

	public static void FinishLevel(string level)
	{
		TDGAMission.OnCompleted(level);
	}

	public static void FailLevel(string level)
	{
		TDGAMission.OnFailed(level, string.Empty);
	}

	public static string GetDeviceInfo()
	{
		return null;
	}

	public static string GetVersionInfo()
	{
		return M_SystemConfig.app_version;
	}

	public void OnAwake()
	{
	}

	public void OnStart()
	{
		AnalyticsToolManager.StartAnalytics();
	}

	public void OnUpdate()
	{
	}

	public void OnDestroy()
	{
		AnalyticsToolManager.EndAnalytics();
	}

	public void Enable(bool b)
	{
	}

	public void OnRestart()
	{
	}

	public void OnApplicationQuit()
	{
		AnalyticsToolManager.EndAnalytics();
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnApplicationPause(bool isPause)
	{
	}
}
