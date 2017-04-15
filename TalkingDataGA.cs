using System;
using System.Collections.Generic;
using UnityEngine;

public class TalkingDataGA
{
	private static AndroidJavaClass agent;

	private static AndroidJavaClass unityClass;

	private static string JAVA_CLASS = "com.tendcloud.tenddata.TalkingDataGA";

	private static string UNTIFY_CLASS = "com.unity3d.player.UnityPlayer";

	private static string deviceId;

	public static void AttachCurrentThread()
	{
		AndroidJNI.AttachCurrentThread();
	}

	public static void DetachCurrentThread()
	{
		AndroidJNI.DetachCurrentThread();
	}

	public static string GetDeviceId()
	{
		if (TalkingDataGA.deviceId == null && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			if (TalkingDataGA.agent == null)
			{
				TalkingDataGA.agent = new AndroidJavaClass(TalkingDataGA.JAVA_CLASS);
			}
			AndroidJavaObject @static = TalkingDataGA.unityClass.GetStatic<AndroidJavaObject>("currentActivity");
			TalkingDataGA.deviceId = TalkingDataGA.agent.CallStatic<string>("getDeviceId", new object[]
			{
				@static
			});
		}
		return TalkingDataGA.deviceId;
	}

	public static void OnStart(string appID, string channelId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			if (TalkingDataGA.agent == null)
			{
				TalkingDataGA.agent = new AndroidJavaClass(TalkingDataGA.JAVA_CLASS);
			}
			TalkingDataGA.agent.SetStatic<int>("sPlatformType", 2);
			TalkingDataGA.unityClass = new AndroidJavaClass(TalkingDataGA.UNTIFY_CLASS);
			AndroidJavaObject @static = TalkingDataGA.unityClass.GetStatic<AndroidJavaObject>("currentActivity");
			TalkingDataGA.agent.CallStatic("init", new object[]
			{
				@static,
				appID,
				channelId
			});
			TalkingDataGA.agent.CallStatic("onResume", new object[]
			{
				@static
			});
		}
	}

	public static void OnEnd()
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor && TalkingDataGA.agent != null)
		{
			AndroidJavaObject @static = TalkingDataGA.unityClass.GetStatic<AndroidJavaObject>("currentActivity");
			TalkingDataGA.agent.CallStatic("onPause", new object[]
			{
				@static
			});
			TalkingDataGA.agent = null;
			TalkingDataGA.unityClass = null;
		}
	}

	public static void OnKill()
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor && TalkingDataGA.agent != null)
		{
			AndroidJavaObject @static = TalkingDataGA.unityClass.GetStatic<AndroidJavaObject>("currentActivity");
			TalkingDataGA.agent.CallStatic("onKill", new object[]
			{
				@static
			});
			TalkingDataGA.agent = null;
			TalkingDataGA.unityClass = null;
		}
	}

	public static void OnEvent(string actionId, Dictionary<string, object> parameters)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			if (parameters != null && parameters.Count > 0)
			{
				int count = parameters.Count;
				AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap", new object[]
				{
					count
				});
				IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
				object[] array = new object[2];
				foreach (KeyValuePair<string, object> current in parameters)
				{
					array[0] = new AndroidJavaObject("java.lang.String", new object[]
					{
						current.Key
					});
					if (typeof(string).IsInstanceOfType(current.Value))
					{
						array[1] = new AndroidJavaObject("java.lang.String", new object[]
						{
							current.Value
						});
					}
					else
					{
						array[1] = new AndroidJavaObject("java.lang.Double", new object[]
						{
							string.Empty + current.Value
						});
					}
					AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
				}
				if (TalkingDataGA.agent != null)
				{
					TalkingDataGA.agent.CallStatic("onEvent", new object[]
					{
						actionId,
						androidJavaObject
					});
				}
			}
			else if (TalkingDataGA.agent != null)
			{
				AndroidJavaObject arg_177_0 = TalkingDataGA.agent;
				string arg_177_1 = "onEvent";
				object[] expr_173 = new object[2];
				expr_173[0] = actionId;
				arg_177_0.CallStatic(arg_177_1, expr_173);
			}
		}
	}

	public static void SetVerboseLogDisabled()
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			if (TalkingDataGA.agent == null)
			{
				TalkingDataGA.agent = new AndroidJavaClass(TalkingDataGA.JAVA_CLASS);
			}
			TalkingDataGA.agent.CallStatic("setVerboseLogDisabled", new object[0]);
		}
	}
}
