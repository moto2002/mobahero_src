using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestAnalyticsPlugin : MonoBehaviour
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
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "startSession"))
			{
				AnySDKAnalytics.getInstance().setDebugMode(true);
				this.startSession();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "stopSession"))
			{
				this.stopSession();
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "setSessionContinueMillis"))
			{
				this.setSessionContinueMillis();
			}
			if (GUI.Button(new Rect(5f, 230f, (float)(Screen.width - 10), 70f), "logError"))
			{
				this.logError();
			}
			if (GUI.Button(new Rect(5f, 305f, (float)(Screen.width - 10), 70f), "logEvent"))
			{
				this.logEvent();
			}
			if (GUI.Button(new Rect(5f, 380f, (float)(Screen.width - 10), 70f), "logTimedEventBegin"))
			{
				this.logTimedEventBegin();
			}
			if (GUI.Button(new Rect(5f, 455f, (float)(Screen.width - 10), 70f), "logTimedEventEnd"))
			{
				this.logTimedEventEnd();
			}
			if (GUI.Button(new Rect(5f, 530f, (float)(Screen.width - 10), 70f), "setCaptureUncaughtException"))
			{
				this.setCaptureUncaughtException();
			}
			if (GUI.Button(new Rect(5f, 605f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestAnalyticsPlugin"));
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

		private void startSession()
		{
			AnySDKAnalytics.getInstance().startSession();
		}

		private void stopSession()
		{
			AnySDKAnalytics.getInstance().stopSession();
		}

		private void logError()
		{
			AnySDKAnalytics.getInstance().logError("100", "test test test");
		}

		private void logEvent()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["100"] = "Page1 open";
			dictionary["角色名称"] = "yonghu";
			dictionary["登陆时间"] = "2014";
			AnySDKAnalytics.getInstance().logEvent("10", dictionary);
		}

		private void setSessionContinueMillis()
		{
			AnySDKAnalytics.getInstance().setSessionContinueMillis(2000L);
		}

		private void setCaptureUncaughtException()
		{
			AnySDKAnalytics.getInstance().setCaptureUncaughtException(true);
		}

		private void logTimedEventBegin()
		{
			AnySDKAnalytics.getInstance().logTimedEventBegin("10");
		}

		private void logTimedEventEnd()
		{
			AnySDKAnalytics.getInstance().logTimedEventEnd("10");
		}

		private void setAccount()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Account_Id"] = "123";
			dictionary["Account_Name"] = "test";
			dictionary["Account_Type"] = Convert.ToString(0);
			dictionary["Account_Level"] = "1";
			dictionary["Account_Age"] = "1";
			dictionary["Account_Operate"] = Convert.ToString(0);
			dictionary["Account_Gender"] = Convert.ToString(0);
			dictionary["Server_Id"] = "1";
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("setAccount", param);
		}

		private void onChargeRequest()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Order_Id"] = "123456";
			dictionary["Product_Name"] = "test";
			dictionary["Currency_Amount"] = Convert.ToString(2.0);
			dictionary["Currency_Type"] = "1";
			dictionary["Payment_Type"] = "1";
			dictionary["Virtual_Currency_Amount"] = Convert.ToString(100);
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("onChargeRequest", param);
		}

		private void onChargeOnlySuccess()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Order_Id"] = "123456";
			dictionary["Product_Name"] = "test";
			dictionary["Currency_Amount"] = Convert.ToString(2.0);
			dictionary["Currency_Type"] = "1";
			dictionary["Payment_Type"] = "1";
			dictionary["Virtual_Currency_Amount"] = Convert.ToString(100);
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("onChargeOnlySuccess", param);
		}

		private void onChargeSuccess()
		{
			AnySDKAnalytics.getInstance().callFuncWithParam("onChargeSuccess", new AnySDKParam("123456"));
		}

		private void onChargeFail()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Order_Id"] = "123456";
			dictionary["Fail_Reason"] = "test";
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("onChargeFail", param);
		}

		private void onPurchase()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Item_Id"] = "123456";
			dictionary["Item_Type"] = "test";
			dictionary["Item_Count"] = Convert.ToString(2);
			dictionary["Virtual_Currency"] = "1";
			dictionary["Currency_Type"] = AnySDK.getInstance().getChannelId();
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("onPurchase", param);
		}

		private void onUse()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Item_Id"] = "123456";
			dictionary["Item_Type"] = "test";
			dictionary["Item_Count"] = Convert.ToString(2);
			dictionary["Use_Reason"] = "1";
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("onUse", param);
		}

		private void onReward()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Item_Id"] = "123456";
			dictionary["Item_Type"] = "test";
			dictionary["Item_Count"] = Convert.ToString(2);
			dictionary["Use_Reason"] = "1";
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("onReward", param);
		}

		private void startLevel()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Level_Id"] = "123456";
			dictionary["Seq_Num"] = "1";
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("startLevel", param);
		}

		private void finishLevel()
		{
			AnySDKAnalytics.getInstance().callFuncWithParam("finishLevel", new AnySDKParam("123456"));
		}

		private void failLevel()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Level_Id"] = "123456";
			dictionary["Fail_Reason"] = "test";
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("failLevel", param);
		}

		private void startTask()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Task_Id"] = "123456";
			dictionary["Task_Type"] = Convert.ToString(0);
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("startTask", param);
		}

		private void finishTask()
		{
			AnySDKAnalytics.getInstance().callFuncWithParam("finishTask", new AnySDKParam("123456"));
		}

		private void failTask()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Task_Id"] = "123456";
			dictionary["Fail_Reason"] = "test";
			AnySDKParam param = new AnySDKParam(dictionary);
			AnySDKAnalytics.getInstance().callFuncWithParam("failTask", param);
		}
	}
}
