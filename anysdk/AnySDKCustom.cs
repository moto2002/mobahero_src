using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDKCustom
	{
		private static AnySDKCustom _instance;

		public static AnySDKCustom getInstance()
		{
			if (AnySDKCustom._instance == null)
			{
				AnySDKCustom._instance = new AnySDKCustom();
			}
			return AnySDKCustom._instance;
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKCustom.AnySDKCustom_nativeIsFunctionSupported(functionName);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKCustom.AnySDKCustom_nativeSetDebugMode(bDebug);
		}

		public void setListener(MonoBehaviour gameObject, string functionName)
		{
			AnySDKUtil.registerActionCallback(AnySDKType.Custom, gameObject, functionName);
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKCustom.AnySDKCustom_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKCustom.AnySDKCustom_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKCustom.AnySDKCustom_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKCustom.AnySDKCustom_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKCustom.AnySDKCustom_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKCustom.AnySDKCustom_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKCustom.AnySDKCustom_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKCustom.AnySDKCustom_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKCustom.AnySDKCustom_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKCustom.AnySDKCustom_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKCustom.AnySDKCustom_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKCustom.AnySDKCustom_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKCustom.AnySDKCustom_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKCustom.AnySDKCustom_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKCustom.AnySDKCustom_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKCustom.AnySDKCustom_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKCustom.AnySDKCustom_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol", CallingConvention = CallingConvention.Cdecl)]
		private static extern void AnySDKCustom_RegisterExternalCallDelegate(IntPtr functionPointer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCustom_nativeSetListener(string gameName, string functionName);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKCustom_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCustom_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCustom_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCustom_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCustom_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKCustom_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKCustom_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKCustom_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCustom_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
