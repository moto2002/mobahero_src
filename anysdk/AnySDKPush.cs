using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDKPush
	{
		private static AnySDKPush _instance;

		public static AnySDKPush getInstance()
		{
			if (AnySDKPush._instance == null)
			{
				AnySDKPush._instance = new AnySDKPush();
			}
			return AnySDKPush._instance;
		}

		public void startPush()
		{
			AnySDKPush.AnySDKPush_nativeStartPush();
		}

		public void closePush()
		{
			AnySDKPush.AnySDKPush_nativeClosePush();
		}

		public void setAlias(string alia)
		{
			AnySDKPush.AnySDKPush_nativeSetAlias(alia);
		}

		public void delAlias(string alia)
		{
			AnySDKPush.AnySDKPush_nativeDelAlias(alia);
		}

		public void setTags(List<string> tags)
		{
			string tags2 = AnySDKUtil.ListToString(tags);
			AnySDKPush.AnySDKPush_nativeSetTags(tags2);
		}

		public void delTags(List<string> tags)
		{
			string tags2 = AnySDKUtil.ListToString(tags);
			AnySDKPush.AnySDKPush_nativeDelTags(tags2);
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKPush.AnySDKPush_nativeIsFunctionSupported(functionName);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKPush.AnySDKPush_nativeSetDebugMode(bDebug);
		}

		public void setListener(MonoBehaviour gameObject, string functionName)
		{
			AnySDKUtil.registerActionCallback(AnySDKType.Push, gameObject, functionName);
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKPush.AnySDKPush_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKPush.AnySDKPush_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKPush.AnySDKPush_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKPush.AnySDKPush_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKPush.AnySDKPush_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKPush.AnySDKPush_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKPush.AnySDKPush_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKPush.AnySDKPush_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKPush.AnySDKPush_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKPush.AnySDKPush_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKPush.AnySDKPush_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKPush.AnySDKPush_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKPush.AnySDKPush_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKPush.AnySDKPush_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKPush.AnySDKPush_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKPush.AnySDKPush_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKPush.AnySDKPush_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol", CallingConvention = CallingConvention.Cdecl)]
		private static extern void AnySDKPush_RegisterExternalCallDelegate(IntPtr functionPointer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeSetListener(string gameName, string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeStartPush();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeClosePush();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeSetAlias(string alia);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeDelAlias(string alia);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeSetTags(string tags);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeDelTags(string tags);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKPush_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKPush_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKPush_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKPush_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKPush_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
