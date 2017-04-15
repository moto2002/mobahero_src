using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDKAds
	{
		private static AnySDKAds _instance;

		public static AnySDKAds getInstance()
		{
			if (AnySDKAds._instance == null)
			{
				AnySDKAds._instance = new AnySDKAds();
			}
			return AnySDKAds._instance;
		}

		public void showAds(AdsType adsType, int idx = 1)
		{
			AnySDKAds.AnySDKAds_nativeShowAds(adsType, idx);
		}

		public void hideAds(AdsType adsType, int idx = 1)
		{
			AnySDKAds.AnySDKAds_nativeHideAds(adsType, idx);
		}

		public void preloadAds(AdsType adsType, int idx = 1)
		{
			AnySDKAds.AnySDKAds_nativePreloadAds(adsType, idx);
		}

		public float queryPoints()
		{
			return AnySDKAds.AnySDKAds_nativeQueryPoints();
		}

		public void spendPoints(int points)
		{
			AnySDKAds.AnySDKAds_nativeSpendPoints(points);
		}

		public bool isAdTypeSupported(AdsType adType)
		{
			return AnySDKAds.AnySDKAds_nativeIsAdTypeSupported(adType);
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKAds.AnySDKAds_nativeIsFunctionSupported(functionName);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKAds.AnySDKAds_nativeSetDebugMode(bDebug);
		}

		public void setListener(MonoBehaviour gameObject, string functionName)
		{
			AnySDKUtil.registerActionCallback(AnySDKType.Ads, gameObject, functionName);
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAds.AnySDKAds_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAds.AnySDKAds_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKAds.AnySDKAds_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKAds.AnySDKAds_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKAds.AnySDKAds_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAds.AnySDKAds_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAds.AnySDKAds_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKAds.AnySDKAds_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAds.AnySDKAds_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAds.AnySDKAds_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKAds.AnySDKAds_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAds.AnySDKAds_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAds.AnySDKAds_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKAds.AnySDKAds_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAds.AnySDKAds_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKAds.AnySDKAds_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKAds.AnySDKAds_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol", CallingConvention = CallingConvention.Cdecl)]
		private static extern void AnySDKAds_RegisterExternalCallDelegate(IntPtr functionPointer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeSetListener(string gameName, string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeShowAds(AdsType adsType, int idx = 1);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeHideAds(AdsType adsType, int idx = 1);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativePreloadAds(AdsType adsType, int idx = 1);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKAds_nativeQueryPoints();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeSpendPoints(int points);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKAds_nativeIsAdTypeSupported(AdsType adsType);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKAds_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKAds_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKAds_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKAds_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAds_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
