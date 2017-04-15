using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace anysdk
{
	public class AnySDK
	{
		private static AnySDK _instance;

		public static AnySDK getInstance()
		{
			if (AnySDK._instance == null)
			{
				AnySDK._instance = new AnySDK();
			}
			return AnySDK._instance;
		}

		public void init(string appKey, string appSecret, string privateKey, string authLoginServer)
		{
			AnySDK.AnySDK_nativeInitPluginSystem(appKey, appSecret, privateKey, authLoginServer);
		}

		[Obsolete("This interface is obsolete!", false)]
		public void loadALLPlugin()
		{
			Debug.Log("This interface is obsolete!");
		}

		public string getCustomParam()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDK.AnySDK_nativeGetCustomParam(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getChannelId()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDK.AnySDK_nativeGetChannelId(stringBuilder);
			return stringBuilder.ToString();
		}

		public string getFrameworkVersion()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 1024;
			AnySDK.AnySDK_nativeGetFrameworkVersion(stringBuilder);
			return stringBuilder.ToString();
		}

		public void release()
		{
			AnySDK.AnySDK_nativeRelease();
		}

		public bool isUserPluginExist()
		{
			return AnySDK.AnySDK_nativeIsUserPluginExist();
		}

		public bool isIAPPluginExist()
		{
			return AnySDK.AnySDK_nativeIsIAPPluginExist();
		}

		public bool isAdsPluginExist()
		{
			return AnySDK.AnySDK_nativeIsAdsPluginExist();
		}

		public bool isAnalyticsPluginExist()
		{
			return AnySDK.AnySDK_nativeIsAnalyticsPluginExist();
		}

		public bool isPushPluginExist()
		{
			return AnySDK.AnySDK_nativeIsPushPluginExist();
		}

		public bool isSharePluginExist()
		{
			return AnySDK.AnySDK_nativeIsSharePluginExist();
		}

		public bool isSocialPluginExist()
		{
			return AnySDK.AnySDK_nativeIsSocialPluginExist();
		}

		public bool isCustomPluginExist()
		{
			return AnySDK.AnySDK_nativeIsCustomPluginExist();
		}

		public bool isRECPluginExist()
		{
			return AnySDK.AnySDK_nativeIsRECPluginExist();
		}

		public bool isCrashPluginExist()
		{
			return AnySDK.AnySDK_nativeIsCrashPluginExist();
		}

		public bool isAdTrackingPluginExist()
		{
			return AnySDK.AnySDK_nativeIsAdTrackingPluginExist();
		}

		public void setIsAnaylticsEnabled(bool enabled)
		{
			AnySDK.AnySDK_nativeSetIsAnaylticsEnabled(enabled);
		}

		public bool isAnaylticsEnabled()
		{
			return AnySDK.AnySDK_nativeIsAnaylticsEnabled();
		}

		[DllImport("PluginProtocol")]
		private static extern void AnySDK_nativeInitPluginSystem(string appKey, string appSecret, string privateKey, string authLoginServer);

		[DllImport("PluginProtocol")]
		private static extern void AnySDK_nativeLoadPlugins();

		[DllImport("PluginProtocol")]
		private static extern void AnySDK_nativeGetChannelId(StringBuilder channelId);

		[DllImport("PluginProtocol")]
		private static extern void AnySDK_nativeGetFrameworkVersion(StringBuilder version);

		[DllImport("PluginProtocol")]
		private static extern void AnySDK_nativeGetCustomParam(StringBuilder customParam);

		[DllImport("PluginProtocol")]
		private static extern void AnySDK_nativeRelease();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsUserPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsIAPPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsAdsPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsAnalyticsPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsSharePluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsSocialPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsPushPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsAdTrackingPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsCustomPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsRECPluginExist();

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsCrashPluginExist();

		[DllImport("PluginProtocol")]
		private static extern void AnySDK_nativeSetIsAnaylticsEnabled(bool enabled);

		[DllImport("PluginProtocol")]
		private static extern bool AnySDK_nativeIsAnaylticsEnabled();
	}
}
