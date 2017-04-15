using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace anysdk
{
	public class AnySDKCrash
	{
		private static AnySDKCrash _instance;

		public static AnySDKCrash getInstance()
		{
			if (AnySDKCrash._instance == null)
			{
				AnySDKCrash._instance = new AnySDKCrash();
			}
			return AnySDKCrash._instance;
		}

		public void setUserIdentifier(string identifier)
		{
			AnySDKCrash.AnySDKCrash_nativeSetUserIdentifier(identifier);
		}

		public void reportException(string message, string exception)
		{
			AnySDKCrash.AnySDKCrash_nativeReportException(message, exception);
		}

		public void leaveBreadcrumb(string breadcrumb)
		{
			AnySDKCrash.AnySDKCrash_nativeLeaveBreadcrumb(breadcrumb);
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKCrash.AnySDKCrash_nativeIsFunctionSupported(functionName);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKCrash.AnySDKCrash_nativeSetDebugMode(bDebug);
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKCrash.AnySDKCrash_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKCrash.AnySDKCrash_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKCrash.AnySDKCrash_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKCrash.AnySDKCrash_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKCrash.AnySDKCrash_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKCrash.AnySDKCrash_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKCrash.AnySDKCrash_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKCrash.AnySDKCrash_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKCrash.AnySDKCrash_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKCrash.AnySDKCrash_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKCrash.AnySDKCrash_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKCrash.AnySDKCrash_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKCrash.AnySDKCrash_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKCrash.AnySDKCrash_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKCrash.AnySDKCrash_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKCrash.AnySDKCrash_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKCrash.AnySDKCrash_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCrash_nativeSetUserIdentifier(string identifier);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCrash_nativeReportException(string message, string exception);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCrash_nativeLeaveBreadcrumb(string breadcrumb);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKCrash_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCrash_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCrash_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCrash_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCrash_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKCrash_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKCrash_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKCrash_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKCrash_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
