using System;
using UnityEngine;

public class HoolaiSDK : MonoBehaviour
{
	public const string LOG_TAG = "fastaccess_Unity ";

	public const string ACTION_ENTER_SERVER = "1";

	public const string ACTION_LEVEL_UP = "2";

	public const string ACTION_CREATE_ROLE = "3";

	public const string ACTION = "ACTION";

	public const string ROLE_ID = "ROLE_ID";

	public const string ROLE_NAME = "ROLE_NAME";

	public const string ROLE_LEVEL = "ROLE_LEVEL";

	public const string ZONE_ID = "ZONE_ID";

	public const string ZONE_NAME = "ZONE_NAME";

	public const string BALANCE = "BALANCE";

	public const string VIP = "VIP";

	public const string PARTYNAME = "PARTY_NAME";

	public const string APP_VERSION = "app_version";

	public const string APP_RES_VERSION = "app_res_version";

	public const int Type_Init_Fail = 1;

	public const int Type_Init_Success = 2;

	public const int Type_Login_Fail = 3;

	public const int Type_Login_Success = 4;

	public const int Type_Logout = 5;

	public const int Type_Pay_Fail = 6;

	public const int Type_Pay_Success = 7;

	public const int Type_Exit_Channel = 8;

	public const int Type_Exit_Game = 9;

	public const int Type_GetServers_Success = 10;

	public const int Type_GetServers_Fail = 11;

	public const int Type_SelectServer_Success = 12;

	public const int Type_SelectServer_Fail = 13;

	public const int Type_SendBI_Result = 14;

	public const int Type_Maintenace = 15;

	public static bool isLogin;

	private static HoolaiSDK _instance;

	public static HoolaiSDK instance
	{
		get
		{
			if (HoolaiSDK._instance == null)
			{
				HoolaiSDK._instance = new HoolaiSDK();
			}
			return HoolaiSDK._instance;
		}
	}

	public AndroidJavaObject androidContext()
	{
		return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
	}

	public void init(string gameObject, string initCallbackMethod, string loginCallbackMethod, string payCallbackMethod)
	{
		this.init(gameObject, initCallbackMethod, loginCallbackMethod, payCallbackMethod, string.Empty);
	}

	public void init(string gameObject, string initCallbackMethod, string loginCallbackMethod, string payCallbackMethod, string onMaintenanceCallbackMethod)
	{
		this.androidContext().Call("doInit", new object[]
		{
			gameObject,
			initCallbackMethod,
			loginCallbackMethod,
			payCallbackMethod
		});
	}

	public void login(string customParams)
	{
		this.androidContext().Call("doLogin", new object[]
		{
			customParams
		});
	}

	public void logout(string customParams)
	{
		if (HoolaiSDK.isLogin)
		{
			this.androidContext().Call("doLogout", new object[]
			{
				customParams
			});
		}
		else
		{
			this.showTip("Please login first!");
		}
	}

	public void pay(int amount, string itemName, string callbackInfo, string customParams)
	{
		if (HoolaiSDK.isLogin)
		{
			this.androidContext().Call("doPay", new object[]
			{
				amount,
				itemName,
				callbackInfo,
				customParams
			});
		}
		else
		{
			this.showTip("Please login first!");
		}
	}

	public void setExtData(string userExtData)
	{
		this.androidContext().Call("setExtData", new object[]
		{
			userExtData
		});
	}

	public void exit(string gameObject, string callbackMethod)
	{
		this.androidContext().Call("doExit", new object[]
		{
			gameObject,
			callbackMethod
		});
	}

	public void releaseResource()
	{
		this.androidContext().Call("releaseResource", new object[0]);
	}

	public void getServers(string gameObject, string callbackMethod, string version)
	{
		this.androidContext().Call("doGetServers", new object[]
		{
			gameObject,
			callbackMethod,
			version
		});
	}

	public void selectServer(string gameObject, string callbackMethod, string serverId)
	{
		this.androidContext().Call("doSelectServer", new object[]
		{
			gameObject,
			callbackMethod,
			serverId
		});
	}

	public void sendBIData(string gameObject, string callbackMethod, string metric, string jsonString)
	{
		this.androidContext().Call("doSendBIData", new object[]
		{
			gameObject,
			callbackMethod,
			metric,
			jsonString
		});
	}

	public void showTip(string info)
	{
		this.androidContext().Call("showToast", new object[]
		{
			info
		});
	}

	public string getManifestData(string name)
	{
		string text = this.androidContext().Call<string>("getMainifestMetaData", new object[]
		{
			name
		});
		Debug.Log("fastaccess_Unity The key is:" + name + " The value is:" + text);
		return text;
	}
}
