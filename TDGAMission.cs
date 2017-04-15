using System;
using UnityEngine;

public class TDGAMission
{
	private static string JAVA_CLASS = "com.tendcloud.tenddata.TDGAMission";

	private static AndroidJavaClass agent;

	private static AndroidJavaClass GetAgent()
	{
		if (TDGAMission.agent == null)
		{
			TDGAMission.agent = new AndroidJavaClass(TDGAMission.JAVA_CLASS);
		}
		return TDGAMission.agent;
	}

	public static void OnBegin(string missionId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			TDGAMission.GetAgent().CallStatic("onBegin", new object[]
			{
				missionId
			});
		}
	}

	public static void OnCompleted(string missionId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			TDGAMission.GetAgent().CallStatic("onCompleted", new object[]
			{
				missionId
			});
		}
	}

	public static void OnFailed(string missionId, string failedCause)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			TDGAMission.GetAgent().CallStatic("onFailed", new object[]
			{
				missionId,
				failedCause
			});
		}
	}
}
