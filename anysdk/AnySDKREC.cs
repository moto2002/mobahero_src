using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDKREC
	{
		private static AnySDKREC _instance;

		public static AnySDKREC getInstance()
		{
			if (AnySDKREC._instance == null)
			{
				AnySDKREC._instance = new AnySDKREC();
			}
			return AnySDKREC._instance;
		}

		public void startRecording()
		{
			AnySDKREC.AnySDKREC_nativeStartRecording();
		}

		public void stopRecording()
		{
			AnySDKREC.AnySDKREC_nativeStopRecording();
		}

		public void share(Dictionary<string, string> shareInfo)
		{
			string text = AnySDKUtil.dictionaryToString(shareInfo);
			Debug.Log("share   " + text);
			AnySDKREC.AnySDKREC_nativeShare(text);
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKREC.AnySDKREC_nativeIsFunctionSupported(functionName);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKREC.AnySDKREC_nativeSetDebugMode(bDebug);
		}

		public void setListener(MonoBehaviour gameObject, string functionName)
		{
			AnySDKUtil.registerActionCallback(AnySDKType.REC, gameObject, functionName);
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKREC.AnySDKREC_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKREC.AnySDKREC_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKREC.AnySDKREC_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKREC.AnySDKREC_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKREC.AnySDKREC_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKREC.AnySDKREC_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKREC.AnySDKREC_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKREC.AnySDKREC_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKREC.AnySDKREC_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKREC.AnySDKREC_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKREC.AnySDKREC_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKREC.AnySDKREC_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKREC.AnySDKREC_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKREC.AnySDKREC_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKREC.AnySDKREC_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKREC.AnySDKREC_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKREC.AnySDKREC_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol", CallingConvention = CallingConvention.Cdecl)]
		private static extern void AnySDKREC_RegisterExternalCallDelegate(IntPtr functionPointer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeStartRecording();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeStopRecording();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeShare(string info);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeSetListener(string gameName, string functionName);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKREC_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKREC_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKREC_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKREC_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKREC_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
