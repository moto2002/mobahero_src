using System;
using UnityEngine;

public class TDGAItem
{
	private static string JAVA_CLASS = "com.tendcloud.tenddata.TDGAItem";

	private static AndroidJavaClass agent;

	private static AndroidJavaClass GetAgent()
	{
		if (TDGAItem.agent == null)
		{
			TDGAItem.agent = new AndroidJavaClass(TDGAItem.JAVA_CLASS);
		}
		return TDGAItem.agent;
	}

	public static void OnPurchase(string item, int itemNumber, double priceInVirtualCurrency)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			TDGAItem.GetAgent().CallStatic("onPurchase", new object[]
			{
				item,
				itemNumber,
				priceInVirtualCurrency
			});
		}
	}

	public static void OnUse(string item, int itemNumber)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			TDGAItem.GetAgent().CallStatic("onUse", new object[]
			{
				item,
				itemNumber
			});
		}
	}
}
