using cn.sharesdk.unity3d;
using System;
using System.Collections;
using UnityEngine;

public class Demo : MonoBehaviour
{
	public GUISkin demoSkin;

	public ShareSDK ssdk;

	private void Start()
	{
		this.ssdk = base.gameObject.GetComponent<ShareSDK>();
		this.ssdk.authHandler = new ShareSDK.EventHandler(this.OnAuthResultHandler);
		this.ssdk.shareHandler = new ShareSDK.EventHandler(this.OnShareResultHandler);
		this.ssdk.showUserHandler = new ShareSDK.EventHandler(this.OnGetUserInfoResultHandler);
		this.ssdk.getFriendsHandler = new ShareSDK.EventHandler(this.OnGetFriendsResultHandler);
		this.ssdk.followFriendHandler = new ShareSDK.EventHandler(this.OnFollowFriendResultHandler);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnGUI()
	{
		GUI.skin = this.demoSkin;
		float num = 1f;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			num = (float)(Screen.width / 320);
		}
		float num2 = 165f * num;
		float num3 = 30f * num;
		float num4 = 20f * num;
		float num5 = 20f * num;
		GUI.skin.button.fontSize = Convert.ToInt32(14f * num);
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f - num2, num4, num2, num3), "Authorize"))
		{
			Debug.Log(this.ssdk == null);
			this.ssdk.Authorize(PlatformType.SinaWeibo);
		}
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f + num5, num4, num2, num3), "Get User Info"))
		{
			this.ssdk.GetUserInfo(PlatformType.SinaWeibo);
		}
		num4 += num3 + 20f * num;
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f - num2, num4, num2, num3), "Show Share Menu"))
		{
			ShareContent shareContent = new ShareContent();
			shareContent.SetText("this is a test string.");
			shareContent.SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
			shareContent.SetTitle("test title");
			shareContent.SetTitleUrl("http://www.mob.com");
			shareContent.SetSite("Mob-ShareSDK");
			shareContent.SetSiteUrl("http://www.mob.com");
			shareContent.SetUrl("http://www.mob.com");
			shareContent.SetComment("test description");
			shareContent.SetMusicUrl("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
			shareContent.SetShareType(2);
			ShareContent shareContent2 = new ShareContent();
			shareContent2.SetText("Sina share content");
			shareContent2.SetImageUrl("http://git.oschina.net/alexyu.yxj/MyTmpFiles/raw/master/kmk_pic_fld/small/107.JPG");
			shareContent2.SetShareType(2);
			shareContent2.SetObjectID("SinaID");
			shareContent.SetShareContentCustomize(PlatformType.SinaWeibo, shareContent2);
			this.ssdk.ShowPlatformList(null, shareContent, 100, 100);
		}
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f + num5, num4, num2, num3), "Show Share View"))
		{
			ShareContent shareContent3 = new ShareContent();
			shareContent3.SetText("this is a test string.");
			shareContent3.SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
			shareContent3.SetTitle("test title");
			shareContent3.SetTitleUrl("http://www.mob.com");
			shareContent3.SetSite("Mob-ShareSDK");
			shareContent3.SetSiteUrl("http://www.mob.com");
			shareContent3.SetUrl("http://www.mob.com");
			shareContent3.SetComment("test description");
			shareContent3.SetMusicUrl("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
			shareContent3.SetShareType(2);
			this.ssdk.ShowShareContentEditor(PlatformType.SinaWeibo, shareContent3);
		}
		num4 += num3 + 20f * num;
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f - num2, num4, num2, num3), "Share Content"))
		{
			ShareContent shareContent4 = new ShareContent();
			shareContent4.SetText("this is a test string.");
			shareContent4.SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
			shareContent4.SetTitle("test title");
			shareContent4.SetTitleUrl("http://www.mob.com");
			shareContent4.SetSite("Mob-ShareSDK");
			shareContent4.SetSiteUrl("http://www.mob.com");
			shareContent4.SetUrl("http://www.mob.com");
			shareContent4.SetComment("test description");
			shareContent4.SetMusicUrl("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
			shareContent4.SetShareType(2);
			this.ssdk.ShareContent(PlatformType.SinaWeibo, shareContent4);
		}
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f + num5, num4, num2, num3), "Get Friends SinaWeibo "))
		{
			Debug.Log("Click Btn Of Get Friends SinaWeibo");
			this.ssdk.GetFriendList(PlatformType.SinaWeibo, 15, 0);
		}
		num4 += num3 + 20f * num;
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f - num2, num4, num2, num3), "Get Token SinaWeibo "))
		{
			Hashtable authInfo = this.ssdk.GetAuthInfo(PlatformType.SinaWeibo);
			Debug.Log("share result :");
			Debug.Log(MiniJSON.jsonEncode(authInfo));
		}
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f + num5, num4, num2, num3), "Close SSO Auth"))
		{
			this.ssdk.DisableSSO(true);
		}
		num4 += num3 + 20f * num;
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f - num2, num4, num2, num3), "Remove Authorize "))
		{
			this.ssdk.CancelAuthorize(PlatformType.SinaWeibo);
		}
		if (GUI.Button(new Rect(((float)Screen.width - num5) / 2f + num5, num4, num2, num3), "Add Friend "))
		{
			this.ssdk.AddFriend(PlatformType.SinaWeibo, "3189087725");
		}
		num4 += num3 + 20f * num;
		if (GUI.Button(new Rect(((float)Screen.width - num2) / 2f, num4, num2, num3), "ShareWithContentName"))
		{
			Hashtable hashtable = new Hashtable();
			hashtable["imgUrl"] = "http://ww1.sinaimg.cn/mw690/006dJESWgw1f6iyb8bzraj31kw0v67a2.jpg";
			this.ssdk.ShareWithContentName(PlatformType.SinaWeibo, "ShareSDK", hashtable);
		}
		num2 += 80f * num;
		num4 += num3 + 20f * num;
		if (GUI.Button(new Rect(((float)Screen.width - num2) / 2f, num4, num2, num3), "ShowShareMenuWithContentName"))
		{
			Hashtable hashtable2 = new Hashtable();
			hashtable2["imgUrl"] = "http://ww1.sinaimg.cn/mw690/006dJESWgw1f6iyb8bzraj31kw0v67a2.jpg";
			this.ssdk.ShowPlatformListWithContentName("ShareSDK", hashtable2, null, 100, 100);
		}
		num4 += num3 + 20f * num;
		if (GUI.Button(new Rect(((float)Screen.width - num2) / 2f, num4, num2, num3), "ShowShareViewWithContentName"))
		{
			Hashtable hashtable3 = new Hashtable();
			hashtable3["imgUrl"] = "http://ww1.sinaimg.cn/mw690/006dJESWgw1f6iyb8bzraj31kw0v67a2.jpg";
			this.ssdk.ShowShareContentEditorWithContentName(PlatformType.SinaWeibo, "ShareSDK", hashtable3);
		}
	}

	private void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			Debug.Log("authorize success !Platform :" + type);
		}
		else if (state == ResponseState.Fail)
		{
			Debug.Log(string.Concat(new object[]
			{
				"fail! throwable stack = ",
				result["stack"],
				"; error msg = ",
				result["msg"]
			}));
		}
		else if (state == ResponseState.Cancel)
		{
			Debug.Log("cancel !");
		}
	}

	private void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			Debug.Log("get user info result :");
			Debug.Log(MiniJSON.jsonEncode(result));
			Debug.Log("Get userInfo success !Platform :" + type);
		}
		else if (state == ResponseState.Fail)
		{
			Debug.Log(string.Concat(new object[]
			{
				"fail! throwable stack = ",
				result["stack"],
				"; error msg = ",
				result["msg"]
			}));
		}
		else if (state == ResponseState.Cancel)
		{
			Debug.Log("cancel !");
		}
	}

	private void OnShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			Debug.Log("share successfully - share result :");
			Debug.Log(MiniJSON.jsonEncode(result));
		}
		else if (state == ResponseState.Fail)
		{
			Debug.Log(string.Concat(new object[]
			{
				"fail! throwable stack = ",
				result["stack"],
				"; error msg = ",
				result["msg"]
			}));
		}
		else if (state == ResponseState.Cancel)
		{
			Debug.Log("cancel !");
		}
	}

	private void OnGetFriendsResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			Debug.Log("get friend list result :");
			Debug.Log(MiniJSON.jsonEncode(result));
		}
		else if (state == ResponseState.Fail)
		{
			Debug.Log(string.Concat(new object[]
			{
				"fail! throwable stack = ",
				result["stack"],
				"; error msg = ",
				result["msg"]
			}));
		}
		else if (state == ResponseState.Cancel)
		{
			Debug.Log("cancel !");
		}
	}

	private void OnFollowFriendResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			Debug.Log("Follow friend successfully !");
		}
		else if (state == ResponseState.Fail)
		{
			Debug.Log(string.Concat(new object[]
			{
				"fail! throwable stack = ",
				result["stack"],
				"; error msg = ",
				result["msg"]
			}));
		}
		else if (state == ResponseState.Cancel)
		{
			Debug.Log("cancel !");
		}
	}
}
