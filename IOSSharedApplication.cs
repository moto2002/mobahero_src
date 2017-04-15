using System;
using UnityEngine;

public static class IOSSharedApplication
{
	public static void CheckUrl(string url)
	{
	}

	public static void OpenUrl(string url)
	{
	}

	public static long PluginGetFreeMemory()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		return @static.Call<long>("GetFreeSpase", new object[0]);
	}
}
