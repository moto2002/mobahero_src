using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDKAdTracking
	{
		private static AnySDKAdTracking _instance;

		public static AnySDKAdTracking getInstance()
		{
			if (AnySDKAdTracking._instance == null)
			{
				AnySDKAdTracking._instance = new AnySDKAdTracking();
			}
			return AnySDKAdTracking._instance;
		}

		public void onRegister(string userId)
		{
			AnySDKAdTracking.AnySDKAdTracking_nativeOnRegister(userId);
		}

		public void onLogin(Dictionary<string, string> userInfo)
		{
			string text = AnySDKUtil.dictionaryToString(userInfo);
			Debug.Log("onLogin   " + text);
			AnySDKAdTracking.AnySDKAdTracking_nativeOnLogin(text);
		}

		public void onPay(Dictionary<string, string> userInfo)
		{
			string text = AnySDKUtil.dictionaryToString(userInfo);
			Debug.Log("onPay   " + text);
			AnySDKAdTracking.AnySDKAdTracking_nativeOnPay(text);
		}

		public void trackEvent(string eventId, Dictionary<string, string> paramMap = null)
		{
			string message;
			if (paramMap == null)
			{
				message = null;
			}
			else
			{
				message = AnySDKUtil.dictionaryToString(paramMap);
			}
			AnySDKAdTracking.AnySDKAdTracking_nativeTrackEvent(eventId, message);
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKAdTracking.AnySDKAdTracking_nativeIsFunctionSupported(functionName);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKAdTracking.AnySDKAdTracking_nativeSetDebugMode(bDebug);
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAdTracking.AnySDKAdTracking_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAdTracking.AnySDKAdTracking_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKAdTracking.AnySDKAdTracking_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKAdTracking.AnySDKAdTracking_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKAdTracking.AnySDKAdTracking_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAdTracking.AnySDKAdTracking_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAdTracking.AnySDKAdTracking_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKAdTracking.AnySDKAdTracking_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAdTracking.AnySDKAdTracking_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAdTracking.AnySDKAdTracking_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKAdTracking.AnySDKAdTracking_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAdTracking.AnySDKAdTracking_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAdTracking.AnySDKAdTracking_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKAdTracking.AnySDKAdTracking_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAdTracking.AnySDKAdTracking_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKAdTracking.AnySDKAdTracking_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKAdTracking.AnySDKAdTracking_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeOnRegister(string userId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeOnLogin(string info);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeOnPay(string info);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeTrackEvent(string eventId, string message);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKAdTracking_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKAdTracking_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKAdTracking_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKAdTracking_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAdTracking_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
