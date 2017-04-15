using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDKUser
	{
		private static AnySDKUser _instance;

		public static AnySDKUser getInstance()
		{
			if (AnySDKUser._instance == null)
			{
				AnySDKUser._instance = new AnySDKUser();
			}
			return AnySDKUser._instance;
		}

		public void login()
		{
			AnySDKUser.AnySDKUser_nativeLogin();
		}

		[Obsolete("Please use Resources.login(Dictionary<string,string> info) instead")]
		public void login(string serverID, string authLoginServer = "")
		{
			AnySDKUser.AnySDKUser_nativeLoginWithParam(serverID, authLoginServer);
		}

		public void login(Dictionary<string, string> info)
		{
			string text = AnySDKUtil.dictionaryToString(info);
			Debug.Log("login   " + text);
			AnySDKUser.AnySDKUser_nativeLoginWithMap(text);
		}

		public string getUserID()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKUser.AnySDKUser_nativeGetUserID(stringBuilder);
			return stringBuilder.ToString();
		}

		public bool isLogined()
		{
			return AnySDKUser.AnySDKUser_nativeIsLogined();
		}

		public bool isFunctionSupported(string functionName)
		{
			return AnySDKUser.AnySDKUser_nativeIsFunctionSupported(functionName);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void setDebugMode(bool bDebug)
		{
			AnySDKUser.AnySDKUser_nativeSetDebugMode(bDebug);
		}

		public void setListener(MonoBehaviour gameObject, string functionName)
		{
			AnySDKUtil.registerActionCallback(AnySDKType.User, gameObject, functionName);
		}

		public string getPluginId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKUser.AnySDKUser_nativeGetPluginId(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getPluginVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKUser.AnySDKUser_nativeGetPluginVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getSDKVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKUser.AnySDKUser_nativeGetSDKVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void callFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			AnySDKUser.AnySDKUser_nativeCallFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public void callFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				AnySDKUser.AnySDKUser_nativeCallFuncWithParam(functionName, null, 0);
			}
			else
			{
				AnySDKUser.AnySDKUser_nativeCallFuncWithParam(functionName, param.ToArray(), param.Count);
			}
		}

		public int callIntFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKUser.AnySDKUser_nativeCallIntFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public int callIntFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKUser.AnySDKUser_nativeCallIntFuncWithParam(functionName, null, 0);
			}
			return AnySDKUser.AnySDKUser_nativeCallIntFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public float callFloatFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKUser.AnySDKUser_nativeCallFloatFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public float callFloatFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKUser.AnySDKUser_nativeCallFloatFuncWithParam(functionName, null, 0);
			}
			return AnySDKUser.AnySDKUser_nativeCallFloatFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public bool callBoolFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			return AnySDKUser.AnySDKUser_nativeCallBoolFuncWithParam(functionName, list.ToArray(), list.Count);
		}

		public bool callBoolFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			if (param == null)
			{
				return AnySDKUser.AnySDKUser_nativeCallBoolFuncWithParam(functionName, null, 0);
			}
			return AnySDKUser.AnySDKUser_nativeCallBoolFuncWithParam(functionName, param.ToArray(), param.Count);
		}

		public string callStringFuncWithParam(string functionName, AnySDKParam param)
		{
			List<AnySDKParam> list = new List<AnySDKParam>();
			list.Add(param);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDKUser.AnySDKUser_nativeCallStringFuncWithParam(functionName, list.ToArray(), list.Count, stringBuilder);
			return stringBuilder.ToString();
		}

		public string callStringFuncWithParam(string functionName, List<AnySDKParam> param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			if (param == null)
			{
				AnySDKUser.AnySDKUser_nativeCallStringFuncWithParam(functionName, null, 0, stringBuilder);
			}
			else
			{
				AnySDKUser.AnySDKUser_nativeCallStringFuncWithParam(functionName, param.ToArray(), param.Count, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		[DllImport("PluginProtocol", CallingConvention = CallingConvention.Cdecl)]
		private static extern void AnySDKUser_RegisterExternalCallDelegate(IntPtr functionPointer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeLogin();

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeSetListener(string gameName, string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeLoginWithParam(string serverID, string authLoginServer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeLoginWithMap(string info);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeGetUserID(StringBuilder userID);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKUser_nativeIsLogined();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKUser_nativeIsFunctionSupported(string functionName);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeSetDebugMode(bool bDebug);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeGetPluginId(StringBuilder pluginID);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeGetPluginVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeGetSDKVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeCallFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern int AnySDKUser_nativeCallIntFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern float AnySDKUser_nativeCallFloatFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDKUser_nativeCallBoolFuncWithParam(string functionName, AnySDKParam[] param, int count);

		[DllImport("PluginProtocol")]
		private static extern void AnySDKUser_nativeCallStringFuncWithParam(string functionName, AnySDKParam[] param, int count, StringBuilder value);
	}
}
