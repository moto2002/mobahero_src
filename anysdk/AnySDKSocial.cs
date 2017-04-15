using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDKSocial
	{
		private static AnySDKSocial _instance;

		public static AnySDKSocial getInstance()
		{
			if (AnySDKSocial._instance == null)
			{
				AnySDKSocial._instance = new AnySDKSocial();
			}
			return AnySDKSocial._instance;
		}

		public void signIn()
		{
			AnySDKSocial.AnySDKSocial_nativeSignIn();
		}

		public void signOut()
		{
			AnySDKSocial.AnySDKSocial_nativeSignOut();
		}

		public void submitScore(string leadboardID, long score)
		{
			AnySDKSocial.AnySDKSocial_nativeSubmitScore(leadboardID, score);
		}

		public void showLeaderboard(string leadboardID)
		{
			AnySDKSocial.AnySDKSocial_nativeShowLeaderboard(leadboardID);
		}

		public void unlockAchievement(Dictionary<string, string> achInfo)
		{
			string info = AnySDKUtil.dictionaryToString(achInfo);
			AnySDKSocial.AnySDKSocial_nativeUnlockAchievement(info);
		}

		public void showAchievements()
		{
			AnySDKSocial.AnySDKSocial_nativeShowAchievements();
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKSocial.AnySDKSocial_nativeSetDebugMode(bDebug);
		}

		public void setListener(MonoBehaviour gameObject, string functionName)
		{
			AnySDKUtil.registerActionCallback(AnySDKType.Social, gameObject, functionName);
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKSocial.AnySDKSocial_nativeIsFunctionSupported(functionName);
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKSocial.AnySDKSocial_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKSocial.AnySDKSocial_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKSocial.AnySDKSocial_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKSocial.AnySDKSocial_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKSocial.AnySDKSocial_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKSocial.AnySDKSocial_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKSocial.AnySDKSocial_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKSocial.AnySDKSocial_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKSocial.AnySDKSocial_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKSocial.AnySDKSocial_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKSocial.AnySDKSocial_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKSocial.AnySDKSocial_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKSocial.AnySDKSocial_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKSocial.AnySDKSocial_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKSocial.AnySDKSocial_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKSocial.AnySDKSocial_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKSocial.AnySDKSocial_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol", CallingConvention = CallingConvention.Cdecl)]
		private static extern void AnySDKSocial_RegisterExternalCallDelegate(IntPtr functionPointer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeSetListener(string gameName, string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeSignIn();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeSignOut();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeShowLeaderboard(string leadboardID);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKSocial_nativeSubmitScore(string leadboardID, long score);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKSocial_nativeUnlockAchievement(string info);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKSocial_nativeShowAchievements();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKSocial_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKSocial_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKSocial_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKSocial_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKSocial_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
