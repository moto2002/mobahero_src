using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace anysdk
{
	public class AnySDKAnalytics
	{
		private static AnySDKAnalytics _instance;

		public static AnySDKAnalytics getInstance()
		{
			if (AnySDKAnalytics._instance == null)
			{
				AnySDKAnalytics._instance = new AnySDKAnalytics();
			}
			return AnySDKAnalytics._instance;
		}

		public void startSession()
		{
			AnySDKAnalytics.AnySDKAnalytics_nativeStartSession();
		}

		public void stopSession()
		{
			AnySDKAnalytics.AnySDKAnalytics_nativeStopSession();
		}

		public void setSessionContinueMillis(long millis)
		{
			AnySDKAnalytics.AnySDKAnalytics_nativeSetSessionContinueMillis(millis);
		}

		public void logError(string errorId, string message)
		{
			AnySDKAnalytics.AnySDKAnalytics_nativeLogError(errorId, message);
		}

		public void logEvent(string eventId, Dictionary<string, string> paramMap = null)
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
			AnySDKAnalytics.AnySDKAnalytics_nativeLogEvent(eventId, message);
		}

		public void logTimedEventBegin(string errorId)
		{
			AnySDKAnalytics.AnySDKAnalytics_nativeLogTimedEventBegin(errorId);
		}

		public void logTimedEventEnd(string eventId)
		{
			AnySDKAnalytics.AnySDKAnalytics_nativeLogTimedEventEnd(eventId);
		}

		public void setCaptureUncaughtException(bool enabled)
		{
			AnySDKAnalytics.AnySDKAnalytics_nativeSetCaptureUncaughtException(enabled);
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKAnalytics.AnySDKAnalytics_nativeIsFunctionSupported(functionName);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKAnalytics.AnySDKAnalytics_nativeSetDebugMode(bDebug);
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAnalytics.AnySDKAnalytics_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAnalytics.AnySDKAnalytics_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKAnalytics.AnySDKAnalytics_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKAnalytics.AnySDKAnalytics_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKAnalytics.AnySDKAnalytics_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAnalytics.AnySDKAnalytics_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAnalytics.AnySDKAnalytics_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKAnalytics.AnySDKAnalytics_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAnalytics.AnySDKAnalytics_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAnalytics.AnySDKAnalytics_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKAnalytics.AnySDKAnalytics_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKAnalytics.AnySDKAnalytics_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKAnalytics.AnySDKAnalytics_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKAnalytics.AnySDKAnalytics_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKAnalytics.AnySDKAnalytics_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKAnalytics.AnySDKAnalytics_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKAnalytics.AnySDKAnalytics_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeStartSession();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeStopSession();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeSetSessionContinueMillis(long milli);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeLogError(string errorId, string message);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeLogEvent(string eventId, string message);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeLogTimedEventBegin(string eventId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeLogTimedEventEnd(string eventId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeSetCaptureUncaughtException(bool enabled);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKAnalytics_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKAnalytics_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKAnalytics_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKAnalytics_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKAnalytics_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
