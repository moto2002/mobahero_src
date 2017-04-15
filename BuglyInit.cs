using System;
using UnityEngine;

public class BuglyInit : MonoBehaviour
{
	private const string BuglyAppIDForiOS = "900014645";

	private const string BuglyAppIDForAndroid = "900016162";

	public bool androidEnable;

	public bool iosEnable = true;

	private BuglyTest buglyTest;

	private void Awake()
	{
		if (Application.platform == RuntimePlatform.Android && !this.androidEnable)
		{
			return;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer && !this.iosEnable)
		{
			return;
		}
		BuglyAgent.ConfigCrashReporter(1, 1);
		BuglyAgent.ConfigDebugMode(true);
		BuglyAgent.ConfigDefault(string.Empty, GlobalSettings.Instance.versionConfig.appVersion, "10000", 0L);
		BuglyAgent.ConfigAutoReportLogLevel(LogSeverity.LogError);
		BuglyAgent.ConfigAutoQuitApplication(true);
		BuglyAgent.RegisterLogCallback(new BuglyAgent.LogCallbackDelegate(CallbackDelegate.Instance.OnApplicationLogCallbackHandler));
		BuglyAgent.InitWithAppId("900016162");
		BuglyAgent.SetScene(3450);
		BuglyAgent.EnableExceptionHandler();
	}

	private void Update()
	{
	}
}
