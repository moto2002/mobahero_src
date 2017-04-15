using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestAdTrackingPlugin : MonoBehaviour
	{
		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void OnDestroy()
		{
		}

		private void OnGUI()
		{
			GUI.color = Color.white;
			GUI.skin.button.fontSize = 30;
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "onRegister"))
			{
				this.onRegister();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "onLogin"))
			{
				this.onLogin();
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "onPay"))
			{
				this.onPay();
			}
			if (GUI.Button(new Rect(5f, 230f, (float)(Screen.width - 10), 70f), "trackEvent"))
			{
				this.trackEvent();
			}
			if (GUI.Button(new Rect(5f, 305f, (float)(Screen.width - 10), 70f), "onCreateRole"))
			{
				this.onCreateRole();
			}
			if (GUI.Button(new Rect(5f, 380f, (float)(Screen.width - 10), 70f), "onLevelUp"))
			{
				this.onLevelUp();
			}
			if (GUI.Button(new Rect(5f, 455f, (float)(Screen.width - 10), 70f), "onStartToPay"))
			{
				this.onStartToPay();
			}
			if (GUI.Button(new Rect(5f, 525f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestAdTrackingPlugin"));
				base.gameObject.AddComponent("Test");
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

		private void onRegister()
		{
			AnySDKAdTracking.getInstance().onRegister("user_unity");
		}

		private void onLogin()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["User_Id"] = "123456";
			dictionary["Role_Id"] = "test";
			dictionary["Role_Name"] = "test";
			dictionary["Level"] = "10";
			AnySDKAdTracking.getInstance().onLogin(dictionary);
		}

		private void onPay()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["User_Id"] = "123456";
			dictionary["Order_Id"] = DateTime.Now.ToString();
			dictionary["Currency_Amount"] = "5";
			dictionary["Currency_Type"] = "CNY";
			dictionary["Payment_Type"] = "test";
			dictionary["Payment_Time"] = DateTime.Now.ToString();
			AnySDKAdTracking.getInstance().onPay(dictionary);
		}

		private void trackEvent()
		{
			AnySDKAdTracking.getInstance().trackEvent("event_1", null);
			AnySDKAdTracking.getInstance().trackEvent("event_2", null);
			AnySDKAdTracking.getInstance().trackEvent("onCustEvent2", null);
			AnySDKAdTracking.getInstance().trackEvent("onCustEvent1", null);
		}

		private void onCreateRole()
		{
			if (!AnySDKAdTracking.getInstance().isFunctionSupported("onStartToPay"))
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["User_Id"] = "123456";
			dictionary["Role_Id"] = "test";
			dictionary["Role_Name"] = "test";
			dictionary["Level"] = "10";
			AnySDKAdTracking.getInstance().trackEvent("onCreateRole", dictionary);
			AnySDKAdTracking.getInstance().callFuncWithParam("onCreateRole", new AnySDKParam(dictionary));
		}

		private void onStartToPay()
		{
			if (!AnySDKAdTracking.getInstance().isFunctionSupported("onStartToPay"))
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["User_Id"] = "123456";
			dictionary["Order_Id"] = DateTime.Now.ToString();
			dictionary["Currency_Amount"] = "5";
			dictionary["Currency_Type"] = "CNY";
			dictionary["Payment_Type"] = "test";
			dictionary["Payment_Time"] = DateTime.Now.ToString();
			AnySDKAdTracking.getInstance().trackEvent("onStartToPay", dictionary);
			AnySDKAdTracking.getInstance().callFuncWithParam("onStartToPay", new AnySDKParam(dictionary));
		}

		private void onLevelUp()
		{
			if (!AnySDKAdTracking.getInstance().isFunctionSupported("onLevelUp"))
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["User_Id"] = "123456";
			dictionary["Role_Id"] = "test";
			dictionary["Role_Name"] = "test";
			dictionary["Level"] = "10";
			AnySDKAdTracking.getInstance().trackEvent("onLevelUp", dictionary);
			AnySDKAdTracking.getInstance().callFuncWithParam("onLevelUp", new AnySDKParam(dictionary));
		}
	}
}
