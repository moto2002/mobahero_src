using System;
using UnityEngine;

public class TDGAVirtualCurrency
{
	private static string JAVA_CLASS = "com.tendcloud.tenddata.TDGAVirtualCurrency";

	private static AndroidJavaClass agent;

	private static AndroidJavaClass GetAgent()
	{
		if (TDGAVirtualCurrency.agent == null)
		{
			TDGAVirtualCurrency.agent = new AndroidJavaClass(TDGAVirtualCurrency.JAVA_CLASS);
		}
		return TDGAVirtualCurrency.agent;
	}

	public static void OnChargeRequest(string orderId, string iapId, double currencyAmount, string currencyType, double virtualCurrencyAmount, string paymentType)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			TDGAVirtualCurrency.GetAgent().CallStatic("onChargeRequest", new object[]
			{
				orderId,
				iapId,
				currencyAmount,
				currencyType,
				virtualCurrencyAmount,
				paymentType
			});
		}
	}

	public static void OnChargeSuccess(string orderId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			TDGAVirtualCurrency.GetAgent().CallStatic("onChargeSuccess", new object[]
			{
				orderId
			});
		}
	}

	public static void OnReward(double virtualCurrencyAmount, string reason)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			TDGAVirtualCurrency.GetAgent().CallStatic("onReward", new object[]
			{
				virtualCurrencyAmount,
				reason
			});
		}
	}
}
