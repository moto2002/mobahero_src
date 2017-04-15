using Assets.Scripts.Model;
using cn.sharesdk.unity3d;
using Com.Game.Module;
using GameLogin;
using MobaClientCom;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class InitSDK : MonoBehaviour
{
	public bool isFirstLogin = true;

	public bool isExit;

	public bool isInit;

	public bool logoutNeedRest = true;

	public bool needLogin;

	private CoroutineManager coroutineManager = new CoroutineManager();

	private static InitSDK _instance;

	private AndroidJavaObject jo;

	private string lblMessage;

	private AndroidJavaClass jc;

	public static IAPManager IAPMgr;

	private bool isIOSPaying;

	private bool isLoginState;

	private ShareSDK ssdk;

	private PlatformType platformTypeLogin = PlatformType.WeChat;

	public bool isPayTestOne;

	public static InitSDK instance
	{
		get
		{
			if (InitSDK._instance == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "HoolSDK";
				InitSDK._instance = gameObject.AddComponent<InitSDK>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return InitSDK._instance;
		}
	}

	[DllImport("__Internal")]
	private static extern void IPayPay(string item, int price, string useid, string callback);

	public bool IsInit()
	{
		return this.isInit;
	}

	private void initBFLogoCallback(string result)
	{
		UnityEngine.Debug.Log("fastaccess_Unity initBFLogoCallback result:" + result);
	}

	private void initCallback(string result)
	{
		UnityEngine.Debug.Log("fastaccess_Unity initCallback result:" + result);
		JsonData jsonData = JsonMapper.ToObject(result);
		int num = (int)jsonData["resultCode"];
		if (num == 2)
		{
			this.isInit = true;
			if (this.needLogin)
			{
				NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
				HoolaiSDK.instance.login("login");
			}
			else
			{
				LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_Init);
			}
			this.needLogin = false;
			UnityEngine.Debug.Log("fastaccess_Unity init success");
		}
		else
		{
			this.isInit = false;
			UnityEngine.Debug.Log("fastaccess_Unity init fail");
			CtrlManager.ShowMsgBox("初始化失败", "是否进行重试？", new Action<bool>(this.AgainCall), PopViewType.PopOneButton, "确定", "取消", null);
		}
	}

	private void AgainCall(bool obj)
	{
		HoolaiSDK.instance.init(base.gameObject.name, "initCallback", "loginCallback", "payCallback");
	}

	private void OnApplicationPause(bool isPause)
	{
		if (!isPause && this.isIOSPaying)
		{
			if (this.coroutineManager == null)
			{
				this.coroutineManager = new CoroutineManager();
			}
			this.coroutineManager.StartCoroutine(this.CheckToUpdatePay(), true);
		}
	}

	[DebuggerHidden]
	private IEnumerator CheckToUpdatePay()
	{
		InitSDK.<CheckToUpdatePay>c__Iterator19C <CheckToUpdatePay>c__Iterator19C = new InitSDK.<CheckToUpdatePay>c__Iterator19C();
		<CheckToUpdatePay>c__Iterator19C.<>f__this = this;
		return <CheckToUpdatePay>c__Iterator19C;
	}

	private void loginCallback(string result)
	{
		UnityEngine.Debug.Log("fastaccess_Unity loginCallback result:" + result);
		GlobalObject.Instance.SetCanPause(true);
		JsonData jsonData = JsonMapper.ToObject(result);
		int num = (int)jsonData["resultCode"];
		if (num == 4)
		{
			JsonData jsonData2 = jsonData["data"];
			UnityEngine.Debug.Log("fastaccess_Unity onLoginSuccess:" + JsonMapper.ToJson(jsonData2));
			HoolaiSDK.isLogin = true;
			string text = (string)jsonData2["nickName"];
			int num2 = (int)jsonData2["uid"];
			string text2 = (string)jsonData2["accessToken"];
			string text3 = (string)jsonData2["channel"];
			int num3 = (int)jsonData2["productId"];
			string text4 = (string)jsonData2["channelUid"];
			Singleton<TipView>.Instance.ShowViewSetText("渠道账号登陆成功..", 1f);
			this.coroutineManager.StartCoroutine(this.TryLoginByPlatformUid(num2, text3, text4, text2, num3, false, false), true);
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"MobaMasterCode.LoginByPlatformUid ",
				num2,
				" ",
				text3,
				" ",
				text4,
				text2,
				" ",
				num3
			}));
		}
		else if (num == 3)
		{
			JsonData jsonData3 = jsonData["data"];
			string str = (string)jsonData3["detail"];
			string str2 = (string)jsonData3["customParams"];
			Singleton<TipView>.Instance.ShowViewSetText("渠道账号登陆失败..", 1f);
			Singleton<LoginView_New>.Instance.SDKLoginFail();
			if (Singleton<NewWaitingView>.Instance.IsOpen)
			{
				Singleton<NewWaitingView>.Instance.Destroy();
			}
			UnityEngine.Debug.Log("fastaccess_Unity  detail:" + str + " customParams:" + str2);
		}
		else
		{
			HoolaiSDK.isLogin = false;
			string text5 = (string)jsonData["customParams"];
			Singleton<TipView>.Instance.ShowViewSetText("账号登出..", 1f);
			if (this.logoutNeedRest)
			{
				GlobalObject.ReStartGame();
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.AreaViewNew_goback, null, false);
			}
			this.logoutNeedRest = true;
			UnityEngine.Debug.Log("fastaccess_Unity 登出成功");
		}
	}

	public void SetExtData(string key)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		UserData userData = ModelManager.Instance.Get_userData_X();
		dictionary.Add("ROLE_ID", userData.UserId);
		int loginCount = ModelManager.Instance.Get_userData_X().LoginCount;
		if (loginCount < 1)
		{
			dictionary.Add("ROLE_NAME", userData.UserId);
		}
		else
		{
			dictionary.Add("ROLE_NAME", userData.NickName);
		}
		dictionary.Add("ROLE_LEVEL", CharacterDataMgr.instance.GetUserLevel(userData.Exp).ToString());
		dictionary.Add("ZONE_ID", (ModelManager.Instance.Get_curLoginServerInfo().areaId + 1).ToString());
		dictionary.Add("ZONE_NAME", ModelManager.Instance.Get_curLoginServerInfo().servername.ToString());
		dictionary.Add("BALANCE", "0");
		dictionary.Add("VIP", userData.VIP.ToString());
		dictionary.Add("PARTY_NAME", "无工会");
		dictionary.Add("app_version", "app_version");
		dictionary.Add("app_res_version", "app_res_version");
		dictionary.Add("ACTION", key);
		HoolaiSDK.instance.setExtData(JsonMapper.ToJson(dictionary));
	}

	public void SetAnySDKExtData(string key)
	{
		InitAnySDK.getInstance().submitLoginGameRole(key);
	}

	[DebuggerHidden]
	private IEnumerator TryLoginByPlatformUid(int uid, string channelname, string channeluid, string token, int productid, bool p1, bool p2)
	{
		InitSDK.<TryLoginByPlatformUid>c__Iterator19D <TryLoginByPlatformUid>c__Iterator19D = new InitSDK.<TryLoginByPlatformUid>c__Iterator19D();
		<TryLoginByPlatformUid>c__Iterator19D.uid = uid;
		<TryLoginByPlatformUid>c__Iterator19D.channelname = channelname;
		<TryLoginByPlatformUid>c__Iterator19D.channeluid = channeluid;
		<TryLoginByPlatformUid>c__Iterator19D.token = token;
		<TryLoginByPlatformUid>c__Iterator19D.productid = productid;
		<TryLoginByPlatformUid>c__Iterator19D.<$>uid = uid;
		<TryLoginByPlatformUid>c__Iterator19D.<$>channelname = channelname;
		<TryLoginByPlatformUid>c__Iterator19D.<$>channeluid = channeluid;
		<TryLoginByPlatformUid>c__Iterator19D.<$>token = token;
		<TryLoginByPlatformUid>c__Iterator19D.<$>productid = productid;
		return <TryLoginByPlatformUid>c__Iterator19D;
	}

	public void LoginByQQ()
	{
		Singleton<TipView>.Instance.ShowViewSetText("安卓版QQ登陆暂未开放", 2f);
	}

	public void LoginByWeChat()
	{
		if (this.ssdk == null)
		{
			this.ssdk = GlobalObject.Instance.transform.Find("Tools").GetComponent<ShareSDK>();
		}
		if (Application.isEditor)
		{
			return;
		}
		NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
		this.isLoginState = true;
		this.ssdk.authHandler = new ShareSDK.EventHandler(this.OnAuthResultHandler);
		this.platformTypeLogin = PlatformType.WeChat;
		if (!this.ssdk.IsClientValid(this.platformTypeLogin))
		{
			Singleton<TipView>.Instance.ShowViewSetText("请先安装微信", 2f);
		}
		else
		{
			this.ssdk.Authorize(PlatformType.WeChat);
		}
	}

	private void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			UnityEngine.Debug.Log("authorize success !Platform :" + type);
			UnityEngine.Debug.Log("authorize success !result :");
			UnityEngine.Debug.Log(MiniJSON.jsonEncode(result));
			string text = string.Empty;
			if (result != null && result.ContainsKey("openid"))
			{
				text = result["openid"].ToString();
			}
			if (text != string.Empty)
			{
				this.coroutineManager.StartCoroutine(this.TryLoginByChannelId(type.ToString(), text), true);
			}
		}
		else if (state == ResponseState.Fail)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"fail! throwable stack = ",
				result["stack"],
				"; error msg = ",
				result["msg"]
			}));
		}
		else if (state == ResponseState.Cancel)
		{
			UnityEngine.Debug.Log("cancel !");
		}
	}

	private void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			UnityEngine.Debug.Log("get user info result :");
			UnityEngine.Debug.Log(MiniJSON.jsonEncode(result));
			UnityEngine.Debug.Log("Get userInfo success !Platform :" + type);
			string text = string.Empty;
			if (result.ContainsKey("openid"))
			{
				text = result["openid"].ToString();
			}
			if (text != string.Empty)
			{
				this.coroutineManager.StartCoroutine(this.TryLoginByChannelId(type.ToString(), text), true);
			}
		}
		else if (state == ResponseState.Fail)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"fail! throwable stack = ",
				result["stack"],
				"; error msg = ",
				result["msg"]
			}));
		}
		else if (state == ResponseState.Cancel)
		{
			UnityEngine.Debug.Log("cancel !");
		}
		this.isLoginState = false;
	}

	[DebuggerHidden]
	public IEnumerator TryLoginByChannelId(string channelId, string userName)
	{
		InitSDK.<TryLoginByChannelId>c__Iterator19E <TryLoginByChannelId>c__Iterator19E = new InitSDK.<TryLoginByChannelId>c__Iterator19E();
		<TryLoginByChannelId>c__Iterator19E.channelId = channelId;
		<TryLoginByChannelId>c__Iterator19E.userName = userName;
		<TryLoginByChannelId>c__Iterator19E.<$>channelId = channelId;
		<TryLoginByChannelId>c__Iterator19E.<$>userName = userName;
		return <TryLoginByChannelId>c__Iterator19E;
	}

	public void payCallback(string result)
	{
		UnityEngine.Debug.Log("fastaccess_Unity payCallback result:" + result);
		this.isIOSPaying = false;
		JsonData jsonData = JsonMapper.ToObject(result);
		int num = (int)jsonData["resultCode"];
		if (num == 7)
		{
			Singleton<TipView>.Instance.ShowViewSetText("成功开始请求支付", 2f);
			this.coroutineManager.StartCoroutine(this.TryUpdatePay(), true);
			UnityEngine.Debug.Log("fastaccess_Unity pay success");
			AnalyticsToolManager.SetChargeSuccess(string.Concat(new string[]
			{
				ModelManager.Instance.Get_userData_X().UserId.ToString(),
				"_",
				ModelManager.Instance.Get_curLoginServerInfo().serverip,
				"_",
				ModelManager.Instance.Get_curLoginServerInfo().tcpaddress
			}));
		}
		else
		{
			UnityEngine.Debug.Log("fastaccess_Unity pay fail");
		}
	}

	public void orderBack(string result)
	{
		UnityEngine.Debug.Log("orderBack result=" + result);
		this.isIOSPaying = false;
		if ("0" == result)
		{
			Singleton<TipView>.Instance.ShowViewSetText("支付成功，等待刷新", 2f);
			if (!GlobalSettings.isLoginByAnySDK)
			{
				AnalyticsToolManager.SetChargeSuccess(string.Concat(new string[]
				{
					ModelManager.Instance.Get_userData_X().UserId.ToString(),
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().serverip,
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().tcpaddress
				}));
			}
		}
		else
		{
			Singleton<TipView>.Instance.ShowViewSetText("支付失败", 2f);
		}
		if (this.coroutineManager == null)
		{
			this.coroutineManager = new CoroutineManager();
		}
		this.coroutineManager.StartCoroutine(this.TryUpdatePay(), true);
	}

	[DebuggerHidden]
	private IEnumerator TryUpdatePay()
	{
		InitSDK.<TryUpdatePay>c__Iterator19F <TryUpdatePay>c__Iterator19F = new InitSDK.<TryUpdatePay>c__Iterator19F();
		<TryUpdatePay>c__Iterator19F.<>f__this = this;
		return <TryUpdatePay>c__Iterator19F;
	}

	private void OnGetMsg_GetActivityTask(MobaMessage msg)
	{
		Singleton<MenuView>.Instance.UpdateFirstPay();
		MVC_MessageManager.RemoveListener_view(MobaGameCode.GetActivityTask, new MobaMessageFunc(this.OnGetMsg_GetActivityTask));
	}

	public void exitCallback(string result)
	{
		UnityEngine.Debug.Log("fastaccess_Unity exitCallback result:" + result);
		JsonData jsonData = JsonMapper.ToObject(result);
		GlobalObject.Instance.SetCanPause(true);
		int num = (int)jsonData["resultCode"];
		if (num == 8)
		{
			UnityEngine.Debug.Log("fastaccess_Unity onChannelExit");
			HoolaiSDK.instance.releaseResource();
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				GlobalObject.ReStartGame();
			}
			else
			{
				GlobalObject.QuitApp();
			}
		}
		else
		{
			UnityEngine.Debug.Log("fastaccess_Unity onGameExit");
			CtrlManager.ShowMsgBox("确认退出", "确定退出刺激好玩的《魔霸英雄》吗？", new Action<bool>(this.ExitCall), PopViewType.PopTwoButton, "确定", "取消", null);
		}
	}

	public void ExitCall(bool isConfirm)
	{
		if (isConfirm)
		{
			HoolaiSDK.instance.releaseResource();
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				GlobalObject.ReStartGame();
			}
			else
			{
				GlobalObject.QuitApp();
			}
		}
	}

	public void getServersCallback(string result)
	{
		UnityEngine.Debug.Log("fastaccess_Unity getServersCallback result:" + result);
		JsonData jsonData = JsonMapper.ToObject(result);
		int num = (int)jsonData["resultCode"];
		if (num == 10)
		{
			UnityEngine.Debug.Log("fastaccess_Unity onGetServers_Success");
			JsonData jsonData2 = jsonData["serverInfos"];
			JsonData jsonData3 = jsonData2["serverList"];
			JsonData jsonData4 = jsonData2["userServerList"];
			for (int i = 0; i < jsonData3.Count; i++)
			{
				JsonData jsonData5 = jsonData3[i];
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"fastaccess_Unity serverId:",
					jsonData5["serverId"],
					" productId:",
					jsonData5["productId"],
					" serverName:",
					jsonData5["serverName"]
				}));
			}
			for (int j = 0; j < jsonData4.Count; j++)
			{
				JsonData jsonData6 = jsonData4[j];
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"fastaccess_Unity serverId:",
					jsonData6["serverId"],
					" productId:",
					jsonData6["productId"],
					" serverName:",
					jsonData6["serverName"]
				}));
			}
		}
		else
		{
			UnityEngine.Debug.Log("fastaccess_Unity onGetServers_Fail");
			string str = (string)jsonData["desc"];
			UnityEngine.Debug.Log("fastaccess_Unity desc:" + str);
		}
	}

	public void selectServerCallback(string result)
	{
		JsonData jsonData = JsonMapper.ToObject(result);
		int num = (int)jsonData["resultCode"];
		if (num == 12)
		{
			UnityEngine.Debug.Log("fastaccess_Unity onSelectServer_Success");
		}
		else
		{
			UnityEngine.Debug.Log("fastaccess_Unity onSelectServer_Fail");
			string str = (string)jsonData["desc"];
			UnityEngine.Debug.Log("fastaccess_Unity desc:" + str);
		}
	}

	public void sendBIDataCallback(string message)
	{
		UnityEngine.Debug.Log("fastaccess_Unity sendBIDataCallback message=" + message);
	}

	private void Update()
	{
		if (!this.isExit && Input.GetKeyDown(KeyCode.Escape))
		{
			this.isExit = true;
			HoolaiSDK.instance.exit(base.gameObject.name, "exitCallback");
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.isExit = false;
		}
		if (Input.touchCount == 7)
		{
			UnityEngine.Debug.Log("ReYun!!!!   Login");
			ReYun.instance.Login("1", 1);
		}
	}

	public void StartSDKInit()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			return;
		}
		if (this.isInit)
		{
			GlobalObject.Instance.SetCanPause(false);
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			HoolaiSDK.instance.login("login");
		}
	}

	public void StartAnySDKInit()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			return;
		}
		if (!this.isInit)
		{
			UnityEngine.Debug.Log("InitAnySDK.getInstance().Init");
			InitAnySDK.getInstance().Init();
		}
		else
		{
			UnityEngine.Debug.Log("InitAnySDK.getInstance().Init  false");
			GlobalObject.Instance.SetCanPause(false);
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
		}
	}

	public void StartSDKLogin()
	{
		if (!this.isInit)
		{
			UnityEngine.Debug.Log("fastaccess_Unity StartSDKLogin   Init");
			this.needLogin = true;
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			HoolaiSDK.instance.init(base.gameObject.name, "initCallback", "loginCallback", "payCallback");
		}
		else
		{
			UnityEngine.Debug.Log("fastaccess_Unity StartSDKLogin   Login");
			GlobalObject.Instance.SetCanPause(false);
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			HoolaiSDK.instance.login("login");
		}
	}

	public void StartAnySDKLogin()
	{
		if (!this.isInit)
		{
			UnityEngine.Debug.Log("fastaccess_Unity StartSDKLogin   Init");
			this.needLogin = true;
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			InitAnySDK.getInstance().Init();
		}
		else
		{
			UnityEngine.Debug.Log("fastaccess_Unity StartSDKLogin   Login");
			GlobalObject.Instance.SetCanPause(false);
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			InitAnySDK.getInstance().login();
		}
	}

	public void StartSDKPay(int price, string itemName, int num)
	{
		if (this.isPayTestOne)
		{
			price = 1;
			itemName = "49001";
		}
		if (GlobalSettings.isLoginByLDSDK)
		{
		}
		if (GlobalSettings.isLoginByAnySDK)
		{
			if (this.isInit)
			{
				NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
				string text = (int.Parse(itemName) - 49000).ToString();
				InitAnySDK.getInstance().payForProduct(price, text, string.Concat(new string[]
				{
					ModelManager.Instance.Get_userData_X().UserId.ToString(),
					"_",
					text.ToString(),
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().serverip,
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().tcpaddress
				}), string.Empty);
				GlobalObject.Instance.forceLockReStart = true;
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("账号状态异常！", 2f);
			}
		}
		else if (GlobalSettings.isLoginByHoolaiSDK)
		{
			if (this.isInit && HoolaiSDK.isLogin)
			{
				NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
				UnityEngine.Debug.Log("fastaccess_Unity StartSDKPay   Init");
				string text2 = (int.Parse(itemName) - 49000).ToString();
				HoolaiSDK.instance.pay(price * 100, text2, string.Concat(new string[]
				{
					ModelManager.Instance.Get_userData_X().UserId.ToString(),
					"_",
					text2.ToString(),
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().serverip,
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().tcpaddress
				}), string.Empty);
				GlobalObject.Instance.forceLockReStart = true;
				AnalyticsToolManager.SetChargeRequest(string.Concat(new string[]
				{
					ModelManager.Instance.Get_userData_X().UserId.ToString(),
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().serverip,
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().tcpaddress
				}), price, "Hoolai");
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("账号状态异常！", 2f);
			}
		}
		else
		{
			if (this.jo == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.xiaomeng.mobaheros.MobaheroMainActivity");
				this.jo = androidJavaClass.CallStatic<AndroidJavaObject>("currentActivity", new object[0]);
			}
			string text3 = (int.Parse(itemName) - 49000).ToString();
			this.jo.Call("startPay", new object[]
			{
				text3,
				price.ToString(),
				ModelManager.Instance.Get_userData_X().UserId.ToString() + "#" + ModelManager.Instance.Get_curLoginServerInfo().serverip,
				string.Concat(new string[]
				{
					ModelManager.Instance.Get_userData_X().UserId.ToString(),
					"_",
					text3.ToString(),
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().serverip,
					"_",
					ModelManager.Instance.Get_curLoginServerInfo().tcpaddress
				}),
				ModelManager.Instance.Get_curLoginServerInfo().serverip
			});
			GlobalObject.Instance.forceLockReStart = true;
		}
	}

	public void CheckSaveProvideContent()
	{
		if (GlobalSettings.isIOSInAppPurchase && InitSDK.IAPMgr == null)
		{
			InitSDK.IAPMgr = InitSDK.instance.gameObject.AddComponent<IAPManager>();
			InitSDK.IAPMgr.CheckSaveProvideContent();
		}
	}

	public void SDKLogout(bool _logoutNeedRest = true)
	{
		this.logoutNeedRest = _logoutNeedRest;
		GlobalObject.Instance.SetCanPause(false);
		HoolaiSDK.instance.logout("logout");
	}

	public void SDKAnySDKLogout(bool _logoutNeedRest = true)
	{
		this.logoutNeedRest = _logoutNeedRest;
		GlobalObject.Instance.SetCanPause(false);
		InitAnySDK.getInstance().logout();
	}

	public void SDKExit()
	{
		GlobalObject.Instance.SetCanPause(false);
		HoolaiSDK.instance.exit(base.gameObject.name, "exitCallback");
	}

	public void AnySDKExit(bool _logoutNeedRest = true)
	{
		GlobalObject.Instance.SetCanPause(false);
		InitAnySDK.getInstance().exit();
	}
}
