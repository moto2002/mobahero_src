using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class DeviceID
{
	[DllImport("__Internal")]
	private static extern string GetIphoneADID();

	public static string Get()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.yang.GetAID.AndroidID");
		string str = androidJavaClass2.CallStatic<string>("GetID", new object[]
		{
			@static
		});
		return "ANDROID-" + str;
	}
}
