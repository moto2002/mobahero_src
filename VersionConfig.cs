using System;

[Serializable]
public class VersionConfig
{
	public string appVersion = "1.0.0";

	public string dataVersion;

	public bool isLoginByHoolaiSDK;

	public bool isLoginByAnySDK;

	public bool isLoginByLDSDK;

	public bool checkVersion;

	public bool needThirdLogin;

	public bool isIOSInAppPurchase;
}
