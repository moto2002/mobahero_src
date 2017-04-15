using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestUserPlugin : MonoBehaviour
	{
		private AnySDKUser _instance;

		private void Awake()
		{
		}

		private void Start()
		{
			this._instance = AnySDKUser.getInstance();
			this._instance.setListener(this, "UserExternalCall");
		}

		private void UserExternalCall(string msg)
		{
			Debug.Log("UserExternalCall(" + msg + ")");
			Dictionary<string, string> dictionary = AnySDKUtil.stringToDictionary(msg);
			int num = Convert.ToInt32(dictionary["code"]);
			string text = dictionary["msg"];
			switch (num)
			{
			}
		}

		private void OnDestroy()
		{
		}

		private void OnGUI()
		{
			GUI.color = Color.white;
			GUI.skin.button.fontSize = 30;
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "Login"))
			{
				this.login();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "logout"))
			{
				this.logout();
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "enterPlatform"))
			{
				this.enterPlatform();
			}
			if (GUI.Button(new Rect(5f, 230f, (float)(Screen.width - 10), 70f), "showToolBar"))
			{
				this.showToolBar(ToolBarPlace.kToolBarBottomLeft);
			}
			if (GUI.Button(new Rect(5f, 305f, (float)(Screen.width - 10), 70f), "hideToolBar"))
			{
				this.hideToolBar();
			}
			if (GUI.Button(new Rect(5f, 380f, (float)(Screen.width - 10), 70f), "accountSwitch"))
			{
				this.accountSwitch();
			}
			if (GUI.Button(new Rect(5f, 455f, (float)(Screen.width - 10), 70f), "antiAddictionQuery"))
			{
				this.antiAddictionQuery();
			}
			if (GUI.Button(new Rect(5f, 530f, (float)(Screen.width - 10), 70f), "realNameRegister"))
			{
				this.realNameRegister();
			}
			if (GUI.Button(new Rect(5f, 605f, (float)(Screen.width - 10), 70f), "submitLoginGameRole"))
			{
				this.submitLoginGameRole();
			}
			if (GUI.Button(new Rect(5f, 680f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestUserPlugin"));
				base.gameObject.AddComponent<Test>();
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
			{
				Application.Quit();
				AnySDK.getInstance().release();
			}
		}

		private void login()
		{
			this._instance.login();
		}

		private void logout()
		{
			if (this._instance.isFunctionSupported("logout"))
			{
				this._instance.callFuncWithParam("logout", null);
			}
		}

		private void enterPlatform()
		{
			if (this._instance.isFunctionSupported("enterPlatform"))
			{
				this._instance.callFuncWithParam("enterPlatform", null);
			}
		}

		private void showToolBar(ToolBarPlace align)
		{
			if (this._instance.isFunctionSupported("showToolBar"))
			{
				AnySDKParam param = new AnySDKParam((int)align);
				this._instance.callFuncWithParam("showToolBar", param);
			}
		}

		private void hideToolBar()
		{
			if (this._instance.isFunctionSupported("hideToolBar"))
			{
				this._instance.callFuncWithParam("hideToolBar", null);
			}
		}

		private void accountSwitch()
		{
			if (this._instance.isFunctionSupported("accountSwitch"))
			{
				this._instance.callFuncWithParam("accountSwitch", null);
			}
		}

		private void antiAddictionQuery()
		{
			if (this._instance.isFunctionSupported("antiAddictionQuery"))
			{
				this._instance.callFuncWithParam("antiAddictionQuery", null);
			}
		}

		private void realNameRegister()
		{
			if (this._instance.isFunctionSupported("realNameRegister"))
			{
				this._instance.callFuncWithParam("realNameRegister", null);
			}
		}

		private void submitLoginGameRole()
		{
			if (this._instance.isFunctionSupported("submitLoginGameRole"))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["dataType"] = "1";
				dictionary["roleId"] = "123456";
				dictionary["roleName"] = "test";
				dictionary["roleLevel"] = "1";
				dictionary["zoneId"] = "1";
				dictionary["zoneName"] = "test";
				dictionary["balance"] = "60";
				dictionary["partyName"] = "test";
				dictionary["vipLevel"] = "1";
				dictionary["roleCTime"] = "1480318110";
				dictionary["roleLevelMTime"] = "-1";
				AnySDKParam param = new AnySDKParam(dictionary);
				this._instance.callFuncWithParam("submitLoginGameRole", param);
			}
		}
	}
}
