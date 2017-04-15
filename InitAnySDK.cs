using anysdk;
using Assets.Scripts.Model;
using Com.Game.Module;
using GameLogin;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InitAnySDK : MonoBehaviour
{
	public static InitAnySDK _instance;

	public AnySDKUser anySDKUser;

	private AnySDKIAP anySDKIAP;

	private string appKey = "26053E3D-3259-6562-F0BF-2C9A5E6DE327";

	private string appSecret = "f94f0efc4d99d9d00bd2a5469a60f700";

	private string privateKey = "96C0987379445198BFF9C4060B1DE79E";

	private string oauthLoginServer = "http://oauth.anysdk.com/api/OauthLoginDemo/Login.php";

	private CoroutineManager coroutineManager = new CoroutineManager();

	public static InitAnySDK getInstance()
	{
		if (null == InitAnySDK._instance)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "AnySDK";
			InitAnySDK._instance = gameObject.AddComponent<InitAnySDK>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		return InitAnySDK._instance;
	}

	public void Init()
	{
		Debug.Log("InitAnySDK  init");
		AnySDK.getInstance().init(this.appKey, this.appSecret, this.privateKey, this.oauthLoginServer);
		Debug.Log("InitAnySDK  setListener");
		this.anySDKUser = AnySDKUser.getInstance();
		this.anySDKUser.setListener(this, "UserExternalCall");
		this.anySDKIAP = AnySDKIAP.getInstance();
		this.anySDKIAP.setListener(this, "IAPExternalCall");
	}

	public void UserExternalCall(string msg)
	{
		Debug.Log("UserExternalCall(" + msg + ")");
		Dictionary<string, string> dictionary = AnySDKUtil.stringToDictionary(msg);
		int num = Convert.ToInt32(dictionary["code"]);
		string text = dictionary["msg"];
		switch (num)
		{
		case 0:
			InitSDK.instance.isInit = true;
			if (InitSDK.instance.needLogin)
			{
				NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
				this.login();
			}
			else
			{
				LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_Init);
			}
			InitSDK.instance.needLogin = false;
			break;
		case 1:
			InitSDK.instance.isInit = false;
			Debug.Log("InitAnySDK init fail");
			CtrlManager.ShowMsgBox("初始化失败", "是否进行重试？", new Action<bool>(this.AgainCall), PopViewType.PopOneButton, "确定", "取消", null);
			break;
		case 2:
			GlobalObject.Instance.SetCanPause(true);
			this.coroutineManager.StartCoroutine(InitSDK.instance.TryLoginByChannelId(AnySDK.getInstance().getChannelId(), this.anySDKUser.getUserID()), true);
			break;
		case 3:
		case 5:
		case 6:
			Singleton<LoginView_New>.Instance.SDKLoginFail();
			if (Singleton<NewWaitingView>.Instance.IsOpen)
			{
				Singleton<NewWaitingView>.Instance.Destroy();
			}
			break;
		case 7:
			if (Singleton<AreaView>.Instance.gameObject != null && Singleton<AreaView>.Instance.gameObject.active)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.AreaViewNew_goback, null, false);
			}
			else
			{
				CtrlManager.ShowMsgBox("账号登出", "重新登陆", new Action<bool>(this.ExitCall), PopViewType.PopOneButton, "确定", "取消", null);
			}
			break;
		case 8:
			if (Singleton<AreaView>.Instance.gameObject != null && Singleton<AreaView>.Instance.gameObject.active)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.AreaViewNew_goback, null, false);
			}
			else
			{
				CtrlManager.ShowMsgBox("账号登出失败", "重新登陆", new Action<bool>(this.ExitCall), PopViewType.PopOneButton, "确定", "取消", null);
			}
			break;
		case 9:
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			break;
		case 10:
			if (Singleton<NewWaitingView>.Instance.IsOpen && Singleton<NewWaitingView>.Instance.gameObject != null)
			{
				Singleton<NewWaitingView>.Instance.Destroy();
			}
			break;
		case 12:
			if (msg == "onGameExit" || msg == "onNo3rdExiterProvide")
			{
				CtrlManager.ShowMsgBox("确认退出", "确定退出刺激好玩的《魔霸英雄》吗？", new Action<bool>(this.ExitCall), PopViewType.PopTwoButton, "确定", "取消", null);
			}
			else
			{
				this.ExitCall(true);
			}
			break;
		case 15:
			if (Singleton<AreaView>.Instance.gameObject != null && Singleton<AreaView>.Instance.gameObject.active)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.AreaViewNew_goback, null, false);
				this.coroutineManager.StartCoroutine(InitSDK.instance.TryLoginByChannelId(AnySDK.getInstance().getChannelId(), this.anySDKUser.getUserID()), true);
			}
			else
			{
				CtrlManager.ShowMsgBox("账号切换成功", "重新登陆", new Action<bool>(this.ExitCall), PopViewType.PopOneButton, "确定", "取消", null);
			}
			break;
		case 16:
			if (Singleton<AreaView>.Instance.gameObject != null && Singleton<AreaView>.Instance.gameObject.active)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.AreaViewNew_goback, null, false);
			}
			else
			{
				CtrlManager.ShowMsgBox("账号切换失败", "重新登陆", new Action<bool>(this.ExitCall), PopViewType.PopOneButton, "确定", "取消", null);
			}
			break;
		case 17:
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			break;
		case 19:
			CtrlManager.ShowMsgBox("确认退出", "确定退出刺激好玩的《魔霸英雄》吗？", new Action<bool>(this.ExitCall), PopViewType.PopTwoButton, "确定", "取消", null);
			break;
		}
	}

	public void ExitCall(bool isConfirm)
	{
		if (isConfirm)
		{
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

	private void AgainCall(bool obj)
	{
		AnySDK.getInstance().init(this.appKey, this.appSecret, this.privateKey, this.oauthLoginServer);
	}

	private void IAPExternalCall(string msg)
	{
		Debug.Log("IAPExternalCall(" + msg + ")");
		Dictionary<string, string> dictionary = AnySDKUtil.stringToDictionary(msg);
		int num = Convert.ToInt32(dictionary["code"]);
		string text = dictionary["msg"];
		switch (num)
		{
		case 0:
			Debug.Log("IAPExternalCall(" + this.anySDKIAP.getOrderId(this.anySDKIAP.getPluginId()[0]) + ")");
			InitSDK.instance.orderBack("0");
			break;
		case 1:
			Singleton<TipView>.Instance.ShowViewSetText("支付失败,请重试！", 2f);
			break;
		case 2:
			Singleton<TipView>.Instance.ShowViewSetText("支付取消,请重试！", 2f);
			break;
		case 3:
			Singleton<TipView>.Instance.ShowViewSetText("支付超时,请重试！", 2f);
			break;
		case 4:
			Singleton<TipView>.Instance.ShowViewSetText("支付信息不完整,请重试！", 2f);
			break;
		case 7:
			Singleton<TipView>.Instance.ShowViewSetText("支付正在进行中，未收到支付结果。是否需要等待，若不等待则进行下一次的支付！", 2f);
			break;
		}
	}

	private void getOrderId()
	{
		string orderId = this.anySDKIAP.getOrderId(string.Empty);
		Debug.Log("AnySDK@ getOrder id " + orderId);
	}

	public void payForProduct(int amount, string itemName, string callbackInfo, string customParams)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["Product_Id"] = itemName;
		dictionary["Product_Name"] = itemName;
		dictionary["Product_Price"] = amount.ToString();
		dictionary["Product_Count"] = "1";
		dictionary["Product_Desc"] = "钻石";
		dictionary["Coin_Name"] = "钻石";
		dictionary["Coin_Rate"] = "10";
		dictionary["Role_Id"] = ModelManager.Instance.Get_userData_X().UserId;
		dictionary["Role_Name"] = ModelManager.Instance.Get_userData_X().NickName;
		dictionary["Role_Grade"] = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp).ToString();
		dictionary["Server_Id"] = (ModelManager.Instance.Get_curLoginServerInfo().areaId + 1).ToString();
		dictionary["Server_Name"] = ModelManager.Instance.Get_curLoginServerInfo().servername;
		dictionary["EXT"] = callbackInfo;
		this.anySDKIAP.payForProduct(dictionary, string.Empty);
	}

	public void login()
	{
		this.anySDKUser.login();
	}

	public void logout()
	{
		if (this.anySDKUser.isFunctionSupported("logout"))
		{
			this.anySDKUser.callFuncWithParam("logout", null);
		}
	}

	public void enterPlatform()
	{
		if (this.anySDKUser.isFunctionSupported("enterPlatform"))
		{
			this.anySDKUser.callFuncWithParam("enterPlatform", null);
		}
	}

	public void showToolBar(ToolBarPlace align)
	{
		if (this.anySDKUser.isFunctionSupported("showToolBar"))
		{
			AnySDKParam param = new AnySDKParam((int)align);
			this.anySDKUser.callFuncWithParam("showToolBar", param);
		}
	}

	public void hideToolBar()
	{
		if (this.anySDKUser.isFunctionSupported("hideToolBar"))
		{
			this.anySDKUser.callFuncWithParam("hideToolBar", null);
		}
	}

	public void accountSwitch()
	{
		if (this.anySDKUser.isFunctionSupported("accountSwitch"))
		{
			this.anySDKUser.callFuncWithParam("accountSwitch", null);
		}
	}

	public void antiAddictionQuery()
	{
		if (this.anySDKUser.isFunctionSupported("antiAddictionQuery"))
		{
			this.anySDKUser.callFuncWithParam("antiAddictionQuery", null);
		}
	}

	public void realNameRegister()
	{
		if (this.anySDKUser.isFunctionSupported("realNameRegister"))
		{
			this.anySDKUser.callFuncWithParam("realNameRegister", null);
		}
	}

	public void exit()
	{
		if (this.anySDKUser.isFunctionSupported("exit"))
		{
			this.anySDKUser.callFuncWithParam("exit", null);
		}
	}

	public void submitLoginGameRole(string type)
	{
		if (this.anySDKUser.isFunctionSupported("submitLoginGameRole"))
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			UserData userData = ModelManager.Instance.Get_userData_X();
			dictionary["dataType"] = type;
			dictionary["roleId"] = userData.UserId;
			int loginCount = ModelManager.Instance.Get_userData_X().LoginCount;
			if (loginCount < 1)
			{
				dictionary["roleCTime"] = "-1";
				dictionary["roleName"] = userData.UserId;
			}
			else
			{
				if (type == "2")
				{
					dictionary["roleCTime"] = ToolsFacade.ServerCurrentTime.ToShortTimeString();
				}
				dictionary["roleName"] = userData.NickName;
			}
			dictionary["roleLevel"] = CharacterDataMgr.instance.GetUserLevel(userData.Exp).ToString();
			dictionary["zoneId"] = (ModelManager.Instance.Get_curLoginServerInfo().areaId + 1).ToString();
			dictionary["zoneName"] = ModelManager.Instance.Get_curLoginServerInfo().servername.ToString();
			dictionary["balance"] = userData.Diamonds.ToString();
			dictionary["partyName"] = "无工会";
			dictionary["vipLevel"] = "0";
			if (type == "3")
			{
				dictionary["roleCTime"] = ToolsFacade.ServerCurrentTime.ToShortTimeString();
			}
			else
			{
				dictionary["roleLevelMTime"] = "-1";
			}
			AnySDKParam param = new AnySDKParam(dictionary);
			this.anySDKUser.callFuncWithParam("submitLoginGameRole", param);
		}
	}
}
