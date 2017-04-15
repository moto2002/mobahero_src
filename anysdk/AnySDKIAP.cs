using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDKIAP
	{
		private static AnySDKIAP _instance;

		public static AnySDKIAP getInstance()
		{
			if (AnySDKIAP._instance == null)
			{
				AnySDKIAP._instance = new AnySDKIAP();
			}
			return AnySDKIAP._instance;
		}

		public void payForProduct(Dictionary<string, string> info, string pluginId = "")
		{
			string info2 = AnySDKUtil.dictionaryToString(info);
			AnySDKIAP.AnySDKIAP_nativePayForProduct(info2, pluginId);
		}

		public string getOrderId(string pluginId = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKIAP.AnySDKIAP_nativeGetOrderId(stringBuilder, pluginId);
			return stringBuilder.ToString();
		}

		public bool isFunctionSupported(string functionName, string pluginId = "")
		{
			return AnySDKIAP.AnySDKIAP_nativeIsFunctionSupported(functionName, pluginId);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKIAP.AnySDKIAP_nativeSetDebugMode(bDebug);
		}

		public void setListener(MonoBehaviour gameObject, string functionName)
		{
			AnySDKUtil.registerActionCallback(AnySDKType.IAP, gameObject, functionName);
		}

		public List<string> getPluginId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKIAP.AnySDKIAP_nativeGetPluginId(stringBuilder);
			return AnySDKUtil.StringToList(stringBuilder.ToString());
		}

		public void resetPayState()
		{
			AnySDKIAP.AnySDKIAP_nativeResetPayState();
		}

		public string getPluginVersion(string pluginId = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKIAP.AnySDKIAP_nativeGetPluginVersion(stringBuilder, pluginId);
			return stringBuilder.ToString();
		}

		public string getSDKVersion(string pluginId = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKIAP.AnySDKIAP_nativeGetSDKVersion(stringBuilder, pluginId);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param, string pluginId = "")
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKIAP.AnySDKIAP_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count, pluginId);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null, string pluginId = "")
		{
			if (param == null)
			{
				AnySDKIAP.AnySDKIAP_nativeCallFuncWithParam(functionName, null, 0, pluginId);
			}
			else
			{
				AnySDKIAP.AnySDKIAP_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count, pluginId);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param, string pluginId = "")
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKIAP.AnySDKIAP_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count, pluginId);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null, string pluginId = "")
		{
			if (param == null)
			{
				return AnySDKIAP.AnySDKIAP_nativeCallIntFuncWithParam(functionName, null, 0, pluginId);
			}
			return AnySDKIAP.AnySDKIAP_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count, pluginId);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param, string pluginId = "")
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKIAP.AnySDKIAP_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count, pluginId);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null, string pluginId = "")
		{
			if (param == null)
			{
				return AnySDKIAP.AnySDKIAP_nativeCallFloatFuncWithParam(functionName, null, 0, pluginId);
			}
			return AnySDKIAP.AnySDKIAP_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count, pluginId);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param, string pluginId = "")
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKIAP.AnySDKIAP_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count, pluginId);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null, string pluginId = "")
		{
			if (param == null)
			{
				return AnySDKIAP.AnySDKIAP_nativeCallBoolFuncWithParam(functionName, null, 0, pluginId);
			}
			return AnySDKIAP.AnySDKIAP_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count, pluginId);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param, string pluginId = "")
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKIAP.AnySDKIAP_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder, pluginId);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null, string pluginId = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKIAP.AnySDKIAP_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder, pluginId);
			}
			else
			{
				AnySDKIAP.AnySDKIAP_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder, pluginId);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol", CallingConvention = CallingConvention.Cdecl)]
		private static extern void AnySDKIAP_RegisterExternalCallDelegate(IntPtr functionPointer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeSetListener(string gameName, string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativePayForProduct(string info, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeGetOrderId(StringBuilder orderId, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeResetPayState();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKIAP_nativeIsFunctionSupported(string functionName, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeGetPluginId(StringBuilder pluginID);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeGetPluginVersion(StringBuilder version, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeGetSDKVersion(StringBuilder version, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKIAP_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKIAP_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKIAP_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count, string pluginId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKIAP_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value, string pluginId);
	}
}
