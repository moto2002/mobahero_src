using System;
using UnityEngine;

public class Test : MonoBehaviour
{
	private static string msgresult;

	private PlatformManager mgr;

	private void Start()
	{
		this.mgr = base.gameObject.AddComponent<PlatformManager>();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(20f, 20f, 180f, 80f), "初始化SDK"))
		{
			this.ShowResult("初始化SDK");
			this.mgr.Init();
		}
		if (GUI.Button(new Rect(220f, 20f, 180f, 80f), "账号登陆"))
		{
			this.ShowResult("账号登陆");
			this.mgr.Login();
		}
		if (GUI.Button(new Rect(420f, 20f, 180f, 80f), "游客登陆"))
		{
			this.ShowResult("游客登陆");
			this.mgr.GuestLogin();
		}
		if (GUI.Button(new Rect(20f, 120f, 180f, 80f), "显示工具条"))
		{
			this.ShowResult("显示工具条");
			this.mgr.ShowToolbar();
		}
		if (GUI.Button(new Rect(220f, 120f, 180f, 80f), "隐藏工具条"))
		{
			this.ShowResult("隐藏工具条");
			this.mgr.HideToolbar();
		}
		if (GUI.Button(new Rect(420f, 120f, 180f, 80f), "暂停页"))
		{
			this.ShowResult("暂停页");
			this.mgr.Pause();
		}
		if (GUI.Button(new Rect(20f, 220f, 180f, 80f), "同步支付"))
		{
			this.ShowResult("同步支付");
			this.ShowResult("同步支付RESULT==> ");
		}
		if (GUI.Button(new Rect(220f, 220f, 180f, 80f), "异步支付"))
		{
			this.ShowResult("异步支付");
			int num = this.mgr.StartPay(Guid.NewGuid().ToString(), "2", "PEAR", 1f, 2, "GAMEZOON2");
			this.ShowResult("异步支付RESULT==> " + num);
		}
		if (GUI.Button(new Rect(420f, 220f, 180f, 80f), "代币充值"))
		{
			this.ShowResult("代币充值");
			this.ShowResult("代币充值RESULT==> ");
		}
		if (GUI.Button(new Rect(20f, 320f, 180f, 80f), "注销账号"))
		{
			this.ShowResult("注销账号,param:1");
			this.mgr.Logout();
		}
		if (GUI.Button(new Rect(220f, 320f, 180f, 80f), "获取uin"))
		{
			this.ShowResult("uin=>" + this.mgr.loginUin);
		}
		if (GUI.Button(new Rect(420f, 320f, 180f, 80f), "获取sessionid"))
		{
			this.ShowResult("sessionId=>" + this.mgr.SessionId);
		}
		if (GUI.Button(new Rect(20f, 420f, 180f, 80f), "获取State"))
		{
			this.ShowResult("LoginState=>" + this.mgr.GetCurrentLoginState());
		}
		if (GUI.Button(new Rect(220f, 420f, 180f, 80f), "获取IsLogined"))
		{
			this.ShowResult("IsLogined=>" + this.mgr.isLogined);
		}
		if (GUI.Button(new Rect(420f, 420f, 180f, 80f), "获取nickName"))
		{
			this.ShowResult("nickName=>" + this.mgr.UserNick);
		}
		GUI.Label(new Rect(20f, 520f, 800f, 1500f), "msg:\r\n" + Test.msgresult);
	}

	private void ShowResult(string result)
	{
		Test.msgresult = result + "\r\n" + Test.msgresult;
	}
}
